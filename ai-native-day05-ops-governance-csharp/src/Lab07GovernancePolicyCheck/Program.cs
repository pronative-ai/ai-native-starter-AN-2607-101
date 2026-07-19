using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Pronative.Day05.Shared;

ConsoleTable.ApplicationStart(07);

const string activitySourceName = "Pronative.Day05.Lab07";
var activitySource = new ActivitySource(activitySourceName);

var tracerBuilder = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
        serviceName: "day05-lab07-governance-policy-check",
        serviceVersion: "2.0.0"))
    .AddSource(activitySourceName);

var config = Day05Config.Load();

ConsoleTable.Header("Day 5 Lab 07 - AI Agent Governance Evidence and Policy Decisions");
ConsoleTable.Row("Batch", config.BatchId);
ConsoleTable.Row("Student", config.StudentId);
ConsoleTable.Row("Workspace", string.IsNullOrWhiteSpace(config.LogAnalyticsWorkspaceId) ? "<not set>" : config.LogAnalyticsWorkspaceId);
ConsoleTable.Row("LookbackDays", config.EvidenceLookbackDays.ToString());

if (!string.IsNullOrWhiteSpace(config.ApplicationInsightsConnectionString))
{
    tracerBuilder.AddAzureMonitorTraceExporter(options =>
    {
        options.ConnectionString = config.ApplicationInsightsConnectionString;
    });
}
else
{
    ConsoleTable.Warning("APPLICATIONINSIGHTS_CONNECTION_STRING is not set. Governance decisions will not be exported to App Insights.");
}

var tracerProvider = tracerBuilder.Build();

if (string.IsNullOrWhiteSpace(config.LogAnalyticsWorkspaceId))
{
    ConsoleTable.Warning("LOG_ANALYTICS_WORKSPACE_ID is not set.");
    Console.WriteLine("Set LOG_ANALYTICS_WORKSPACE_ID in .env and re-run.");
    Console.WriteLine("================================================================================");
    Console.WriteLine("                     Application 07 End");
    Console.WriteLine("================================================================================");
    Environment.ExitCode = 1;
    return;
}

var operations = await QueryObservedOperationsAsync(config);

if (operations.Count == 0)
{
    ConsoleTable.Warning("No telemetry rows found for this batch in Log Analytics or Application Insights.");
    Console.WriteLine();
    Console.WriteLine("Run Labs 01-06 first to generate operational telemetry, then re-run Lab 07.");
    Console.WriteLine("================================================================================");
    Console.WriteLine("                     Application 07 End");
    Console.WriteLine("================================================================================");
    tracerProvider.Dispose();
    return;
}

var policy = new GovernancePolicy(config);

ConsoleTable.Header("Governance Decisions From Live Evidence");
foreach (var operation in operations)
{
    var decision = policy.Evaluate(operation);
    Console.WriteLine($"{operation.Timestamp:u} {decision.Outcome,-16} {operation.EventName,-42} {operation.StudentId,-14} {decision.Reason}");

    using var activity = activitySource.StartActivity("ai.native.governance.policy_decision", ActivityKind.Internal);
    activity?.SetTag("BatchId", operation.BatchId);
    activity?.SetTag("StudentId", operation.StudentId);
    activity?.SetTag("EventName", operation.EventName);
    activity?.SetTag("TraceId", operation.TraceId);
    activity?.SetTag("Backend", operation.Backend);
    activity?.SetTag("GatewayRoute", operation.GatewayRoute);
    activity?.SetTag("ModelDeployment", operation.ModelDeployment);
    activity?.SetTag("ai.total.tokens", operation.TotalTokens);
    activity?.SetTag("RuntimeSuccess", operation.Success);
    activity?.SetTag("governance.outcome", decision.Outcome);
    activity?.SetTag("governance.reason", decision.Reason);
}

ConsoleTable.Header("Enterprise Governance Interpretation");
Console.WriteLine("Citadel lens: this evidence belongs in the AI Control Plane and Security Fabric.");
Console.WriteLine("Agent Governance Toolkit lens: policy decisions should happen before risky tool/runtime actions execute.");
Console.WriteLine("Agent 365 lens: inventory, ownership, lifecycle, and admin visibility need this kind of evidence.");

ConsoleTable.ApplicationEnd(07);
tracerProvider.ForceFlush(10000);
tracerProvider.Dispose();

