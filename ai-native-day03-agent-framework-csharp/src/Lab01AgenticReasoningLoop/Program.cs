using System.Diagnostics;
using System.Text.Json;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Pronative.Day03.Shared;

var config = Day03TrainingConfig.Load(args);
Day03Console.PrintAppStart();
Day03Console.PrintHeader(config, "Lab 01 - Agentic AI Reasoning Loop");

Console.WriteLine("This lab uses Microsoft Agent Framework directly:");
Console.WriteLine("- AIProjectClient");
Console.WriteLine("- AzureCliCredential");
Console.WriteLine("- AsAIAgent(...)");
Console.WriteLine("- AIAgent.AsBuilder().Use(...) for run middleware");
Console.WriteLine("- Function invocation middleware for tool governance");
Console.WriteLine("- IChatClient middleware through the Foundry clientFactory hook");
Console.WriteLine("- AIFunctionFactory.Create(...) for real tool/action invocation");
Console.WriteLine("- LoopAgent + DelegateLoopEvaluator for reflection and bounded retry");
Console.WriteLine("- AgentSession state bag as the local checkpoint boundary");
Console.WriteLine();

Console.Write("Enter an agentic readiness question, or press Enter for the default: ");
var prompt = Console.ReadLine();
if (string.IsNullOrWhiteSpace(prompt))
{
    prompt = $"""
    For batch {config.BatchId} and student {config.StudentId}, demonstrate the agentic reasoning loop.

    You must:
    1. State the business goal.
    2. Create a short milestone plan.
    3. Take an action by calling the get_batch_readiness_signal tool.
    4. Use the tool result as the observation.
    5. Reflect on whether the plan is ready for Lab 02.

    Return the answer using these headings exactly:
    Goal
    Plan
    Action
    Observation
    Reflection
    """;
}

var projectClient = new AIProjectClient(new Uri(config.ProjectEndpoint), new AzureCliCredential());

// Tools are created with AIFunctionFactory from static methods, enabling function governance middleware visibility
var planTool = AIFunctionFactory.Create(
    (Func<string, string>)AgenticReasoningTools.BuildMilestonePlan,
    name: "build_milestone_plan",
    description: "Create a short task breakdown and milestone graph for the requested agentic workflow goal.");

var readinessTool = AIFunctionFactory.Create(
    (Func<string, string, string>)AgenticReasoningTools.GetBatchReadinessSignal,
    name: "get_batch_readiness_signal",
    description: "Get structured ProNative AI training readiness signal for a batch and student. Inputs are batchId and studentId.");

var checkpointTool = AIFunctionFactory.Create(
    (Func<string, string, string, string>)AgenticReasoningTools.RecordCheckpoint,
    name: "record_reasoning_checkpoint",
    description: "Record a lightweight checkpoint for the current agentic reasoning loop stage. Inputs are batchId, studentId, and stage.");

// Foundry-backed agent with tools, instructions, and a clientFactory hook for IChatClient middleware
var baseAgent = projectClient.AsAIAgent(
    model: config.ModelDeployment,
    name: config.AgentName,
    description: "Day 3 Lab 01 agentic reasoning loop agent",
    instructions: """
    You are a ProNative AI agentic reasoning coach for Microsoft Agent Framework training.

    Your job is to demonstrate the loop:
    goal -> plan -> action -> observation -> reflection.

    You must use tools instead of inventing operational evidence:
    - build_milestone_plan for the planning step
    - get_batch_readiness_signal for the action/observation step
    - record_reasoning_checkpoint at the end of the reflection step

    When the user asks about batch or student readiness, you must call get_batch_readiness_signal.
    Do not invent readiness data.
    The Plan section must summarize the milestone plan returned by the tool.
    The Action section must mention the tool calls.
    The Observation section must summarize the readiness result.
    The Reflection section must explicitly state whether another iteration is required.
    If the response is missing one of the required headings, the loop evaluator may ask you to revise it.
    Keep the output clear enough for students to inspect during training.
    """,
    tools: [planTool, readinessTool, checkpointTool],
    clientFactory: BuildChatClientPipeline);

