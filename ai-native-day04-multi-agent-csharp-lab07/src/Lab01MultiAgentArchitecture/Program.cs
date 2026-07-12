#pragma warning disable MAAI001

using System.Text;
using System.Text.Json;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Pronative.MultiAgentTraining.Shared;

Day04Console.PrintAppStart();

DotEnvLoader.Load();
var config = Day04LabConfig.Load(args);

PrintHeader(config);

Console.WriteLine("Official Microsoft Agent Framework APIs used in this lab:");
Console.WriteLine("- AIProjectClient.AsAIAgent(...) for live Foundry-backed agent participants");
Console.WriteLine("- AgentWorkflowBuilder.CreateSequentialBuilderWith(...)");
Console.WriteLine("- AgentWorkflowBuilder.CreateConcurrentBuilderWith(...)");
Console.WriteLine("- AgentWorkflowBuilder.CreateHandoffBuilderWith(...)");
Console.WriteLine("- AgentWorkflowBuilder.CreateGroupChatBuilderWith(...)");
Console.WriteLine("- RoundRobinGroupChatManager for native group-style coordination");
Console.WriteLine("- InProcessExecution.RunStreamingAsync(...) for workflow execution and event visibility");
Console.WriteLine();

Console.Write("Enter a ProNative training operations request, or press Enter for the default: ");
var request = Console.ReadLine();
if (string.IsNullOrWhiteSpace(request))
{
    request = """
    ProNative wants to add an optional Friday evening Day 3 support clinic for batch AN-2607-101.
    Students are progressing unevenly on workflow agents, skill-driven development, and Azure AI Search grounding.
    Compare the delivery impact, learner value, Azure cost impact, and governance risk before recommending a decision.
    """;
}

var projectClient = new AIProjectClient(new Uri(config.ProjectEndpoint), new AzureCliCredential());
var agents = CreateAgents(projectClient, config);

var inputMessages = new List<ChatMessage>
{
    new(
        ChatRole.User,
        $"""
        BatchId: {config.BatchId}
        StudentId: {config.StudentId}
        Lab: Day 4 Lab 01 - Multi-Agent Architecture

        Request:
        {request.Trim()}

        Keep all responses concise. Use enterprise training delivery language.
        """)
};

var sequentialWorkflow = AgentWorkflowBuilder
    .CreateSequentialBuilderWith([
        agents.IntakeAnalyst,
        agents.DeliveryPlanner,
        agents.RiskReviewer,
        agents.FinalDecision
    ])
    .WithName("Day 4 Lab 01 - Sequential Coordination")
    .WithDescription("Pipeline coordination: each agent receives the previous agent output.")
    .WithChainOnlyAgentResponses(true)
    .WithIntermediateOutputFrom([agents.IntakeAnalyst, agents.DeliveryPlanner, agents.RiskReviewer])
    .WithOutputFrom([agents.FinalDecision])
    .Build();

await RunWorkflowAsync(
    "Sequential coordination",
    "The request moves through a fixed pipeline: intake -> plan -> risk review -> final decision.",
    sequentialWorkflow,
    inputMessages,
    $"day04-lab01-sequential-{Sanitize(config.StudentId)}");

var concurrentWorkflow = AgentWorkflowBuilder
    .CreateConcurrentBuilderWith([
        agents.LearnerValueSpecialist,
        agents.AzureCostSpecialist,
        agents.GovernanceSpecialist
    ])
    .WithName("Day 4 Lab 01 - Concurrent Coordination")
    .WithDescription("Fan-out coordination: independent specialists evaluate the same request in parallel.")
    .WithAggregator(branchOutputs =>
    {
        var merged = new List<ChatMessage>
        {
            new(
                ChatRole.Assistant,
                """
                Concurrent specialist findings follow. Compare the independent viewpoints before producing a final decision.
                """)
        };

        foreach (var branch in branchOutputs)
        {
            var lastMessage = branch.LastOrDefault(message => !string.IsNullOrWhiteSpace(message.Text));
            if (lastMessage is not null)
            {
                merged.Add(new ChatMessage(ChatRole.Assistant, lastMessage.Text));
            }
        }

        return merged;
    })
    .Build();

await RunWorkflowAsync(
    "Concurrent coordination",
    "The same request fans out to independent specialists, then the workflow aggregates their last outputs.",
    concurrentWorkflow,
    inputMessages,
    $"day04-lab01-concurrent-{Sanitize(config.StudentId)}");

