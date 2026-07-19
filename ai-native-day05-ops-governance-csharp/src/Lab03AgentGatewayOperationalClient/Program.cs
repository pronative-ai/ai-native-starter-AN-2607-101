using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Pronative.Day05.Shared;

ConsoleTable.ApplicationStart(03);

var config = Day05Config.Load();
using var activitySource = new ActivitySource("Pronative.Day05.Lab03");

ConsoleTable.Header("Day 5 Lab 03 - AgentGateway Runtime Control for AI Traffic");
ConsoleTable.Row("Gateway", config.AgentGatewayEndpoint);
ConsoleTable.Row("Route", config.AgentGatewayRoute);
ConsoleTable.Row("Batch", config.BatchId);
ConsoleTable.Row("Student", config.StudentId);

if (config.AgentGatewayRoute.Contains("health", StringComparison.OrdinalIgnoreCase))
{
    ConsoleTable.Warning("AGENTGATEWAY_ROUTE points to a health endpoint. Use an AI/model route for lab completion.");
    Console.WriteLine("Recommended shape: /openai/v1/chat/completions or the equivalent route configured in your AgentGateway.");
    Console.WriteLine("================================================================================");
    Console.WriteLine("                     Application 03 End");
    Console.WriteLine("================================================================================");
    Environment.ExitCode = 1;
    return;
}

using var activity = activitySource.StartActivity("ai.native.agentgateway.chat_completion", ActivityKind.Client);
activity?.SetTag("BatchId", config.BatchId);
activity?.SetTag("StudentId", config.StudentId);
activity?.SetTag("gateway.endpoint", config.AgentGatewayEndpoint);
activity?.SetTag("gateway.route", config.AgentGatewayRoute);
activity?.SetTag("ai.model.deployment", config.AzureOpenAiChatDeployment);

var route = config.AgentGatewayRoute.StartsWith('/') ? config.AgentGatewayRoute : "/" + config.AgentGatewayRoute;
var requestUrl = $"{config.AgentGatewayEndpoint}{route}";

var requestBody = Environment.GetEnvironmentVariable("AGENTGATEWAY_BODY");
if (string.IsNullOrWhiteSpace(requestBody))
{
    requestBody = JsonSerializer.Serialize(new
    {
        model = config.AzureOpenAiChatDeployment,
        messages = new[]
        {
            new
            {
                role = "system",
                content = "You are an AI gateway operations assistant. Answer with one route/control signal."
            },
            new
            {
                role = "user",
                content = "What should an AgentGateway log prove for an enterprise AI request?"
            }
        },
        max_completion_tokens = 180
    });
}

using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
TraceHeaders.Apply(request, config);

if (!string.IsNullOrWhiteSpace(config.AgentGatewayApiKey))
{
    request.Headers.TryAddWithoutValidation("api-key", config.AgentGatewayApiKey);
}

if (!string.IsNullOrWhiteSpace(config.AgentGatewayBearerToken))
{
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.AgentGatewayBearerToken);
}

request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

ConsoleTable.Header("Request Headers To Observe In Gateway / AKS Logs");
foreach (var header in request.Headers)
{
    if (!header.Key.Contains("Authorization", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"{header.Key}: {string.Join(",", header.Value)}");
    }
}

var started = Stopwatch.GetTimestamp();

try
{
    using var response = await client.SendAsync(request);
    var responseText = await response.Content.ReadAsStringAsync();
    var elapsed = Stopwatch.GetElapsedTime(started);

    activity?.SetTag("http.response.status_code", (int)response.StatusCode);
    activity?.SetTag("ai.latency_ms", elapsed.TotalMilliseconds);

    ConsoleTable.Header("Gateway Response");
    ConsoleTable.Row("Status", $"{(int)response.StatusCode} {response.ReasonPhrase}");
    ConsoleTable.Row("LatencyMs", $"{elapsed.TotalMilliseconds:F0}");

    if (!response.IsSuccessStatusCode)
    {
        activity?.SetStatus(ActivityStatusCode.Error, response.ReasonPhrase);
        Console.WriteLine(Shorten(responseText, 1600));
        WriteEvent(config, route, elapsed, false, null, response.ReasonPhrase);
        Console.WriteLine("================================================================================");
        Console.WriteLine("                     Application 03 End");
        Console.WriteLine("================================================================================");
        Environment.ExitCode = 1;
        return;
    }

    var usage = TryExtractUsage(responseText);
    if (usage is not null)
    {
        activity?.SetTag("ai.prompt.tokens", usage.PromptTokens);
        activity?.SetTag("ai.completion.tokens", usage.CompletionTokens);
        activity?.SetTag("ai.total.tokens", usage.TotalTokens);
    }

    Console.WriteLine(Shorten(responseText, 1800));
    WriteEvent(config, route, elapsed, true, usage, null);
}
catch (Exception ex)
{
    var elapsed = Stopwatch.GetElapsedTime(started);
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    WriteEvent(config, route, elapsed, false, null, ex.Message, ex.GetType().Name);
    Environment.ExitCode = 1;
}

Console.WriteLine();
Console.WriteLine("Trainer walkthrough:");
Console.WriteLine("- Find this request in AgentGateway logs, AKS pod logs, or Container Insights / Log Analytics.");
Console.WriteLine("- Correlate using x-batch-id, x-student-id, traceparent, or x-trace-id.");
Console.WriteLine("- Discuss route, backend, policy, rate limit, timeout, identity, and cost/token attribution.");

ConsoleTable.ApplicationEnd(03);

static OpenAiUsage? TryExtractUsage(string responseText)
{
    try
    {
        return OpenAiResponse.ExtractUsage(responseText);
    }
    catch
    {
        return null;
    }
}

static void WriteEvent(
    Day05Config config,
    string route,
    TimeSpan elapsed,
    bool success,
    OpenAiUsage? usage,
    string? errorMessage,
    string? errorType = null)
{
    var evt = OperationalEvent.FromConfig(
        config,
        eventName: success ? "lab03.agentgateway_ai_request_completed" : "lab03.agentgateway_ai_request_failed",
        agentName: "gateway-ops-client",
        modelDeployment: config.AzureOpenAiChatDeployment,
        gatewayRoute: route,
        backend: config.AgentGatewayEndpoint,
        latency: elapsed,
        success: success,
        errorType: errorType,
        errorMessage: errorMessage) with
    {
        PromptTokens = usage?.PromptTokens,
        CompletionTokens = usage?.CompletionTokens,
        TotalTokens = usage?.TotalTokens
    };

    ConsoleTable.Header("Operational Event");
    evt.WriteToConsole();
}

static string Shorten(string value, int maxLength)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return "<empty response>";
    }

    var shortened = value.Length <= maxLength ? value : value[..maxLength] + "\n... truncated ...";
    return OpenAiResponse.TryPrettyJson(shortened);
}
