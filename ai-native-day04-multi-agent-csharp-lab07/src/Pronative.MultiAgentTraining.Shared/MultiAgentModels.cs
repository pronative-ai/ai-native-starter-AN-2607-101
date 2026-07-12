namespace Pronative.MultiAgentTraining.Shared;

public sealed record MultiAgentOptions(
    string BatchId,
    string StudentId,
    string GatewayEndpoint,
    string GatewayRoute)
{
    public static MultiAgentOptions FromEnvironment() => new(
        Env("PN_BATCH_ID", "AN-2607-101"),
        Env("PN_STUDENT_ID", "ST-2606-1000"),
        Env("PN_AGENTGATEWAY_ENDPOINT", "https://agentgateway-an2607101.azurecontainerapps.io"),
        Env("PN_AGENTGATEWAY_ROUTE", "agents/live-project-readiness"));

    private static string Env(string name, string fallback) =>
        string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name))
            ? fallback
            : Environment.GetEnvironmentVariable(name)!;
}

public sealed record AgentMessage(string AgentName, string Role, string Content, bool ApprovalRequired = false);

public sealed record AgentCard(
    string Name,
    string Description,
    IReadOnlyList<string> Capabilities,
    string Endpoint,
    string Auth);

public sealed record GatewayRoute(
    string Name,
    string Path,
    string Target,
    int RequestsPerMinute,
    int MaxTokensPerRequest,
    IReadOnlyDictionary<string, string> Tags);