var handoffWorkflow = AgentWorkflowBuilder
    .CreateHandoffBuilderWith(agents.TriageRouter)
    .WithHandoff(
        agents.TriageRouter,
        agents.DeliveryPlanner,
        "Use this handoff when the request needs schedule, trainer, learner-flow, or delivery planning analysis.")
    .WithHandoff(
        agents.TriageRouter,
        agents.AzureCostSpecialist,
        "Use this handoff when the request needs Azure cost, model usage, search service, or environment-cost analysis.")
    .WithHandoff(
        agents.TriageRouter,
        agents.GovernanceSpecialist,
        "Use this handoff when the request needs responsible AI, access, approval, safety, or governance analysis.")
    .WithHandoff(
        agents.DeliveryPlanner,
        agents.FinalDecision,
        "Use this handoff when delivery analysis is ready for the final recommendation.")
    .WithHandoff(
        agents.AzureCostSpecialist,
        agents.FinalDecision,
        "Use this handoff when cost analysis is ready for the final recommendation.")
    .WithHandoff(
        agents.GovernanceSpecialist,
        agents.FinalDecision,
        "Use this handoff when governance analysis is ready for the final recommendation.")
    .WithHandoffInstructions(
        """
        Select the best specialist by calling the appropriate handoff tool. Do not answer everything yourself.
        If the specialist has completed analysis, hand off to the final decision agent.
        The final decision agent should end with HANDOFF_COMPLETE.
        """)
    .EmitAgentResponseUpdateEvents(true)
    .EmitAgentResponseEvents(true)
    .WithTerminationCondition(messages =>
        messages.Any(message => message.Text.Contains("HANDOFF_COMPLETE", StringComparison.OrdinalIgnoreCase)))
    .Build();

await RunWorkflowAsync(
    "Handoff coordination",
    "The router agent chooses which specialist should continue by using native handoff tools.",
    handoffWorkflow,
    inputMessages,
    $"day04-lab01-handoff-{Sanitize(config.StudentId)}");

var groupChatWorkflow = AgentWorkflowBuilder
    .CreateGroupChatBuilderWith(participants =>
        new RoundRobinGroupChatManager(
            participants,
            shouldTerminateFunc: (_, history, _) =>
            {
                var assistantTurns = history.Count(message => message.Role == ChatRole.Assistant);
                var explicitCompletion = history.Any(message =>
                    message.Text.Contains("GROUP_COMPLETE", StringComparison.OrdinalIgnoreCase));

                return new ValueTask<bool>(assistantTurns >= 4 || explicitCompletion);
            })
        {
            MaximumIterationCount = 4
        })
    .AddParticipants([
        agents.LearnerValueSpecialist,
        agents.AzureCostSpecialist,
        agents.GovernanceSpecialist,
        agents.FinalDecision
    ])
    .WithName("Day 4 Lab 01 - Group-Style Coordination")
    .WithDescription("Shared conversation coordination using RoundRobinGroupChatManager.")
    .Build();

Day04Console.PrintLabStart(1);

await RunWorkflowAsync(
    "Group-style coordination",
    "Agents collaborate in a shared conversation under the native RoundRobinGroupChatManager.",
    groupChatWorkflow,
    inputMessages,
    $"day04-lab01-groupchat-{Sanitize(config.StudentId)}");

Day04Console.PrintLabEnd(1);

Console.WriteLine();
Console.WriteLine("Lab complete.");
Console.WriteLine("Compare the four outputs and ask students which topology best fits each enterprise scenario.");

Day04Console.PrintAppEnd();

