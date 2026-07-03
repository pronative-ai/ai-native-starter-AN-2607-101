using System.Text.Json;

namespace Pronative.AgentTraining.Shared;

public sealed record TrainingConfig
{
    public string BatchId { get; init; } = "AN-2607-101";
    public string StudentId { get; init; } = "ST-2606-1000";
    public string OpenAiV1Endpoint { get; init; } = "https://proj-an2607101-default-resource.openai.azure.com/openai/v1";
    public string ModelDeployment { get; init; } = "gpt-5-mini";
    public string ApiKey { get; init; } = "";
    public string BearerToken { get; init; } = "";
    public string AgentName { get; init; } = "it-support-agent";
    public string WorkflowName { get; init; } = "ContosoPay-Customer-Support-Triage";
    public string McpServerUrl { get; init; } = "https://trainer-configured-mcp-endpoint.example";
    public bool UseLiveFoundry { get; init; } = false;

    public static TrainingConfig Load(string[] args)
    {
        var filePath = args.FirstOrDefault(a => a.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            ?? "appsettings.json";

        var fromFile = File.Exists(filePath)
            ? JsonSerializer.Deserialize<TrainingConfig>(
                File.ReadAllText(filePath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : new TrainingConfig();

        return (fromFile ?? new TrainingConfig()).WithEnvironmentOverrides();
    }

    private TrainingConfig WithEnvironmentOverrides() => this with
    {
        BatchId = Environment.GetEnvironmentVariable("BATCH_ID") ?? BatchId,
        StudentId = Environment.GetEnvironmentVariable("STUDENT_ID") ?? StudentId,
        OpenAiV1Endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_V1_ENDPOINT") ?? OpenAiV1Endpoint,
        ModelDeployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_DEPLOYMENT") ?? ModelDeployment,
        ApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? ApiKey,
        BearerToken = Environment.GetEnvironmentVariable("AZURE_OPENAI_BEARER_TOKEN") ?? BearerToken,
        AgentName = Environment.GetEnvironmentVariable("FOUNDRY_AGENT_NAME") ?? AgentName,
        WorkflowName = Environment.GetEnvironmentVariable("FOUNDRY_WORKFLOW_NAME") ?? WorkflowName,
        McpServerUrl = Environment.GetEnvironmentVariable("MCP_SERVER_URL") ?? McpServerUrl,
        UseLiveFoundry = bool.TryParse(Environment.GetEnvironmentVariable("USE_LIVE_FOUNDRY"), out var useLive)
            ? useLive
            : UseLiveFoundry
    };
}

public static class TrainingConfigConsole
{
    public static void Print(TrainingConfig config, string labName)
    {
        Console.WriteLine($"ProNative AI-Native Fullstack Engineering - Day 2 - {labName}");
        Console.WriteLine($"Batch: {config.BatchId} | Student: {config.StudentId}");
        Console.WriteLine($"Foundry/OpenAI v1 endpoint: {config.OpenAiV1Endpoint}");
        Console.WriteLine($"Model deployment: {config.ModelDeployment}");
        Console.WriteLine($"Live Foundry calls enabled: {config.UseLiveFoundry}");
        Console.WriteLine();
    }
}
