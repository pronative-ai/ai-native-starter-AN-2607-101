#pragma warning disable MAAI001
#pragma warning disable SCME0002

using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.Json;
using Azure.AI.Projects;
using Azure.Core;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Specialized.Magentic;
using Microsoft.Extensions.AI;
using Pronative.MultiAgentTraining.Shared;

Day04Console.PrintAppStart();

DotEnvLoader.Load();
var config = Day04MagenticConfig.Load(args);

PrintHeader(config);

Console.WriteLine("Official Microsoft Agent Framework APIs used in this lab:");
Console.WriteLine("- AIProjectClient.AsAIAgent(...) for live Foundry-backed manager and worker agents");
Console.WriteLine("- AgentWorkflowBuilder.CreateMagenticBuilderWith(...)");
Console.WriteLine("- MagenticWorkflowBuilder.AddParticipants(...)");
Console.WriteLine("- MagenticWorkflowBuilder.WithMaxRounds(...), WithMaxResets(...), WithMaxStalls(...)");
Console.WriteLine("- MagenticWorkflowBuilder.RequirePlanSignoff(...)");
Console.WriteLine("- InProcessExecution.RunStreamingAsync(...) for live Magentic workflow execution");
Console.WriteLine("- MagenticPlanCreatedEvent, MagenticReplannedEvent, and MagenticProgressLedgerUpdatedEvent");
Console.WriteLine();

Console.Write("Enter a complex ProNative delivery goal, or press Enter for the default: ");
var goal = Console.ReadLine();
if (string.IsNullOrWhiteSpace(goal))
{
    goal = """
    Prepare a Day 4 readiness plan for batch AN-2607-101.
    The plan must help students move from workflow agents to multi-agent systems, include protocol decisions,
    identify AgentGateway readiness, and produce acceptance checks before Day 5 operations/governance.
    """;
}

var projectClient = new AIProjectClient(
    new Uri(config.ProjectEndpoint),
    new FoundryTokenProvider(new AzureCliCredential()));
var team = CreateMagenticTeam(projectClient, config);

var workflow = AgentWorkflowBuilder
    .CreateMagenticBuilderWith(team.Manager)
    .AddParticipants([
        team.CurriculumWorker,
        team.ProtocolWorker,
        team.GatewayWorker,
        team.ValidationWorker
    ])
    .WithMaxRounds(config.MaxRounds)
    .WithMaxResets(config.MaxResets)
    .WithMaxStalls(config.MaxStalls)
    .RequirePlanSignoff(config.RequirePlanSignoff)
    .Build();

var inputMessages = new List<ChatMessage>
{
    new(
        ChatRole.User,
        $"""
        BatchId: {config.BatchId}
        StudentId: {config.StudentId}
        Lab: Day 4 Lab 02 - Magentic-Style Coordinator-Worker Orchestration

        Goal:
        {goal.Trim()}

        Produce a coordinated enterprise training operations plan.
        The manager must plan, assign specialist work, track progress, and decide when the request is satisfied.
        The final answer must include:
        - execution plan
        - worker responsibilities
        - risks and mitigations
        - acceptance checks
        - evidence to capture
        """)
};

Day04Console.PrintLabStart(2);

await RunMagenticWorkflowAsync(workflow, inputMessages, $"day04-lab02-magentic-{Sanitize(config.StudentId)}");

Day04Console.PrintLabEnd(2);

Console.WriteLine();
Console.WriteLine("Lab complete.");
Console.WriteLine("Trainer prompt: ask students what the manager owned versus what the worker agents owned.");

Day04Console.PrintAppEnd();

