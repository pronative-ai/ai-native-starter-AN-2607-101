using A2A;
using System.Text.Json;
using Pronative.MultiAgentTraining.Shared;

DotEnvLoader.Load();
var config = Lab03A2AConsumerConfig.FromEnvironment(args);

Day04Console.PrintAppStart();

ConsoleFormatting.Header("Day 4 Lab 03 - A2A Consumer");
Console.WriteLine("Official capability: A2A agent discovery and consumption.");
Console.WriteLine("Package: A2A 1.0.0-preview2.");
Console.WriteLine("Required APIs:");
Console.WriteLine("- A2ACardResolver.GetAgentCardAsync(...)");
Console.WriteLine("- A2AClient.SendMessageAsync(...)");
Console.WriteLine("- SendMessageRequest, Message, Part.FromText(...)");
Console.WriteLine();
Console.WriteLine("API Center role: enterprise catalog/governance registry.");
Console.WriteLine("Runtime role: the consumer calls the discovered A2A endpoint directly or through AgentGateway.");
Console.WriteLine();
Console.WriteLine($"Base URL: {config.A2ABaseUrl}");
Console.WriteLine($"Agent card path: {config.AgentCardPath}");
Console.WriteLine($"Prompt: {config.MessageText}");

using var httpClient = new HttpClient(new SocketsHttpHandler
{
    ConnectTimeout = TimeSpan.FromSeconds(100)
})
{
    Timeout = TimeSpan.FromSeconds(240)
};
ApplyOptionalAuthHeaders(httpClient);

ConsoleFormatting.Header("Discover Agent Card");
var resolver = new A2ACardResolver(
    new Uri(config.A2ABaseUrl),
    httpClient,
    config.AgentCardPath,
    logger: null);

A2A.AgentCard agentCard = null!;
var maxRetries = config.MaxRetries;
var retryDelaySeconds = config.RetryDelaySeconds;

for (var attempt = 1; attempt <= maxRetries; attempt++)
{
    try
    {
        agentCard = await resolver.GetAgentCardAsync();
        break;
    }
    catch (Exception ex) when (ex is A2A.A2AException or HttpRequestException
        or TimeoutException or OperationCanceledException)
    {
        if (attempt >= maxRetries)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Failed to connect to A2A agent host at {config.A2ABaseUrl} after {maxRetries} attempts.");
            Console.WriteLine("Ensure the A2A Agent Exposure (Lab03A2AAgentExposure) is running first.");
            Console.ResetColor();
            Environment.Exit(1);
        }

        Console.WriteLine($"Attempt {attempt}/{maxRetries}: A2A agent host not ready at {config.A2ABaseUrl}. Retrying in {retryDelaySeconds}s...");
        await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
    }
}

Console.WriteLine($"Name: {agentCard.Name}");
Console.WriteLine($"Description: {agentCard.Description}");
Console.WriteLine($"Version: {agentCard.Version}");
Console.WriteLine($"Supported interfaces: {agentCard.SupportedInterfaces.Count}");

foreach (var agentInterface in agentCard.SupportedInterfaces)
{
    Console.WriteLine($"- {agentInterface.ProtocolBinding} {agentInterface.ProtocolVersion}: {agentInterface.Url}");
}

var selectedInterface = agentCard.SupportedInterfaces.FirstOrDefault()
    ?? throw new InvalidOperationException("The discovered Agent Card did not contain a supported A2A interface.");

ConsoleFormatting.Header("Invoke Remote Agent");
Console.WriteLine($"Selected endpoint: {selectedInterface.Url}");

using var client = new A2AClient(new Uri(selectedInterface.Url), httpClient);
var response = await client.SendMessageAsync(new SendMessageRequest
{
    Message = new Message
    {
        MessageId = Guid.NewGuid().ToString("N"),
        Role = Role.User,
        Parts = [Part.FromText(config.MessageText)],
        ContextId = config.ContextId,
        Metadata = new Dictionary<string, JsonElement>
        {
            ["BatchId"] = JsonSerializer.SerializeToElement(config.BatchId),
            ["StudentId"] = JsonSerializer.SerializeToElement(config.StudentId),
            ["LabId"] = JsonSerializer.SerializeToElement("day04-lab03"),
            ["DiscoverySource"] = JsonSerializer.SerializeToElement("Azure API Center registration metadata")
        }
    }
});

