#pragma warning disable MAAI001

using System.Text;
using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Pronative.Day03.Shared;

var config = Day03TrainingConfig.Load(args);
var labConfig = WorkflowAgentLabConfig.Load(config);

Day03Console.PrintAppStart();
Day03Console.PrintHeader(config, "Lab 07 - Workflow Agent");

Console.WriteLine("This lab uses Microsoft Agent Framework workflow-agent capabilities directly:");
Console.WriteLine("- AIProjectClient.AsAIAgent(...) for each specialist agent");
Console.WriteLine("- AgentWorkflowBuilder.CreateSequentialBuilderWith(...) for the agent pipeline");
Console.WriteLine("- SequentialWorkflowBuilder.WithChainOnlyAgentResponses(true)");
Console.WriteLine("- OrchestrationBuilderBase.WithIntermediateOutputFrom(...) and WithOutputFrom(...)");
Console.WriteLine("- InProcessExecution.RunStreamingAsync(...) for live workflow execution");
Console.WriteLine("- AgentResponseUpdateEvent, AgentResponseEvent, and WorkflowOutputEvent for visibility");
Console.WriteLine("- ChatClientStructuredOutputExtensions.GetResponseAsync<T>(...) for the final typed result");
Console.WriteLine();

Console.Write("Enter a training change request, or press Enter for the default: ");
var request = Console.ReadLine();
if (string.IsNullOrWhiteSpace(request))
{
    request = """
    ProNative wants to add an optional evening support clinic before Day 4 for students who struggled with
    Agent Framework skills, workflow state, and Azure AI Search grounding. Evaluate the intent, create an
    execution plan, review delivery risk, and produce a final structured action recommendation.
    """;
}

var projectClient = new AIProjectClient(new Uri(config.ProjectEndpoint), new AzureCliCredential());

// Step 1 agent: Intent Analyst - reads the training change request and extracts category, goal, constraints, and gaps
var intentAnalystAgent = projectClient.AsAIAgent(
    model: config.ModelDeployment,
    name: $"day03-lab07-intent-{Sanitize(config.StudentId)}",
    description: "Workflow agent step 1: intent analyst",
    instructions:
        """
        You are the Intent Analyst Agent in a ProNative AI training workflow.

        Read the incoming training change request and produce:
        - intent category
        - business goal
        - affected learners or trainers
        - known constraints
        - missing information

        Be concise and pass a clean analysis to the planning agent.
        """);

// Step 2 agent: Planning Agent - creates proposed steps, owners, resources, and fallback options from the intent analysis
var planAgent = projectClient.AsAIAgent(
    model: config.ModelDeployment,
    name: $"day03-lab07-plan-{Sanitize(config.StudentId)}",
    description: "Workflow agent step 2: implementation planner",
    instructions:
        """
        You are the Planning Agent in a ProNative AI training workflow.

        Use the prior agent's intent analysis as your input. Produce:
        - proposed steps
        - owner for each step
        - Azure/Foundry resources touched
        - evidence that must be captured
        - fallback option

        Do not ignore constraints from the intent analysis.
        """);

// Step 3 agent: Risk Reviewer - identifies learner impact, cost, timing, governance, and security risks
var riskReviewerAgent = projectClient.AsAIAgent(
    model: config.ModelDeployment,
    name: $"day03-lab07-risk-{Sanitize(config.StudentId)}",
    description: "Workflow agent step 3: risk reviewer",
    instructions:
        """
        You are the Risk Reviewer Agent in a ProNative AI training workflow.

        Review the proposed plan and identify:
        - learner impact risk
        - Azure cost or access risk
        - delivery timing risk
        - governance/security risk
        - approval or human-in-the-loop requirement

        Return explicit blockers and mitigations. If the request is safe to proceed, say so with conditions.
        """);

// Step 4 agent: Finalizer - produces enterprise-ready recommendation with Decision, Rationale, Action Plan, Risks, Approval, Evidence
var finalizerAgent = projectClient.AsAIAgent(
    model: config.ModelDeployment,
    name: $"day03-lab07-final-{Sanitize(config.StudentId)}",
    description: "Workflow agent step 4: finalizer",
    instructions:
        """
        You are the Finalizer Agent in a ProNative AI training workflow.

        Use the full upstream analysis, plan, and risk review to produce a final recommendation.
        Your response must include these headings:
        Decision
        Rationale
        Action Plan
        Risks
        Approval
        Evidence To Capture

        Keep the response enterprise-ready and grounded in the request.
        """);

