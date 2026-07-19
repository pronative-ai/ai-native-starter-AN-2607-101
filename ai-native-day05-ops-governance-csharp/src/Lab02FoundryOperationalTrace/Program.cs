using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Pronative.Day05.Shared;

ConsoleTable.ApplicationStart(02);

var config = Day05Config.Load();
using var activitySource = new ActivitySource("Pronative.Day05.Lab02");

ConsoleTable.Header("Day 5 Lab 02 - Foundry Operational Troubleshooting and Run Evidence");
ConsoleTable.Row("Endpoint", config.AzureOpenAiEndpoint);
ConsoleTable.Row("Deployment", config.AzureOpenAiChatDeployment);
ConsoleTable.Row("Batch", config.BatchId);
ConsoleTable.Row("Student", config.StudentId);

if (string.IsNullOrWhiteSpace(config.AzureOpenAiApiKey) &&
    string.IsNullOrWhiteSpace(config.AzureOpenAiBearerToken))
{
    ConsoleTable.Warning("Set AZURE_OPENAI_API_KEY or AZURE_OPENAI_BEARER_TOKEN before running this lab.");
    Console.WriteLine("This lab must make a live Foundry/Azure OpenAI call to produce operational evidence.");
ConsoleTable.ApplicationEnd(02);
    Environment.ExitCode = 1;
    return;
}

using var activity = activitySource.StartActivity("ai.native.foundry.operational_trace", ActivityKind.Client);
activity?.SetTag("BatchId", config.BatchId);
activity?.SetTag("StudentId", config.StudentId);
activity?.SetTag("ai.model.deployment", config.AzureOpenAiChatDeployment);

var requestBody = new
{
    model = config.AzureOpenAiChatDeployment,
    messages = new[]
    {
        new
        {
            role = "system",
            content = "You are an AI operations assistant. Answer briefly and include one troubleshooting signal."
        },
        new
        {
            role = "user",
            content = Environment.GetEnvironmentVariable("LAB02_PROMPT") ??
                      "A user reports that an agent response is slow. What evidence should we collect first?"
        }
    },
    max_completion_tokens = 180
};

using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
using var request = new HttpRequestMessage(HttpMethod.Post, $"{config.AzureOpenAiEndpoint}/chat/completions");

if (!string.IsNullOrWhiteSpace(config.AzureOpenAiApiKey))
{
    request.Headers.TryAddWithoutValidation("api-key", config.AzureOpenAiApiKey);
}
else
{
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.AzureOpenAiBearerToken);
}

TraceHeaders.Apply(request, config);
request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

var started = Stopwatch.GetTimestamp();

try
{
    using var response = await client.SendAsync(request);
    var responseText = await response.Content.ReadAsStringAsync();
    var elapsed = Stopwatch.GetElapsedTime(started);

    activity?.SetTag("http.response.status_code", (int)response.StatusCode);
    activity?.SetTag("ai.latency_ms", elapsed.TotalMilliseconds);

    ConsoleTable.Header("HTTP Result");
    ConsoleTable.Row("Status", $"{(int)response.StatusCode} {response.ReasonPhrase}");
    ConsoleTable.Row("LatencyMs", $"{elapsed.TotalMilliseconds:F0}");

    foreach (var header in response.Headers)
    {
        if (header.Key.Contains("request", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Contains("trace", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Contains("apim", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"{header.Key}: {string.Join(",", header.Value)}");
        }
    }

    if (!response.IsSuccessStatusCode)
    {
        activity?.SetStatus(ActivityStatusCode.Error, response.ReasonPhrase);
        ConsoleTable.Header("Error Body");
        Console.WriteLine(OpenAiResponse.TryPrettyJson(responseText));

        WriteEvent(config, elapsed, false, null, response.ReasonPhrase);
        Console.WriteLine("================================================================================");
        Console.WriteLine("                     Application 02 End");
        Console.WriteLine("================================================================================");
        Environment.ExitCode = 1;
        return;
    }

    var usage = OpenAiResponse.ExtractUsage(responseText);
    activity?.SetTag("ai.prompt.tokens", usage.PromptTokens);
    activity?.SetTag("ai.completion.tokens", usage.CompletionTokens);
    activity?.SetTag("ai.total.tokens", usage.TotalTokens);

    ConsoleTable.Header("Model Answer");
    Console.WriteLine(OpenAiResponse.ExtractChatAnswerOrRawJson(responseText));

    WriteEvent(config, elapsed, true, usage, null);

    ConsoleTable.Header("Troubleshooting Modes");
    Console.WriteLine("- Change AZURE_OPENAI_CHAT_DEPLOYMENT to a non-existent deployment to observe deployment errors.");
    Console.WriteLine("- Remove auth to observe 401/403 readiness failures.");
    Console.WriteLine("- Increase prompt size or run concurrent calls to discuss latency and throttling evidence.");

ConsoleTable.ApplicationEnd(02);
}
catch (Exception ex)
{
    var elapsed = Stopwatch.GetElapsedTime(started);
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    WriteEvent(config, elapsed, false, null, ex.Message, ex.GetType().Name);
ConsoleTable.ApplicationEnd(02);
    Environment.ExitCode = 1;
}

static void WriteEvent(
    Day05Config config,
    TimeSpan elapsed,
    bool success,
    OpenAiUsage? usage,
    string? errorMessage,
    string? errorType = null)
{
    var evt = OperationalEvent.FromConfig(
        config,
        eventName: success ? "lab02.foundry_chat_completed" : "lab02.foundry_chat_failed",
        agentName: "day05-operational-trace-client",
        modelDeployment: config.AzureOpenAiChatDeployment,
        backend: config.AzureOpenAiEndpoint,
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
