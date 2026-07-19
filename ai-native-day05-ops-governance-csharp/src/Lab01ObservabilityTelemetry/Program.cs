using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Pronative.Day05.Shared;

ConsoleTable.ApplicationStart(01);

const string activitySourceName = "Pronative.Day05.Lab01";

var config = Day05Config.Load();
using var activitySource = new ActivitySource(activitySourceName);

var tracerBuilder = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
        serviceName: "day05-lab01-llmops-genaiops-observability",
        serviceVersion: "2.0.0"))
    .AddSource(activitySourceName)
    .AddConsoleExporter();

if (!string.IsNullOrWhiteSpace(config.ApplicationInsightsConnectionString))
{
    tracerBuilder.AddAzureMonitorTraceExporter(options =>
    {
        options.ConnectionString = config.ApplicationInsightsConnectionString;
    });
}
else
{
    ConsoleTable.Warning("APPLICATIONINSIGHTS_CONNECTION_STRING is not set. This run will still call Foundry, but Azure Monitor evidence will not be exported.");
}

using var tracerProvider = tracerBuilder.Build();

ConsoleTable.Header("Day 5 Lab 01 - LLMOps and GenAIOps Observability Baseline");
ConsoleTable.Row("Endpoint", config.AzureOpenAiEndpoint);
ConsoleTable.Row("Deployment", config.AzureOpenAiChatDeployment);
ConsoleTable.Row("Batch", config.BatchId);
ConsoleTable.Row("Student", config.StudentId);

if (string.IsNullOrWhiteSpace(config.AzureOpenAiApiKey) &&
    string.IsNullOrWhiteSpace(config.AzureOpenAiBearerToken))
{
    ConsoleTable.Warning("Set AZURE_OPENAI_API_KEY or AZURE_OPENAI_BEARER_TOKEN before running this lab.");
    Console.WriteLine("This lab is not complete without a live Foundry/Azure OpenAI call.");
ConsoleTable.ApplicationEnd(01);
    Environment.ExitCode = 1;
    return;
}

using var operation = activitySource.StartActivity(
    "ai.native.llmops.genaiops.live_foundry_request",
    ActivityKind.Client);

operation?.SetTag("BatchId", config.BatchId);
operation?.SetTag("StudentId", config.StudentId);
operation?.SetTag("EnvironmentId", config.EnvironmentId);
operation?.SetTag("CostCenter", config.CostCenter);
operation?.SetTag("ai.system", "azure-foundry");
operation?.SetTag("ai.operation.type", "chat.completions");
operation?.SetTag("ai.model.deployment", config.AzureOpenAiChatDeployment);

var requestBody = new
{
    model = config.AzureOpenAiChatDeployment,
    messages = new[]
    {
        new
        {
            role = "system",
            content = "You are an AI operations assistant. Answer with concise operational evidence points."
        },
        new
        {
            role = "user",
            content = "For a production AI agent, what telemetry should we capture for LLMOps and GenAIOps?"
        }
    },
    max_completion_tokens = 220
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
    using var foundryCall = activitySource.StartActivity("foundry.chat.completions", ActivityKind.Client);
    foundryCall?.SetTag("server.address", new Uri(config.AzureOpenAiEndpoint).Host);
    foundryCall?.SetTag("ai.model.deployment", config.AzureOpenAiChatDeployment);

    using var response = await client.SendAsync(request);
    var responseText = await response.Content.ReadAsStringAsync();
    var elapsed = Stopwatch.GetElapsedTime(started);

    foundryCall?.SetTag("http.response.status_code", (int)response.StatusCode);
    foundryCall?.SetTag("ai.latency_ms", elapsed.TotalMilliseconds);
    operation?.SetTag("http.response.status_code", (int)response.StatusCode);
    operation?.SetTag("ai.latency_ms", elapsed.TotalMilliseconds);

    ConsoleTable.Header("Live Foundry Result");
    ConsoleTable.Row("Status", $"{(int)response.StatusCode} {response.ReasonPhrase}");
    ConsoleTable.Row("LatencyMs", $"{elapsed.TotalMilliseconds:F0}");

    if (!response.IsSuccessStatusCode)
    {
        foundryCall?.SetStatus(ActivityStatusCode.Error, response.ReasonPhrase);
        operation?.SetStatus(ActivityStatusCode.Error, response.ReasonPhrase);
        Console.WriteLine(OpenAiResponse.TryPrettyJson(responseText));
        Console.WriteLine("================================================================================");
        Console.WriteLine("                     Application 01 End");
        Console.WriteLine("================================================================================");
        Environment.ExitCode = 1;
        return;
    }

    var usage = OpenAiResponse.ExtractUsage(responseText);
    foundryCall?.SetTag("ai.prompt.tokens", usage.PromptTokens);
    foundryCall?.SetTag("ai.completion.tokens", usage.CompletionTokens);
    foundryCall?.SetTag("ai.total.tokens", usage.TotalTokens);
    operation?.SetTag("ai.prompt.tokens", usage.PromptTokens);
    operation?.SetTag("ai.completion.tokens", usage.CompletionTokens);
    operation?.SetTag("ai.total.tokens", usage.TotalTokens);

    ConsoleTable.Header("Model Answer");
    Console.WriteLine(OpenAiResponse.ExtractChatAnswerOrRawJson(responseText));

    var evt = OperationalEvent.FromConfig(
        config,
        eventName: "lab01.live_foundry_observability_completed",
        agentName: "day05-observability-client",
        modelDeployment: config.AzureOpenAiChatDeployment,
        backend: config.AzureOpenAiEndpoint,
        latency: elapsed) with
    {
        PromptTokens = usage.PromptTokens,
        CompletionTokens = usage.CompletionTokens,
        TotalTokens = usage.TotalTokens
    };

    ConsoleTable.Header("Operational Event");
    evt.WriteToConsole();

    Console.WriteLine();
    Console.WriteLine("Log Analytics / Application Insights evidence to verify:");
    Console.WriteLine("- service.name == day05-lab01-llmops-genaiops-observability");
    Console.WriteLine("- BatchId, StudentId, ai.model.deployment, ai.total.tokens");
    Console.WriteLine("- operation name: ai.native.llmops.genaiops.live_foundry_request");

ConsoleTable.ApplicationEnd(01);
}
catch (Exception ex)
{
    var elapsed = Stopwatch.GetElapsedTime(started);
    operation?.SetStatus(ActivityStatusCode.Error, ex.Message);

    var evt = OperationalEvent.FromConfig(
        config,
        eventName: "lab01.live_foundry_observability_failed",
        agentName: "day05-observability-client",
        modelDeployment: config.AzureOpenAiChatDeployment,
        backend: config.AzureOpenAiEndpoint,
        latency: elapsed,
        success: false,
        errorType: ex.GetType().Name,
        errorMessage: ex.Message);

    ConsoleTable.Header("Operational Event");
    evt.WriteToConsole();
ConsoleTable.ApplicationEnd(01);
    Environment.ExitCode = 1;
}