// Middleware pipeline: run logging, function governance (blocks destructive tools), auto-approval for safe tools
var governedAgent = baseAgent
    .AsBuilder()
    .Use(sharedFunc: AgentRunMiddleware)
    .Use(FunctionGovernanceMiddleware)
    .UseToolApproval(new ToolApprovalAgentOptions
    {
        AutoApprovalRules =
        [
            functionCall => new ValueTask<bool>(
                functionCall.Name is "build_milestone_plan" or "get_batch_readiness_signal" or "record_reasoning_checkpoint")
        ]
    })
    .Build();

// LoopAgent wraps the governed agent with a DelegateLoopEvaluator that checks for required sections (Goal, Plan, Action, Observation, Reflection)
var loopAgent = new LoopAgent(
    governedAgent,
    new DelegateLoopEvaluator(ReflectOnRequiredSectionsAsync),
    new LoopAgentOptions
    {
        MaxIterations = 2,
        NonStreamingReturnsLastResponseOnly = true,
        OnBehalfOfAuthorName = "reflection-evaluator"
    });

// AgentSession is the local checkpoint boundary; StateBag holds run-scoped metadata
var session = await loopAgent.CreateSessionAsync();
session.StateBag.SetValue("BatchId", config.BatchId);
session.StateBag.SetValue("StudentId", config.StudentId);
session.StateBag.SetValue(
    "CheckpointStore",
    "AgentSession.StateBag today; Dapr actor checkpoint in Lab 04 / deployed ACA path");

var run = await loopAgent.RunAsync(prompt, session);

Day03Console.PrintLabStart(1);
Console.WriteLine("Agent Framework Run Result");
Console.WriteLine("==========================");
Console.WriteLine(run.ToString());
Console.WriteLine();
Console.WriteLine("Session Checkpoint Boundary");
Console.WriteLine("===========================");
Console.WriteLine(session.StateBag.Serialize());
Day03Console.PrintLabEnd(1);

Day03Console.PrintAppEnd();

static IChatClient BuildChatClientPipeline(IChatClient innerClient)
{
    Console.WriteLine("[middleware:chatclient] installed on Foundry-backed IChatClient");
    return new ReasoningChatClientMiddleware(innerClient);
}

// Run middleware: logs inbound message count and records execution duration in session state bag
static async Task AgentRunMiddleware(
    IEnumerable<ChatMessage> messages,
    AgentSession? session,
    AgentRunOptions? options,
    Func<IEnumerable<ChatMessage>, AgentSession?, AgentRunOptions?, CancellationToken, Task> next,
    CancellationToken cancellationToken)
{
    var messageCount = messages.Count();
    var stopwatch = Stopwatch.StartNew();

    session?.StateBag.SetValue("RunMiddleware", "Applied: logging, session tagging, and checkpoint boundary");
    Console.WriteLine($"[middleware:run] inbound_messages={messageCount}");

    await next(messages, session, options, cancellationToken).ConfigureAwait(false);

    stopwatch.Stop();
    session?.StateBag.SetValue("LastRunDurationMs", stopwatch.ElapsedMilliseconds.ToString());
    Console.WriteLine($"[middleware:run] completed_ms={stopwatch.ElapsedMilliseconds}");
}

// Function governance middleware: blocks destructive tools (e.g., delete*) and logs all tool invocations
static async ValueTask<object?> FunctionGovernanceMiddleware(
    AIAgent agent,
    FunctionInvocationContext context,
    Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
    CancellationToken cancellationToken)
{
    Console.WriteLine($"[middleware:function] validating_tool={context.Function.Name}");

    if (context.Function.Name.Contains("delete", StringComparison.OrdinalIgnoreCase))
    {
        context.Terminate = true;
        return "Blocked by function governance middleware: destructive tools are not allowed in Lab 01.";
    }

    var result = await next(context, cancellationToken).ConfigureAwait(false);

    Console.WriteLine($"[middleware:function] completed_tool={context.Function.Name}");
    return result;
}