static Day04MagenticTeam CreateMagenticTeam(AIProjectClient projectClient, Day04MagenticConfig config)
{
    var suffix = Sanitize(config.StudentId);

    var manager = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l2-manager-{suffix}",
        description: "Magentic manager agent that plans, coordinates workers, tracks progress, and decides completion.",
        instructions:
            """
            You are the Magentic Manager Agent for ProNative AI training operations.

            Your responsibilities:
            - understand the goal
            - create a compact task plan
            - select the best worker agent for each step
            - track progress using the worker outputs
            - re-plan when evidence is missing or work stalls
            - produce a final enterprise-ready recommendation when the request is satisfied

            Do not do every specialist task yourself. Use the worker agents.
            Keep the final output concise and actionable.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var curriculumWorker = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l2-curriculum-{suffix}",
        description: "Curriculum worker for learner flow, day alignment, and training continuity.",
        instructions:
            """
            You are the Curriculum Worker Agent.

            Focus on:
            - day-level learning flow
            - concept sequencing
            - learner readiness
            - continuity from Day 3 into Day 4 and Day 5
            - how the output supports later live project days

            Return concrete curriculum actions and avoid Azure infrastructure detail unless needed.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var protocolWorker = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l2-protocol-{suffix}",
        description: "Protocol worker for MCP, UTCP, A2A, AG-UI, and A2UI boundaries.",
        instructions:
            """
            You are the Protocol Worker Agent.

            Focus on:
            - A2A discovery and task/message boundary
            - MCP tool-server boundary
            - UTCP direct API tool-calling boundary
            - AG-UI / A2UI agent-to-user boundary
            - where each protocol should appear in Day 4

            Return protocol choices, rationale, and teaching examples.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var gatewayWorker = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l2-gateway-{suffix}",
        description: "AgentGateway worker for routing, observability, rate limiting, and cost/token control.",
        instructions:
            """
            You are the AgentGateway Worker Agent.

            Focus on:
            - gateway route shape
            - model/tool/backend routing
            - rate limiting
            - cost and token attribution
            - trace correlation
            - Azure Container Apps and Azure Monitor readiness

            Return a practical gateway readiness slice for training.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    var validationWorker = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: $"d4l2-validation-{suffix}",
        description: "Validation worker for acceptance checks, evidence, and delivery risk.",
        instructions:
            """
            You are the Validation Worker Agent.

            Focus on:
            - acceptance checks
            - evidence to capture
            - trainer validation steps
            - risk and mitigation
            - what should block delivery

            Return crisp go/no-go criteria.
            """,
        clientFactory: inner => new ModelIdEnforcer(inner, config.ModelDeployment));

    return new Day04MagenticTeam(manager, curriculumWorker, protocolWorker, gatewayWorker, validationWorker);
}

static async Task RunMagenticWorkflowAsync(
    Workflow workflow,
    List<ChatMessage> inputMessages,
    string sessionId)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 88));
    Console.WriteLine("Native Magentic workflow run");
    Console.WriteLine(new string('=', 88));
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

            case MagenticPlanCreatedEvent plan:
                Console.WriteLine();
                Console.WriteLine("[magentic:event] plan_created");
                Console.WriteLine(DescribeOutput(plan.FullTaskLedger));
                if (plan.Data is MagenticPlanReviewRequest reviewRequest1)
                {
                    Console.WriteLine("Automatically signing off plan...");
                    var approval = reviewRequest1.Approve();
                    await run.TrySendMessageAsync(approval);
                }
                break;

            case MagenticReplannedEvent replan:
                Console.WriteLine();
                Console.WriteLine("[magentic:event] replanned");
                Console.WriteLine(DescribeOutput(replan.FullTaskLedger));
                if (replan.Data is MagenticPlanReviewRequest reviewRequest2)
                {
                    Console.WriteLine("Automatically signing off replan...");
                    var approval = reviewRequest2.Approve();
                    await run.TrySendMessageAsync(approval);
                }
                break;

            case RequestInfoEvent requestInfo:
                if (requestInfo.Request.TryGetDataAs<MagenticPlanReviewRequest>(out var reviewRequest))
                {
                    Console.WriteLine("Automatically signing off plan/replan...");
                    var approval = reviewRequest.Approve();
                    var response = requestInfo.Request.CreateResponse(approval);
                    await run.TrySendMessageAsync(response);
                }
                break;

            case MagenticProgressLedgerUpdatedEvent progress:
                Console.WriteLine();
                Console.WriteLine("[magentic:event] progress_ledger_updated");
                Console.WriteLine(ToJson(progress.ProgressLedger));
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
        ? "(No terminal output captured. Review Magentic and agent_response events above.)"
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

static string ToJson<T>(T value) =>
    JsonSerializer.Serialize(
        value,
        new JsonSerializerOptions
        {
            WriteIndented = true
        });

static string OneLine(string value)
{
    var normalized = value
        .ReplaceLineEndings(" ")
        .Replace("  ", " ", StringComparison.Ordinal);

    return normalized.Length <= 260
        ? normalized
        : normalized[..260] + "...";
}

static string Sanitize(string value) =>
    new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