static async Task<List<ObservedOperation>> QueryObservedOperationsAsync(Day05Config config)
{
    var kql = $$"""
let lookback = {{config.EvidenceLookbackDays}}d;
let batchId = '{{config.BatchId}}';
let allData = union isfuzzy=true
    (traces   | where timestamp > ago(lookback) | extend itemType = "traces"),
    (requests | where timestamp > ago(lookback) | extend itemType = "requests"),
    (dependencies | where timestamp > ago(lookback) | extend itemType = "dependencies"),
    (customEvents | where timestamp > ago(lookback) | extend itemType = "customEvents"),
    (AppTraces    | where TimeGenerated > ago(lookback) | extend itemType = "AppTraces"),
    (AppRequests  | where TimeGenerated > ago(lookback) | extend itemType = "AppRequests"),
    (AppDependencies | where TimeGenerated > ago(lookback) | extend itemType = "AppDependencies"),
    (AppEvents    | where TimeGenerated > ago(lookback) | extend itemType = "AppEvents");
allData
| extend cd = todynamic(column_ifexists("customDimensions", "{}"))
| extend cd2 = todynamic(column_ifexists("Properties", "{}"))
| extend cd3 = todynamic(column_ifexists("properties", "{}"))
| extend cd = coalesce(cd, cd2, cd3)
| extend
    BatchId = coalesce(tostring(cd.BatchId), column_ifexists("BatchId", ""), column_ifexists("batch_id", "")),
    StudentId = tostring(cd.StudentId),
    EventName = coalesce(tostring(cd.EventName), column_ifexists("name", "")),
    Backend = coalesce(tostring(cd.Backend), column_ifexists("target", ""), column_ifexists("url", ""), column_ifexists("Backend", "")),
    GatewayRoute = tostring(cd.GatewayRoute),
    ModelDeployment = coalesce(tostring(cd['ai.model.deployment']), tostring(cd.ModelDeployment), column_ifexists("ModelDeployment", "")),
    TotalTokens = toint(coalesce(cd['ai.total.tokens'], cd.TotalTokens, column_ifexists("TotalTokens", "0"))),
    RuntimeSuccess = coalesce(tobool(cd.Success), tobool(column_ifexists("success", "true")), true),
    TraceId = coalesce(tostring(cd.TraceId), column_ifexists("operation_Id", ""))
| where BatchId == batchId or BatchId has batchId
| extend Timestamp = coalesce(todatetime(column_ifexists("timestamp", datetime(null))), todatetime(column_ifexists("TimeGenerated", datetime(null))))
| project Timestamp, EventName, BatchId, StudentId, TraceId, Backend, GatewayRoute, ModelDeployment, TotalTokens, RuntimeSuccess
| where isnotempty(Timestamp)
| order by Timestamp desc
| take 50
""";

    var credential = new AzureCliCredential();

    if (!string.IsNullOrWhiteSpace(config.LogAnalyticsWorkspaceId))
    {
        var laResult = await QueryLogAnalyticsAsync(config, kql, credential);
        if (laResult.Count > 0) return laResult;
    }

    var appId = ExtractAppId(config.ApplicationInsightsConnectionString);
    if (!string.IsNullOrEmpty(appId))
    {
        var aiResult = await QueryAppInsightsAsync(appId, kql, credential);
        if (aiResult.Count > 0) return aiResult;
    }

    return new List<ObservedOperation>();
}

static async Task<List<ObservedOperation>> QueryLogAnalyticsAsync(Day05Config config, string kql, AzureCliCredential credential)
{
    var payload = new { query = kql, timespan = $"P{config.EvidenceLookbackDays}D" };
    var url = $"https://api.loganalytics.azure.com/v1/workspaces/{config.LogAnalyticsWorkspaceId}/query";

    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
    var token = await credential.GetTokenAsync(new TokenRequestContext(new[] { "https://api.loganalytics.io/.default" }));

    using var request = new HttpRequestMessage(HttpMethod.Post, url);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
    request.Content = new StringContent(JsonSerializer.Serialize(payload, OperationalEvent.JsonOptions), Encoding.UTF8, "application/json");

    using var response = await client.SendAsync(request);
    if (!response.IsSuccessStatusCode) return new List<ObservedOperation>();
    return ParseObservedOperations(await response.Content.ReadAsStringAsync());
}

static async Task<List<ObservedOperation>> QueryAppInsightsAsync(string appId, string kql, AzureCliCredential credential)
{
    var url = $"https://api.applicationinsights.io/v1/apps/{appId}/query?query={Uri.EscapeDataString(kql)}";

    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
    var token = await credential.GetTokenAsync(new TokenRequestContext(new[] { "https://api.applicationinsights.io/.default" }));

    using var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

    using var response = await client.SendAsync(request);
    if (!response.IsSuccessStatusCode) return new List<ObservedOperation>();
    return ParseObservedOperations(await response.Content.ReadAsStringAsync());
}

static string ExtractAppId(string connectionString)
{
    foreach (var part in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries))
    {
        var trimmed = part.Trim();
        if (trimmed.StartsWith("ApplicationId=", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed["ApplicationId=".Length..];
        }
    }

    return "";
}

