using DotNetEnv;
using Microsoft.Extensions.Configuration;

namespace Pronative.Day05.Shared;

public sealed record Day05Config
{
    public string BatchId { get; init; } = "AN-2607-101";
    public string StudentId { get; init; } = "ST-2606-1000";
    public string EnvironmentId { get; init; } = "an2607101";
    public string CostCenter { get; init; } = "Training";
    public string Owner { get; init; } = "pronative-ai";
    public string AzureOpenAiEndpoint { get; init; } = "https://proj-an2607101-default-resource.openai.azure.com/openai/v1";
    public string AzureOpenAiChatDeployment { get; init; } = "gpt-5-mini";
    public string AzureOpenAiApiKey { get; init; } = "";
    public string AzureOpenAiBearerToken { get; init; } = "";
    public string AgentGatewayEndpoint { get; init; } = "https://aks-an2607101-agw-dns-xywyud9k.hcp.centralindia.azmk8s.io";
    public string AgentGatewayRoute { get; init; } = "/openai/v1/chat/completions";
    public string AgentGatewayApiKey { get; init; } = "";
    public string AgentGatewayBearerToken { get; init; } = "";
    public string ApplicationInsightsConnectionString { get; init; } = "";
    public string LogAnalyticsWorkspaceId { get; init; } = "";
    public string RunpodEndpointId { get; init; } = "";
    public string RunpodApiKey { get; init; } = "";
    public string RunpodModel { get; init; } = "";
    public string AzureSubscriptionId { get; init; } = "";
    public int EvidenceLookbackDays { get; init; } = 7;

    public static Day05Config Load()
    {
        var envFile = FindEnvFile();
        if (envFile is not null)
        {
            Env.Load(envFile);
        }

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return new Day05Config
        {
            BatchId = Read(config, "BATCH_ID", "AN-2607-101"),
            StudentId = Read(config, "STUDENT_ID", "ST-2606-1000"),
            EnvironmentId = Read(config, "ENVIRONMENT_ID", "an2607101"),
            CostCenter = Read(config, "COST_CENTER", "Training"),
            Owner = Read(config, "OWNER", "pronative-ai"),
            AzureOpenAiEndpoint = Read(config, "AZURE_OPENAI_ENDPOINT", "https://proj-an2607101-default-resource.openai.azure.com/openai/v1").TrimEnd('/'),
            AzureOpenAiChatDeployment = Read(config, "AZURE_OPENAI_CHAT_DEPLOYMENT", "gpt-5-mini"),
            AzureOpenAiApiKey = Read(config, "AZURE_OPENAI_API_KEY", ""),
            AzureOpenAiBearerToken = Read(config, "AZURE_OPENAI_BEARER_TOKEN", ""),
            AgentGatewayEndpoint = Read(config, "AGENTGATEWAY_ENDPOINT", "https://aks-an2607101-agw-dns-xywyud9k.hcp.centralindia.azmk8s.io").TrimEnd('/'),
            AgentGatewayRoute = Read(config, "AGENTGATEWAY_ROUTE", "/openai/v1/chat/completions"),
            AgentGatewayApiKey = Read(config, "AGENTGATEWAY_API_KEY", ""),
            AgentGatewayBearerToken = Read(config, "AGENTGATEWAY_BEARER_TOKEN", ""),
            ApplicationInsightsConnectionString = Read(config, "APPLICATIONINSIGHTS_CONNECTION_STRING", ""),
            LogAnalyticsWorkspaceId = Read(config, "LOG_ANALYTICS_WORKSPACE_ID", ""),
            RunpodEndpointId = Read(config, "RUNPOD_ENDPOINT_ID", ""),
            RunpodApiKey = Read(config, "RUNPOD_API_KEY", ""),
            RunpodModel = Read(config, "RUNPOD_MODEL", ""),
            AzureSubscriptionId = Read(config, "AZURE_SUBSCRIPTION_ID", ""),
            EvidenceLookbackDays = ReadInt(config, "EVIDENCE_LOOKBACK_DAYS", 7)
        };
    }

    public IDictionary<string, string> StandardTags() => new Dictionary<string, string>
    {
        ["BatchId"] = BatchId,
        ["StudentId"] = StudentId,
        ["EnvironmentId"] = EnvironmentId,
        ["CostCenter"] = CostCenter,
        ["Owner"] = Owner
    };

    private static string Read(IConfiguration config, string key, string fallback)
    {
        return config[key] ?? config[key.Replace("_", ":")] ?? fallback;
    }

    private static int ReadInt(IConfiguration config, string key, int fallback)
    {
        var value = Read(config, key, "");
        return int.TryParse(value, out var result) && result > 0 ? result : fallback;
    }

    private static string? FindEnvFile()
    {
        var candidates = new[]
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory(),
            Environment.CurrentDirectory
        };

        foreach (var start in candidates.Distinct())
        {
            var dir = new DirectoryInfo(start);
            while (dir is not null)
            {
                var path = Path.Combine(dir.FullName, ".env");
                if (File.Exists(path))
                    return path;
                dir = dir.Parent;
            }
        }

        return null;
    }
}
