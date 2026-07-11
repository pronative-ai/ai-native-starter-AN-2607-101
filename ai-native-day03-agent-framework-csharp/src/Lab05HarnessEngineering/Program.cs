#pragma warning disable MAAI001

using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Pronative.Day03.Shared;

var config = Day03TrainingConfig.Load(args);

Day03Console.PrintHeader(config, "Lab 05 - Harness Engineering");

Console.WriteLine("This lab uses the Microsoft Agent Framework Harness directly:");
Console.WriteLine("- AzureOpenAIClient + AzureCliCredential for the live model client");
Console.WriteLine("- ChatClient.AsIChatClient() from Microsoft.Extensions.AI.OpenAI");
Console.WriteLine("- AsHarnessAgent(new HarnessAgentOptions { ... })");
Console.WriteLine("- LoopEvaluators through HarnessAgentOptions");
Console.WriteLine("- Harness compaction through MaxContextWindowTokens and MaxOutputTokens");
Console.WriteLine("- Harness-managed tool approval, file memory/access, todo/mode providers, and OpenTelemetry");
Console.WriteLine();

var azureOpenAiEndpoint = ResolveAzureOpenAiEndpoint();
var artifactsDirectory = Path.Combine(AppContext.BaseDirectory, "artifacts", config.StudentId);
var fileStoreDirectory = Path.Combine(artifactsDirectory, "harness-file-store");
Directory.CreateDirectory(fileStoreDirectory);

var fileStore = new FileSystemAgentFileStore(fileStoreDirectory);

await fileStore.WriteAsync(
    "input/day03-lab05-request.md",
    $"""
    # Harness Lab Input

    Batch: {config.BatchId}
    Student: {config.StudentId}
    Request: Review whether the Day 3 Lab 05 harness is ready for classroom delivery.
    Required output headings: Evidence, Evaluation, Completion.
    Completion marker: DONE.
    """,
    CancellationToken.None);

// Harness tools created via AIFunctionFactory; they are passed through ChatOptions.Tools to the model
var assessReadinessTool = AIFunctionFactory.Create(
    (Func<string, string, string, string>)HarnessTrainingTools.AssessHarnessReadiness,
    name: "assess_harness_readiness",
    description: "Assess whether a harness-based training lab is ready. Inputs are batchId, studentId, and request.");

var captureEvidenceTool = AIFunctionFactory.Create(
    (Func<string, string, string, string, string>)HarnessTrainingTools.CaptureHarnessEvidence,
    name: "capture_harness_evidence",
    description: "Create structured evidence from the readiness assessment. Inputs are batchId, studentId, request, and assessmentJson.");

// Live Azure OpenAI client: AzureOpenAIClient -> ChatClient -> IChatClient, wrapped directly by the harness
var azureOpenAiClient = new AzureOpenAIClient(azureOpenAiEndpoint, new AzureCliCredential());
using IChatClient chatClient = azureOpenAiClient
    .GetChatClient(config.ModelDeployment)
    .AsIChatClient();