static List<ObservedOperation> ParseObservedOperations(string json)
{
    using var doc = JsonDocument.Parse(json);
    if (!doc.RootElement.TryGetProperty("tables", out var tables) || tables.GetArrayLength() == 0)
    {
        return new List<ObservedOperation>();
    }

    var table = tables[0];
    var columns = table.GetProperty("columns").EnumerateArray().Select(c => c.GetProperty("name").GetString() ?? "").ToList();

    var timestampIndex = IndexOf(columns, "timestamp");
    var eventIndex = IndexOf(columns, "EventName");
    var batchIndex = IndexOf(columns, "BatchId");
    var studentIndex = IndexOf(columns, "StudentId");
    var traceIndex = IndexOf(columns, "TraceId");
    var backendIndex = IndexOf(columns, "Backend");
    var routeIndex = IndexOf(columns, "GatewayRoute");
    var modelIndex = IndexOf(columns, "ModelDeployment");
    var tokensIndex = IndexOf(columns, "TotalTokens");
    var successIndex = IndexOf(columns, "RuntimeSuccess");

    var result = new List<ObservedOperation>();
    foreach (var row in table.GetProperty("rows").EnumerateArray())
    {
        var values = row.EnumerateArray().ToList();
        result.Add(new ObservedOperation(
            GetDateTime(values, timestampIndex),
            GetString(values, eventIndex),
            GetString(values, batchIndex),
            GetString(values, studentIndex),
            GetString(values, traceIndex),
            GetString(values, backendIndex),
            GetString(values, routeIndex),
            GetString(values, modelIndex),
            GetInt(values, tokensIndex),
            GetBool(values, successIndex)));
    }

    return result;
}

static int IndexOf(IReadOnlyList<string> columns, string name)
{
    for (var index = 0; index < columns.Count; index++)
    {
        if (columns[index].Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            return index;
        }
    }

    return -1;
}

static string GetString(IReadOnlyList<JsonElement> values, int index)
{
    return index >= 0 && index < values.Count ? values[index].ToString() : "";
}

static int GetInt(IReadOnlyList<JsonElement> values, int index)
{
    return index >= 0 && index < values.Count && values[index].TryGetInt32(out var result) ? result : 0;
}

static bool GetBool(IReadOnlyList<JsonElement> values, int index)
{
    if (index < 0 || index >= values.Count || values[index].ValueKind == JsonValueKind.Null)
    {
        return true;
    }

    return values[index].ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.String => bool.TryParse(values[index].GetString(), out var result) ? result : true,
        _ => true
    };
}

static DateTimeOffset GetDateTime(IReadOnlyList<JsonElement> values, int index)
{
    return index >= 0 &&
           index < values.Count &&
           values[index].TryGetDateTimeOffset(out var result)
        ? result
        : DateTimeOffset.MinValue;
}

internal sealed record ObservedOperation(
    DateTimeOffset Timestamp,
    string EventName,
    string BatchId,
    string StudentId,
    string TraceId,
    string Backend,
    string GatewayRoute,
    string ModelDeployment,
    int TotalTokens,
    bool Success);

internal sealed record GovernanceDecision(string Outcome, string Reason);

internal sealed class GovernancePolicy(Day05Config config)
{
    public GovernanceDecision Evaluate(ObservedOperation operation)
    {
        if (!operation.Success)
        {
            return new GovernanceDecision("Review", "failed operation needs investigation before reuse");
        }

        if (string.IsNullOrWhiteSpace(operation.TraceId) ||
            string.IsNullOrWhiteSpace(operation.StudentId))
        {
            return new GovernanceDecision("Review", "missing trace or student attribution");
        }

        if (operation.Backend.Contains("runpod", StringComparison.OrdinalIgnoreCase))
        {
            return new GovernanceDecision("RequireApproval", "external neocloud runtime requires explicit approval and cost controls");
        }

        if (!string.IsNullOrWhiteSpace(operation.Backend) &&
            IsExternalNonApprovedBackend(operation.Backend))
        {
            return new GovernanceDecision("Deny", "backend is outside approved Azure/Runpod runtime boundary");
        }

        if (!string.IsNullOrWhiteSpace(operation.ModelDeployment) &&
            !operation.ModelDeployment.Equals(config.AzureOpenAiChatDeployment, StringComparison.OrdinalIgnoreCase) &&
            !operation.ModelDeployment.Equals(config.RunpodModel, StringComparison.OrdinalIgnoreCase))
        {
            return new GovernanceDecision("Review", "model deployment is not in the approved training allowlist");
        }

        if (!string.IsNullOrWhiteSpace(operation.GatewayRoute))
        {
            return new GovernanceDecision("Allow", "gateway-routed operation has trace and batch attribution");
        }

        return new GovernanceDecision("Allow", "observed operation is inside approved training boundary");
    }

    private static bool IsExternalNonApprovedBackend(string backend)
    {
        return backend.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
               !backend.Contains("azure.com", StringComparison.OrdinalIgnoreCase) &&
               !backend.Contains("openai.azure.com", StringComparison.OrdinalIgnoreCase) &&
               !backend.Contains("runpod", StringComparison.OrdinalIgnoreCase);
    }
}