AIAgent[] workflowAgents =
[
    intentAnalystAgent,
    planAgent,
    riskReviewerAgent,
    finalizerAgent
];

// Sequential workflow-agent pipeline: agents run in order with intermediate output visible and final output from the last agent
var workflow = AgentWorkflowBuilder
    .CreateSequentialBuilderWith(workflowAgents)
    .WithName("Day 3 Lab 07 - Workflow Agent")
    .WithDescription("Sequential workflow-agent pipeline for a structured ProNative training change request.")
    .WithChainOnlyAgentResponses(true)
    .WithIntermediateOutputFrom([intentAnalystAgent, planAgent, riskReviewerAgent])
    .WithOutputFrom([finalizerAgent])
    .Build();

var inputMessages = new List<ChatMessage>
{
    new(
        ChatRole.User,
        $"""
        BatchId: {config.BatchId}
        StudentId: {config.StudentId}
        Workflow: Day 3 Lab 07 - Workflow Agent

        Training change request:
        {request.Trim()}

        Run the request through the workflow-agent pipeline:
        1. Intent analysis
        2. Implementation planning
        3. Risk review
        4. Final action recommendation
        """)
};

var transcript = new StringBuilder();
transcript.AppendLine("=== Input Request ===");
transcript.AppendLine(request.Trim());
transcript.AppendLine();

// Streaming workflow execution emits AgentResponseUpdateEvent (tokens), AgentResponseEvent (per-agent), and WorkflowOutputEvent
await using var run = await InProcessExecution.RunStreamingAsync(
    workflow,
    inputMessages,
    sessionId: $"day03-lab07-{config.StudentId}",
    cancellationToken: CancellationToken.None);