static Day04Agents CreateAgents(AIProjectClient projectClient, Day04LabConfig config)
{
    var suffix = Sanitize(config.StudentId);

    var intakeAnalyst = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l1-intake-{suffix}",
        description: "Analyzes the request and extracts the business intent.",
        instructions:
            """
            You are the Intake Analyst Agent for ProNative AI training operations.

            Identify:
            - request intent
            - affected days/modules
            - impacted students/trainers
            - constraints
            - missing information

            Keep the response compact and pass clean context to the next agent.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var deliveryPlanner = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l1-planner-{suffix}",
        description: "Plans the delivery and learner-flow impact.",
        instructions:
            """
            You are the Delivery Planner Agent.

            Decide how the request affects:
            - trainer schedule
            - learner flow
            - lab readiness
            - continuity into Day 4 and live projects

            Produce practical delivery actions and explicit dependencies.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var riskReviewer = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l1-risk-{suffix}",
        description: "Reviews delivery, cost, access, and governance risk.",
        instructions:
            """
            You are the Risk Reviewer Agent.

            Review the proposed training change for:
            - learner experience risk
            - Azure cost risk
            - access/readiness risk
            - responsible AI/security/governance risk
            - approval requirement

            Return blockers, mitigations, and a readiness call.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var learnerValueSpecialist = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l1-learner-{suffix}",
        description: "Evaluates learner value and curriculum continuity.",
        instructions:
            """
            You are the Learner Value Specialist.

            Focus only on learner value, curriculum continuity, and concept clarity.
            Recommend whether the change helps students succeed in agentic workflows.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var azureCostSpecialist = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l1-cost-{suffix}",
        description: "Evaluates Azure resource and model usage cost impact.",
        instructions:
            """
            You are the Azure Cost Specialist.

            Focus only on Azure cost and operational impact:
            - model calls
            - Azure AI Search
            - Foundry resources
            - student-specific resource groups
            - weekend shutdown/deprovisioning needs

            Keep the recommendation practical for batch AN-2607-101.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var governanceSpecialist = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l1-gov-{suffix}",
        description: "Evaluates responsible AI, access, approval, and governance.",
        instructions:
            """
            You are the Governance Specialist.

            Focus only on:
            - access boundaries
            - responsible AI
            - approval needs
            - telemetry/evidence
            - trainer-only versus student-permitted action

            Be specific about human approval and evidence.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var triageRouter = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l1-router-{suffix}",
        description: "Routes the request to the best specialist using native handoff tools.",
        instructions:
            """
            You are the Triage Router Agent.

            Your job is to choose the best next specialist. Do not solve the full request yourself.
            Use the available handoff tool to route to exactly one specialist:
            - Delivery Planner for schedule, trainer, and learner-flow work
            - Azure Cost Specialist for cost, model, Search, and environment impact
            - Governance Specialist for access, approval, safety, and compliance impact

            After a specialist has responded, the workflow can hand off to the Final Decision Agent.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var finalDecision = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l1-final-{suffix}",
        description: "Produces the final recommendation.",
        instructions:
            """
            You are the Final Decision Agent.

            Produce a concise enterprise recommendation with:
            Decision
            Why
            Actions
            Risks
            Approval
            Evidence

            For handoff workflows, end the final answer with HANDOFF_COMPLETE.
            For group-chat workflows, end the final answer with GROUP_COMPLETE.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    return new Day04Agents(
        intakeAnalyst,
        deliveryPlanner,
        riskReviewer,
        learnerValueSpecialist,
        azureCostSpecialist,
        governanceSpecialist,
        triageRouter,
        finalDecision);
}

static async Task RunWorkflowAsync(
    string title,
    string explanation,
    Workflow workflow,
    List<ChatMessage> inputMessages,
    string sessionId)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 88));
    Console.WriteLine(title);
    Console.WriteLine(new string('=', 88));
    Console.WriteLine(explanation);
    Console.WriteLine();

    var finalOutput = new StringBuilder();

    await using var run = await InProcessExecution.RunStreamingAsync(
        workflow,
        inputMessages,
        sessionId: sessionId,
        cancellationToken: CancellationToken.None);

    await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

    await foreach (var workflowEvent in run.WatchStreamAsync())
    {
        switch (workflowEvent)
        {
            case WorkflowStartedEvent:
                Console.WriteLine("[workflow:event] started");
                break;

            case AgentResponseUpdateEvent update:
                var updateText = update.Update.Text;
                if (!string.IsNullOrWhiteSpace(updateText))
                {
                    Console.Write(updateText);
                    finalOutput.Append(updateText);
                }
                break;

            case AgentResponseEvent response:
                Console.WriteLine();
                Console.WriteLine($"[workflow:event] agent_response={response.ExecutorId}");
                var responseText = OneLine(response.Response.ToString());
                Console.WriteLine(responseText);
                finalOutput.AppendLine(responseText);
                break;

            case WorkflowOutputEvent output:
                Console.WriteLine();
                Console.WriteLine($"[workflow:event] output intermediate={output.IsIntermediate()}");
                var outputText = DescribeOutput(output.Data);
                Console.WriteLine(outputText);
                finalOutput.AppendLine(outputText);
                break;

            case ExecutorFailedEvent failed:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Executor '{failed.ExecutorId}' failed.");
                Console.Error.WriteLine(failed.Data?.ToString() ?? "No error data returned.");
                Console.ResetColor();
                return;

            case WorkflowErrorEvent error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Workflow failed.");
                Console.Error.WriteLine(error.Exception?.ToString() ?? "Unknown workflow error.");
                Console.ResetColor();
                return;
        }
    }

    Console.WriteLine();
    Console.WriteLine("[summary] Final captured output:");
    Console.WriteLine(string.IsNullOrWhiteSpace(finalOutput.ToString())
        ? "(No terminal output captured. Review agent_response events above.)"
        : finalOutput.ToString().Trim());
}