// Loop evaluator: checks if the response contains all required reasoning sections; continues with feedback if not
static ValueTask<LoopEvaluation> ReflectOnRequiredSectionsAsync(
    LoopContext context,
    CancellationToken cancellationToken)
{
    var transcript = context.LastResponse.ToString();

    var requiredSections = new[] { "Goal", "Plan", "Action", "Observation", "Reflection" };
    var missingSections = requiredSections
        .Where(section => !transcript.Contains(section, StringComparison.OrdinalIgnoreCase))
        .ToArray();

    if (missingSections.Length == 0)
    {
        return ValueTask.FromResult(LoopEvaluation.Stop());
    }

    var feedback = "Revise the response. Missing required sections: " + string.Join(", ", missingSections);
    return ValueTask.FromResult(LoopEvaluation.Continue(feedback));
}

// IChatClient middleware: logs request message count and response size/elapsed time around every LLM call
internal sealed class ReasoningChatClientMiddleware(IChatClient innerClient) : DelegatingChatClient(innerClient)
{
    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var materializedMessages = messages as ICollection<ChatMessage> ?? messages.ToArray();
        var stopwatch = Stopwatch.StartNew();

        Console.WriteLine(
            $"[middleware:chatclient] request_messages={materializedMessages.Count}; model={options?.ModelId ?? "(provider default)"}");

        var response = await base.GetResponseAsync(
            materializedMessages,
            options,
            cancellationToken).ConfigureAwait(false);

        stopwatch.Stop();
        Console.WriteLine(
            $"[middleware:chatclient] response_chars={response.ToString().Length}; elapsed_ms={stopwatch.ElapsedMilliseconds}");

        return response;
    }
}

// Static tools for the reasoning loop: milestone planning, readiness check, and checkpoint recording
internal static class AgenticReasoningTools
{
    public static string BuildMilestonePlan(string goal)
    {
        var normalizedGoal = string.IsNullOrWhiteSpace(goal)
            ? "Prepare the student for Day 3 Agentic AI reasoning"
            : goal.Trim();

        var plan = new
        {
            goal = normalizedGoal,
            milestoneGraph = new[]
            {
                new { id = "M1", name = "Clarify goal", dependsOn = Array.Empty<string>() },
                new { id = "M2", name = "Select action/tool", dependsOn = new[] { "M1" } },
                new { id = "M3", name = "Observe evidence", dependsOn = new[] { "M2" } },
                new { id = "M4", name = "Reflect and decide next step", dependsOn = new[] { "M3" } }
            },
            note = "This is a lightweight plan graph for Lab 01. Lab 02 upgrades this idea to Agent Framework Workflows with WorkflowBuilder, executors, and edges."
        };

        return JsonSerializer.Serialize(plan);
    }

    public static string GetBatchReadinessSignal(string batchId, string studentId)
    {
        var normalizedBatch = string.IsNullOrWhiteSpace(batchId) ? "AN-2607-101" : batchId.Trim();
        var normalizedStudent = string.IsNullOrWhiteSpace(studentId) ? "ST-2606-1000" : studentId.Trim();

        var result = new
        {
            batchId = normalizedBatch,
            studentId = normalizedStudent,
            readiness = "ready_with_checks",
            signals = new[]
            {
                new
                {
                    name = "foundry_project_access",
                    status = "check_required",
                    evidence = "Student must be able to authenticate with AzureCliCredential and access the shared Foundry project."
                },
                new
                {
                    name = "agent_framework_pipeline",
                    status = "ready",
                    evidence = "The lab wraps the Foundry agent with run middleware, function governance middleware, and tool approval."
                },
                new
                {
                    name = "tool_invocation",
                    status = "ready",
                    evidence = "The action step uses AIFunctionFactory-created tools and middleware-visible function invocation."
                }
            },
            nextAction = "Proceed to Lab 02 only after the live Foundry run succeeds and the tool result appears in the Observation section."
        };

        return JsonSerializer.Serialize(result);
    }

    public static string RecordCheckpoint(string batchId, string studentId, string stage)
    {
        var checkpoint = new
        {
            batchId = string.IsNullOrWhiteSpace(batchId) ? "AN-2607-101" : batchId.Trim(),
            studentId = string.IsNullOrWhiteSpace(studentId) ? "ST-2606-1000" : studentId.Trim(),
            stage = string.IsNullOrWhiteSpace(stage) ? "reflection" : stage.Trim(),
            checkpointKind = "local-agent-session",
            productionUpgrade = "Persist the same checkpoint contract with Dapr actor state in Azure Container Apps."
        };

        return JsonSerializer.Serialize(checkpoint);
    }
}