// Consume workflow events: live token streaming, per-agent responses, intermediate/terminal output, and error handling
await foreach (var workflowEvent in run.WatchStreamAsync())
{
    switch (workflowEvent)
    {
        case WorkflowStartedEvent started:
            Console.WriteLine("[workflow:event] started");
            transcript.AppendLine("=== Workflow Started ===");
            transcript.AppendLine(started.ToString());
            break;

        case AgentResponseUpdateEvent update:
            var updateText = update.Update.Text;
            if (!string.IsNullOrWhiteSpace(updateText))
            {
                Console.Write(updateText);
                transcript.Append(updateText);
            }
            break;

        case AgentResponseEvent response:
            Console.WriteLine();
            Console.WriteLine($"[workflow:event] agent_response={response.ExecutorId}");
            Console.WriteLine(response.Response.ToString());
            transcript.AppendLine();
            transcript.AppendLine($"=== Agent Response: {response.ExecutorId} ===");
            transcript.AppendLine(response.Response.ToString());
            break;

        case WorkflowOutputEvent output:
            Console.WriteLine();
            Console.WriteLine($"[workflow:event] output intermediate={output.IsIntermediate()}");
            var outputText = DescribeWorkflowOutput(output.Data);
            Console.WriteLine(outputText);
            transcript.AppendLine();
            transcript.AppendLine(output.IsIntermediate()
                ? "=== Intermediate Workflow Output ==="
                : "=== Terminal Workflow Output ===");
            transcript.AppendLine(outputText);
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

Day03Console.PrintLabStart(7);
Console.WriteLine("Structured Output Normalization");
Console.WriteLine("===============================");

var structuredResult = await BuildStructuredResultAsync(config, labConfig, request.Trim(), transcript.ToString());

Console.WriteLine(ToJson(structuredResult));
Day03Console.PrintLabEnd(7);

Directory.CreateDirectory(labConfig.ArtifactsDirectory);
var artifactPath = Path.Combine(labConfig.ArtifactsDirectory, "day03-lab07-workflow-agent-result.json");
await File.WriteAllTextAsync(
    artifactPath,
    ToJson(new WorkflowAgentArtifact(
        config.BatchId,
        config.StudentId,
        request.Trim(),
        transcript.ToString(),
        structuredResult,
        DateTimeOffset.UtcNow)),
    CancellationToken.None);

Console.WriteLine();
Console.WriteLine($"Evidence artifact: {artifactPath}");

Day03Console.PrintAppEnd();

static async Task<WorkflowAgentStructuredResult> BuildStructuredResultAsync(
    Day03TrainingConfig config,
    WorkflowAgentLabConfig labConfig,
    string originalRequest,
    string workflowTranscript)
{
    // Structured output normalization with GetResponseAsync<T>: converts the raw transcript into a typed schema
    using IChatClient chatClient = new AzureOpenAIClient(labConfig.AzureOpenAiEndpoint, new AzureCliCredential())
        .GetChatClient(config.ModelDeployment)
        .AsIChatClient();

    var prompt =
        $"""
        Convert the workflow-agent transcript into the requested JSON schema.

        BatchId: {config.BatchId}
        StudentId: {config.StudentId}

        Original request:
        {originalRequest}

        Workflow transcript:
        {workflowTranscript}

        Requirements:
        - terminalStatus must be one of: approved, approved_with_conditions, needs_approval, blocked.
        - agentSequence must include the four agents in order.
        - nextAction must be specific and suitable for a trainer.
        - risks and evidenceToCapture must be arrays.
        """;

    var response = await chatClient.GetResponseAsync<WorkflowAgentStructuredResult>(
        prompt,
        options: new ChatOptions(),
        useJsonSchemaResponseFormat: true,
        cancellationToken: CancellationToken.None);

    if (response.TryGetResult(out var result) && result is not null)
    {
        return result;
    }

    throw new InvalidOperationException(
        "The model did not return parseable structured output for WorkflowAgentStructuredResult. " +
        $"Raw structured-output response: {response.Text}");
}

static string DescribeWorkflowOutput(object? data)
{
    return data switch
    {
        null => "(null workflow output)",
        List<ChatMessage> messages => string.Join(
            Environment.NewLine,
            messages.Select(message => $"{message.Role}: {message.Text}")),
        IEnumerable<ChatMessage> messages => string.Join(
            Environment.NewLine,
            messages.Select(message => $"{message.Role}: {message.Text}")),
        ChatMessage message => $"{message.Role}: {message.Text}",
        _ => data.ToString() ?? string.Empty
    };
}

static string Sanitize(string value)
{
    var safe = new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
    return string.IsNullOrWhiteSpace(safe) ? "student" : safe;
}

static string ToJson(object value) => JsonSerializer.Serialize(
    value,
    new JsonSerializerOptions { WriteIndented = true });

internal sealed record WorkflowAgentLabConfig(Uri AzureOpenAiEndpoint, string ArtifactsDirectory)
{
    public static WorkflowAgentLabConfig Load(Day03TrainingConfig config)
    {
        var endpoint =
            Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
            ?? Environment.GetEnvironmentVariable("AZURE_AI_FOUNDRY_OPENAI_ENDPOINT")
            ?? "https://proj-an2607101-default-resource.openai.azure.com/";

        if (endpoint.EndsWith("/openai/v1", StringComparison.OrdinalIgnoreCase))
        {
            endpoint = endpoint[..^"/openai/v1".Length];
        }

        return new WorkflowAgentLabConfig(
            new Uri(endpoint.TrimEnd('/') + "/"),
            Path.Combine(AppContext.BaseDirectory, "artifacts", config.StudentId));
    }
}

public sealed class WorkflowAgentStructuredResult
{
    public string TerminalStatus { get; set; } = string.Empty;
    public string IntentSummary { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public string NextAction { get; set; } = string.Empty;
    public string ApprovalRequired { get; set; } = string.Empty;
    public string[] AgentSequence { get; set; } = [];
    public string[] ActionPlan { get; set; } = [];
    public string[] Risks { get; set; } = [];
    public string[] EvidenceToCapture { get; set; } = [];
}

internal sealed record WorkflowAgentArtifact(
    string BatchId,
    string StudentId,
    string OriginalRequest,
    string WorkflowTranscript,
    WorkflowAgentStructuredResult StructuredResult,
    DateTimeOffset CompletedAtUtc);