static string DescribeOutput(object? data) =>
    data switch
    {
        null => "(no output data)",
        List<ChatMessage> messages => string.Join(
            Environment.NewLine,
            messages.Select(message => $"{message.Role}: {message.Text}")),
        IEnumerable<ChatMessage> messages => string.Join(
            Environment.NewLine,
            messages.Select(message => $"{message.Role}: {message.Text}")),
        ChatMessage message => $"{message.Role}: {message.Text}",
        ChatResponse response => response.Text,
        _ => data.ToString() ?? "(unprintable output)"
    };

static string OneLine(string value)
{
    var normalized = value
        .ReplaceLineEndings(" ")
        .Replace("  ", " ", StringComparison.Ordinal);

    return normalized.Length <= 240
        ? normalized
        : normalized[..240] + "...";
}

static string Sanitize(string value) =>
    new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

static void PrintHeader(Day04LabConfig config)
{
    Console.WriteLine("ProNative AI-Native Fullstack Engineering - Day 4");
    Console.WriteLine("Lab 01 - Multi-Agent Architecture");
    Console.WriteLine($"Batch: {config.BatchId} | Student: {config.StudentId}");
    Console.WriteLine($"Foundry project endpoint: {config.ProjectEndpoint}");
    Console.WriteLine($"Model deployment: {config.ModelDeployment}");
    Console.WriteLine();
}

internal sealed record Day04Agents(
    AIAgent IntakeAnalyst,
    AIAgent DeliveryPlanner,
    AIAgent RiskReviewer,
    AIAgent LearnerValueSpecialist,
    AIAgent AzureCostSpecialist,
    AIAgent GovernanceSpecialist,
    AIAgent TriageRouter,
    AIAgent FinalDecision);

internal sealed record Day04LabConfig
{
    public string BatchId { get; init; } = "AN-2607-101";
    public string StudentId { get; init; } = "ST-2606-1000";
    public string ProjectEndpoint { get; init; } = "https://proj-an2607101-default-resource.services.ai.azure.com/api/projects/proj-an2607101-default";
    public string ModelDeployment { get; init; } = "gpt-5-mini";

    public static Day04LabConfig Load(string[] args)
    {
        var filePath = args.FirstOrDefault(arg => arg.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            ?? "appsettings.json";

        var fromFile = File.Exists(filePath)
            ? JsonSerializer.Deserialize<Day04LabConfig>(
                File.ReadAllText(filePath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : new Day04LabConfig();

        return (fromFile ?? new Day04LabConfig()).WithEnvironmentOverrides();
    }

    private Day04LabConfig WithEnvironmentOverrides() => this with
    {
        BatchId = Environment.GetEnvironmentVariable("BATCH_ID") ?? BatchId,
        StudentId = Environment.GetEnvironmentVariable("STUDENT_ID") ?? StudentId,
        ProjectEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT")
            ?? Environment.GetEnvironmentVariable("PROJECT_ENDPOINT")
            ?? ProjectEndpoint,
        ModelDeployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_DEPLOYMENT") ?? ModelDeployment
    };
}

internal sealed class ModelIdEnforcer : DelegatingChatClient
{
    private readonly string _modelId;

    public ModelIdEnforcer(IChatClient innerClient, string modelId) : base(innerClient)
    {
        _modelId = modelId;
    }

    public override Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new ChatOptions();
        options.ModelId ??= _modelId;
        return base.GetResponseAsync(chatMessages, options, cancellationToken);
    }

    public override IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new ChatOptions();
        options.ModelId ??= _modelId;
        return base.GetStreamingResponseAsync(chatMessages, options, cancellationToken);
    }
}