// AsHarnessAgent creates the harness-managed agent with all built-in providers (todo, mode, file memory/access, compaction, OpenTelemetry, loop evaluators)
AIAgent harnessAgent = chatClient.AsHarnessAgent(new HarnessAgentOptions
{
    Id = $"day03-lab05-{config.StudentId.ToLowerInvariant()}",
    Name = "day03-lab05-harness-agent",
    Description = "Official Microsoft Agent Framework Harness lab for repeatable prompts, tools, stateful context, compaction, approval, and traces.",

    HarnessInstructions =
        """
        You are running inside Microsoft Agent Framework Harness.
        Use the harness-managed todo and mode providers to keep track of the work.
        Use tools for operational evidence instead of inventing readiness data.
        Use file memory/access when you need to persist or inspect lab evidence.
        End the final response with DONE only when the Evidence, Evaluation, and Completion sections are present.
        """,

    ChatOptions = new ChatOptions
    {
        Instructions =
            """
            You are a ProNative AI harness-readiness agent for Day 3 training.

            For each request:
            1. Call assess_harness_readiness.
            2. Call capture_harness_evidence using the assessment JSON.
            3. Return exactly these headings: Evidence, Evaluation, Completion.
            4. Mention the batch ID and student ID.
            5. End the answer with DONE.
            """,
        Tools =
        [
            assessReadinessTool,
            captureEvidenceTool
        ],
        MaxOutputTokens = 1200
    },

    // Compaction is configured through the harness. When both values are set,
    // the harness creates its context-window compaction pipeline.
    MaxContextWindowTokens = 16000,
    MaxOutputTokens = 1200,
    DisableCompaction = false,

    // Harness built-in providers explicitly enabled for training visibility
    DisableAgentModeProvider = false,
    DisableFileMemory = false,
    DisableFileAccess = false,
    FileMemoryStore = fileStore,
    FileAccessStore = fileStore,

    // The hosted web-search tool is not required for this harness lab. Leaving
    // it enabled can fail when the selected Azure model endpoint does not expose
    // hosted web-search support.
    DisableWebSearch = true,

    // Tool approval is provided by the harness through ToolApprovalAgent.
    DisableToolAutoApproval = false,
    ToolApprovalAgentOptions = new ToolApprovalAgentOptions
    {
        AutoApprovalRules =
        [
            functionCall => new ValueTask<bool>(
                functionCall.Name is "assess_harness_readiness" or "capture_harness_evidence"),
            FileAccessProvider.ReadOnlyToolsAutoApprovalRule
        ]
    },

    // OpenTelemetry is configured through HarnessAgentOptions, not by manually
    // wrapping the agent with OpenTelemetryAgent.
    DisableOpenTelemetry = false,
    OpenTelemetrySourceName = "pronative.day03.lab05.harness",

    // LoopAgent with CompletionMarkerLoopEvaluator: stops when "DONE" appears in the response
    LoopEvaluators =
    [
        new CompletionMarkerLoopEvaluator("DONE")
    ],
    MaximumIterationsPerRequest = 2
});

// Harness session with StateBag metadata: batch, student, endpoint, and file store evidence
AgentSession session = await harnessAgent.CreateSessionAsync();
session.StateBag.SetValue("BatchId", config.BatchId);
session.StateBag.SetValue("StudentId", config.StudentId);
session.StateBag.SetValue("HarnessEvidence", "Microsoft.Agents.AI.Harness + AsHarnessAgent + HarnessAgentOptions");
session.StateBag.SetValue("AzureOpenAiEndpoint", azureOpenAiEndpoint.ToString());
session.StateBag.SetValue("FileStoreDirectory", fileStoreDirectory);

var prompt =
    $"""
    Review whether the Day 3 Lab 05 harness is ready for classroom delivery.

    BatchId: {config.BatchId}
    StudentId: {config.StudentId}

    Use the harness-managed tools and context providers. Read or persist evidence if useful.
    The final answer must include Evidence, Evaluation, and Completion headings and must end with DONE.
    """;

Console.WriteLine("Step 1 - Harness configuration evidence");
Console.WriteLine("=======================================");
Console.WriteLine($"Package: Microsoft.Agents.AI.Harness");
Console.WriteLine($"Required construction: AsHarnessAgent(new HarnessAgentOptions {{ ... }})");
Console.WriteLine($"Azure OpenAI endpoint: {azureOpenAiEndpoint}");
Console.WriteLine($"Model deployment: {config.ModelDeployment}");
Console.WriteLine($"File store: {fileStoreDirectory}");
Console.WriteLine("Loop evaluator: CompletionMarkerLoopEvaluator(\"DONE\")");
Console.WriteLine("Tool approval: ToolApprovalAgentOptions on HarnessAgentOptions");
Console.WriteLine("Compaction: MaxContextWindowTokens + MaxOutputTokens on HarnessAgentOptions");
Console.WriteLine("OpenTelemetry: OpenTelemetrySourceName on HarnessAgentOptions");
Console.WriteLine();

Console.WriteLine("Step 2 - Run repeatable harness prompt");
Console.WriteLine("======================================");
Console.WriteLine(prompt);
Console.WriteLine();

