using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Pronative.Day05.Shared;

ConsoleTable.ApplicationStart(06);

var config = Day05Config.Load();
using var activitySource = new ActivitySource("Pronative.Day05.Lab06");

ConsoleTable.Header("Day 5 Lab 06 - Runpod vLLM Neocloud Runtime");
ConsoleTable.Row("EndpointId", string.IsNullOrWhiteSpace(config.RunpodEndpointId) ? "<not set>" : config.RunpodEndpointId);
ConsoleTable.Row("Model", string.IsNullOrWhiteSpace(config.RunpodModel) ? "<not set>" : config.RunpodModel);
ConsoleTable.Row("Batch", config.BatchId);
ConsoleTable.Row("Student", config.StudentId);

if (string.IsNullOrWhiteSpace(config.RunpodEndpointId) ||
    string.IsNullOrWhiteSpace(config.RunpodApiKey) ||
    string.IsNullOrWhiteSpace(config.RunpodModel))
{
    ConsoleTable.Warning("Set RUNPOD_ENDPOINT_ID, RUNPOD_API_KEY, and RUNPOD_MODEL before running this lab.");
    Console.WriteLine("This lab is complete only when it calls a live Runpod vLLM/OpenAI-compatible endpoint.");
    Console.WriteLine("Expected base URL:");
    Console.WriteLine("https://api.runpod.ai/v2/{RUNPOD_ENDPOINT_ID}/openai/v1");
    ConsoleTable.ApplicationEnd(06);
    Environment.ExitCode = 1;
    return;
}

using var activity = activitySource.StartActivity("ai.native.runpod.vllm.chat_completion", ActivityKind.Client);
activity?.SetTag("BatchId", config.BatchId);
activity?.SetTag("StudentId", config.StudentId);
activity?.SetTag("runpod.endpoint_id", config.RunpodEndpointId);
activity?.SetTag("ai.model.deployment", config.RunpodModel);
activity?.SetTag("runtime.type", "neocloud-vllm");

var baseUrl = Environment.GetEnvironmentVariable("RUNPOD_OPENAI_BASE_URL") ??
              $"https://api.runpod.ai/v2/{config.RunpodEndpointId}/openai/v1";
var url = $"{baseUrl.TrimEnd('/')}/chat/completions";

var prompt = Environment.GetEnvironmentVariable("RUNPOD_PROMPT") ??
             "Explain one risk and one cost-control practice for neocloud LLM inference.";

var requestBody = new
{
    model = config.RunpodModel,
    messages = new[]
    {
        new
        {
            role = "system",
            content = "You are an AI operations assistant. Keep the answer concise and operational."
        },
        new
        {
            role = "user",
            content = prompt
        }
    },
    max_tokens = 180,
    temperature = 0.2
};

using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
using var request = new HttpRequestMessage(HttpMethod.Post, url);
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.RunpodApiKey);
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

    ConsoleTable.Header("Runpod vLLM Response");
    ConsoleTable.Row("Status", $"{(int)response.StatusCode} {response.ReasonPhrase}");
    ConsoleTable.Row("LatencyMs", $"{elapsed.TotalMilliseconds:F0}");

    if (!response.IsSuccessStatusCode)
    {
        activity?.SetStatus(ActivityStatusCode.Error, response.ReasonPhrase);
        Console.WriteLine(OpenAiResponse.TryPrettyJson(responseText));
        WriteEvent(config, url, elapsed, false, null, response.ReasonPhrase);
        Console.WriteLine("================================================================================");
        Console.WriteLine("                     Application 06 End");
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

    WriteEvent(config, url, elapsed, true, usage, null);

    Console.WriteLine();
    Console.WriteLine("Trainer walkthrough:");
    Console.WriteLine("- Open Runpod endpoint logs and worker lifecycle.");
    Console.WriteLine("- Compare first-call cold start with subsequent warm calls.");
    Console.WriteLine("- Verify max workers, active workers, timeout, GPU type, and endpoint shutdown controls.");

ConsoleTable.ApplicationEnd(06);
}
catch (Exception ex)
{
    var elapsed = Stopwatch.GetElapsedTime(started);
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    WriteEvent(config, url, elapsed, false, null, ex.Message, ex.GetType().Name);
ConsoleTable.ApplicationEnd(06);
    Environment.ExitCode = 1;
}

static void WriteEvent(
    Day05Config config,
    string backend,
    TimeSpan elapsed,
    bool success,
    OpenAiUsage? usage,
    string? errorMessage,
    string? errorType = null)
{
    var evt = OperationalEvent.FromConfig(
        config,
        eventName: success ? "lab06.runpod_vllm_completed" : "lab06.runpod_vllm_failed",
        agentName: "runpod-neocloud-vllm-client",
        modelDeployment: config.RunpodModel,
        backend: backend,
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
