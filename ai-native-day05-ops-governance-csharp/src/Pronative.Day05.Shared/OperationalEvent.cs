using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pronative.Day05.Shared;

public sealed record OperationalEvent
{
    public required string EventName { get; init; }
    public required string BatchId { get; init; }
    public required string StudentId { get; init; }
    public string EnvironmentId { get; init; } = "an2607101";
    public string? AgentName { get; init; }
    public string? ModelDeployment { get; init; }
    public string? GatewayRoute { get; init; }
    public string? Backend { get; init; }
    public string? OperationId { get; init; }
    public string? TraceId { get; init; }
    public int? PromptTokens { get; init; }
    public int? CompletionTokens { get; init; }
    public int? TotalTokens { get; init; }
    public long? LatencyMs { get; init; }
    public bool Success { get; init; } = true;
    public string? ErrorType { get; init; }
    public string? ErrorMessage { get; init; }
    public IDictionary<string, string> Tags { get; init; } = new Dictionary<string, string>();

    public static OperationalEvent FromConfig(
        Day05Config config,
        string eventName,
        string? agentName = null,
        string? modelDeployment = null,
        string? gatewayRoute = null,
        string? backend = null,
        TimeSpan? latency = null,
        bool success = true,
        string? errorType = null,
        string? errorMessage = null)
    {
        var activity = Activity.Current;
        return new OperationalEvent
        {
            EventName = eventName,
            BatchId = config.BatchId,
            StudentId = config.StudentId,
            EnvironmentId = config.EnvironmentId,
            AgentName = agentName,
            ModelDeployment = modelDeployment,
            GatewayRoute = gatewayRoute,
            Backend = backend,
            OperationId = activity?.SpanId.ToString(),
            TraceId = activity?.TraceId.ToString(),
            LatencyMs = latency is null ? null : (long)latency.Value.TotalMilliseconds,
            Success = success,
            ErrorType = errorType,
            ErrorMessage = errorMessage,
            Tags = config.StandardTags()
        };
    }

    public void WriteToConsole()
    {
        var json = JsonSerializer.Serialize(this, JsonOptions);
        Console.WriteLine(json);
    }

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