ConsoleFormatting.Header("A2A Response");
switch (response.PayloadCase)
{
    case SendMessageResponseCase.Message:
        Console.WriteLine("Payload: Message");
        PrintMessage(response.Message);
        break;

    case SendMessageResponseCase.Task:
        Console.WriteLine("Payload: Task");
        Console.WriteLine($"Task ID: {response.Task?.Id}");
        Console.WriteLine($"Task state: {response.Task?.Status?.State}");
        if (response.Task?.Artifacts is { Count: > 0 })
        {
            foreach (var artifact in response.Task.Artifacts)
            {
                Console.WriteLine($"Artifact: {artifact.Name}");
                foreach (var part in artifact.Parts)
                {
                    PrintPart(part);
                }
            }
        }
        break;

    default:
        Console.WriteLine($"Payload: {response.PayloadCase}");
        Console.WriteLine("No message or task payload was returned.");
        break;
}

Day04Console.PrintLabStart(3);

ConsoleFormatting.Header("Training Takeaway");
Console.WriteLine("Provider exposes an Agent Card and message endpoint.");
Console.WriteLine("API Center stores the enterprise registration and governance metadata.");
Console.WriteLine("Consumer resolves the Agent Card and invokes the selected A2A interface.");
Console.WriteLine("AgentGateway can sit between consumer and provider when runtime policy, logs, rate limits, and cost attribution are required.");

Day04Console.PrintLabEnd(3);

Day04Console.PrintAppEnd();

static void ApplyOptionalAuthHeaders(HttpClient httpClient)
{
    var apiKey = Environment.GetEnvironmentVariable("A2A_API_KEY");
    if (!string.IsNullOrWhiteSpace(apiKey))
    {
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", apiKey);
    }

    var bearerToken = Environment.GetEnvironmentVariable("A2A_BEARER_TOKEN");
    if (!string.IsNullOrWhiteSpace(bearerToken))
    {
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", bearerToken);
    }
}

static void PrintMessage(Message? message)
{
    if (message is null)
    {
        Console.WriteLine("<empty message>");
        return;
    }

    Console.WriteLine($"Role: {message.Role}");
    Console.WriteLine($"Message ID: {message.MessageId}");
    Console.WriteLine($"Context ID: {message.ContextId}");

    foreach (var part in message.Parts)
    {
        PrintPart(part);
    }
}

static void PrintPart(Part part)
{
    if (part.Text is not null)
    {
        Console.WriteLine(part.Text);
        return;
    }

    if (part.Data is not null)
    {
        Console.WriteLine(part.Data);
        return;
    }

    if (part.Url is not null)
    {
        Console.WriteLine($"URL part: {part.Url} ({part.MediaType ?? "unknown media type"})");
        return;
    }

    if (part.Raw is not null)
    {
        Console.WriteLine($"Raw part: {part.Filename ?? "<unnamed>"} ({part.MediaType ?? "unknown media type"})");
        return;
    }

    Console.WriteLine($"Part type: {part.ContentCase}");
}

public sealed record Lab03A2AConsumerConfig(
    string BatchId,
    string StudentId,
    string A2ABaseUrl,
    string AgentCardPath,
    string ContextId,
    string MessageText,
    int MaxRetries,
    int RetryDelaySeconds)
{
    public static Lab03A2AConsumerConfig FromEnvironment(string[] args)
    {
        var batchId = Env("BATCH_ID", "AN-2607-101");
        var studentId = Env("STUDENT_ID", "ST-2606-1000");

        return new Lab03A2AConsumerConfig(
            batchId,
            studentId,
            Arg(args, "--base-url", Env("A2A_BASE_URL", "http://localhost:5000")),
            Arg(args, "--agent-card-path", Env("A2A_AGENT_CARD_PATH", "/a2a/training-ops/v1/card")),
            Arg(args, "--context-id", Env("A2A_CONTEXT_ID", $"{batchId.ToLowerInvariant()}-{studentId.ToLowerInvariant()}-day04-lab03")),
            Arg(args, "--message", Env("A2A_MESSAGE", "As a consumer agent, ask the training operations A2A agent to assess readiness for Day 4 protocol coverage.")),
            int.TryParse(Env("A2A_MAX_RETRIES", "10"), out var maxRetries) ? maxRetries : 10,
            int.TryParse(Env("A2A_RETRY_DELAY_SECONDS", "2"), out var delay) ? delay : 2);
    }

    private static string Env(string name, string fallback) =>
        string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name))
            ? fallback
            : Environment.GetEnvironmentVariable(name)!;

    private static string Arg(string[] args, string name, string fallback)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : fallback;
    }
}