static void PrintHeader(Day04MagenticConfig config)
{
    Console.WriteLine("ProNative AI-Native Fullstack Engineering - Day 4");
    Console.WriteLine("Lab 02 - Magentic-Style Coordinator-Worker Orchestration");
    Console.WriteLine($"Batch: {config.BatchId} | Student: {config.StudentId}");
    Console.WriteLine($"Foundry project endpoint: {config.ProjectEndpoint}");
    Console.WriteLine($"Model deployment: {config.ModelDeployment}");
    Console.WriteLine($"Require plan signoff: {config.RequirePlanSignoff}");
    Console.WriteLine($"Max rounds: {config.MaxRounds} | Max resets: {config.MaxResets} | Max stalls: {config.MaxStalls}");
    Console.WriteLine();
}

internal sealed record Day04MagenticTeam(
    AIAgent Manager,
    AIAgent CurriculumWorker,
    AIAgent ProtocolWorker,
    AIAgent GatewayWorker,
    AIAgent ValidationWorker);

internal sealed record Day04MagenticConfig
{
    public string BatchId { get; init; } = "AN-2607-101";
    public string StudentId { get; init; } = "ST-2606-1000";
    public string ProjectEndpoint { get; init; } = "https://proj-an2607101-default-resource.services.ai.azure.com/api/projects/proj-an2607101-default";
    public string ModelDeployment { get; init; } = "gpt-5-mini";
    public bool RequirePlanSignoff { get; init; }
    public int? MaxRounds { get; init; } = 4;
    public int? MaxResets { get; init; } = 1;
    public int MaxStalls { get; init; } = 1;

    public static Day04MagenticConfig Load(string[] args)
    {
        var filePath = args.FirstOrDefault(arg => arg.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            ?? "appsettings.json";

        var fromFile = File.Exists(filePath)
            ? JsonSerializer.Deserialize<Day04MagenticConfig>(
                File.ReadAllText(filePath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : new Day04MagenticConfig();

        return (fromFile ?? new Day04MagenticConfig()).WithEnvironmentOverrides();
    }

    private Day04MagenticConfig WithEnvironmentOverrides() => this with
    {
        BatchId = Environment.GetEnvironmentVariable("BATCH_ID") ?? BatchId,
        StudentId = Environment.GetEnvironmentVariable("STUDENT_ID") ?? StudentId,
        ProjectEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT")
            ?? Environment.GetEnvironmentVariable("PROJECT_ENDPOINT")
            ?? ProjectEndpoint,
        ModelDeployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_DEPLOYMENT") ?? ModelDeployment,
        RequirePlanSignoff = TryGetBool("MAGENTIC_REQUIRE_PLAN_SIGNOFF", RequirePlanSignoff),
        MaxRounds = TryGetNullableInt("MAGENTIC_MAX_ROUNDS", MaxRounds),
        MaxResets = TryGetNullableInt("MAGENTIC_MAX_RESETS", MaxResets),
        MaxStalls = TryGetInt("MAGENTIC_MAX_STALLS", MaxStalls)
    };

    private static bool TryGetBool(string name, bool fallback) =>
        bool.TryParse(Environment.GetEnvironmentVariable(name), out var value) ? value : fallback;

    private static int TryGetInt(string name, int fallback) =>
        int.TryParse(Environment.GetEnvironmentVariable(name), out var value) ? value : fallback;

    private static int? TryGetNullableInt(string name, int? fallback)
    {
        var raw = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        if (raw.Equals("null", StringComparison.OrdinalIgnoreCase) ||
            raw.Equals("unlimited", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return int.TryParse(raw, out var value) ? value : fallback;
    }
}

internal sealed class FoundryTokenProvider : AuthenticationTokenProvider
{
    private readonly TokenCredential _credential;
    private static readonly string[] s_scopes = ["https://ai.azure.com/.default"];

    public FoundryTokenProvider(TokenCredential credential)
    {
        _credential = credential;
    }

    public override AuthenticationToken GetToken(GetTokenOptions options, CancellationToken cancellationToken)
    {
        var token = _credential.GetToken(new TokenRequestContext(s_scopes), cancellationToken);
        return new AuthenticationToken(token.Token, "Bearer", token.ExpiresOn, null);
    }

    public override async ValueTask<AuthenticationToken> GetTokenAsync(GetTokenOptions options, CancellationToken cancellationToken)
    {
        var token = await _credential.GetTokenAsync(new TokenRequestContext(s_scopes), cancellationToken);
        return new AuthenticationToken(token.Token, "Bearer", token.ExpiresOn, null);
    }

    public override GetTokenOptions CreateTokenOptions(IReadOnlyDictionary<string, object?> properties)
        => new(properties!);
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