var response = await harnessAgent.RunAsync(prompt, session);

Console.WriteLine("Step 3 - Harness response");
Console.WriteLine("=========================");
Console.WriteLine(response.ToString());
Console.WriteLine();

Console.WriteLine("Step 4 - Session evidence");
Console.WriteLine("=========================");
Console.WriteLine(session.StateBag.Serialize());
Console.WriteLine();

var evidenceSnapshot = new HarnessEvidenceSnapshot(
    config.BatchId,
    config.StudentId,
    azureOpenAiEndpoint.ToString(),
    config.ModelDeployment,
    response.ToString(),
    DateTimeOffset.UtcNow);

await fileStore.WriteAsync(
    "output/day03-lab05-harness-evidence.json",
    JsonSerializer.Serialize(evidenceSnapshot, new JsonSerializerOptions { WriteIndented = true }),
    CancellationToken.None);

Console.WriteLine("Step 5 - Evidence artifact");
Console.WriteLine("==========================");
Console.WriteLine("Harness evidence written to the harness file store:");
Console.WriteLine("output/day03-lab05-harness-evidence.json");
Console.WriteLine();
Console.WriteLine("Trainer acceptance check:");
Console.WriteLine("- Code contains PackageReference Include=\"Microsoft.Agents.AI.Harness\"");
Console.WriteLine("- Code contains AsHarnessAgent(new HarnessAgentOptions { ... })");
Console.WriteLine("- Code contains LoopEvaluators, compaction options, tool approval, file memory/access stores, and OpenTelemetry options");

static Uri ResolveAzureOpenAiEndpoint()
{
    var endpoint =
        Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
        ?? Environment.GetEnvironmentVariable("AZURE_AI_FOUNDRY_OPENAI_ENDPOINT")
        ?? "https://proj-an2607101-default-resource.openai.azure.com/";

    if (endpoint.EndsWith("/openai/v1", StringComparison.OrdinalIgnoreCase))
    {
        endpoint = endpoint[..^"/openai/v1".Length];
    }

    return new Uri(endpoint.TrimEnd('/') + "/");
}

internal static class HarnessTrainingTools
{
    public static string AssessHarnessReadiness(string batchId, string studentId, string request)
    {
        var result = new
        {
            batchId,
            studentId,
            request,
            status = "ready-with-live-azure-prerequisites",
            requiredHarnessCapabilities = new[]
            {
                "HarnessAgent or AsHarnessAgent",
                "HarnessAgentOptions",
                "LoopEvaluators",
                "Context-window compaction",
                "ToolApprovalAgent through harness options",
                "FileMemoryProvider and FileAccessProvider through harness options",
                "TodoProvider and AgentModeProvider through harness defaults",
                "OpenTelemetry through harness options"
            },
            classroomRisk = "Requires az login and an Azure OpenAI-compatible endpoint with access to the configured model deployment.",
            recommendation = "Proceed when the live model call succeeds and the DONE loop evaluator stops within the iteration limit."
        };

        return JsonSerializer.Serialize(result);
    }

    public static string CaptureHarnessEvidence(string batchId, string studentId, string request, string assessmentJson)
    {
        var evidence = new
        {
            batchId,
            studentId,
            request,
            assessmentJson,
            evidence = new[]
            {
                "The harness owns the agent runtime construction.",
                "The model receives function tools through ChatOptions.Tools.",
                "The harness owns loop execution through LoopEvaluators.",
                "The harness owns tool approval and OpenTelemetry wiring through HarnessAgentOptions.",
                "File memory/access stores are configured on HarnessAgentOptions."
            },
            completionCriteria = "Final response contains Evidence, Evaluation, Completion, and DONE."
        };

        return JsonSerializer.Serialize(evidence);
    }
}

internal sealed record HarnessEvidenceSnapshot(
    string BatchId,
    string StudentId,
    string AzureOpenAiEndpoint,
    string ModelDeployment,
    string Response,
    DateTimeOffset CapturedAtUtc);
