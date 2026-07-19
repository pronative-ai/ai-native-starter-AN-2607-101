using System.Text.Json;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Pronative.Day05.Shared;

ConsoleTable.ApplicationStart(04);

var config = Day05Config.Load();

ConsoleTable.Header("Day 5 Lab 04 - AI FinOps Evidence");
ConsoleTable.Row("Batch", config.BatchId);
ConsoleTable.Row("Environment", config.EnvironmentId);
ConsoleTable.Row("Subscription", string.IsNullOrWhiteSpace(config.AzureSubscriptionId) ? "<default subscription>" : config.AzureSubscriptionId);
ConsoleTable.Row("LookbackDays", config.EvidenceLookbackDays.ToString());

var credential = new DefaultAzureCredential();
var arm = new ArmClient(credential);
var subscription = await ResolveSubscriptionAsync(arm, config.AzureSubscriptionId);
var subscriptionId = subscription.Id.SubscriptionId;

if (string.IsNullOrWhiteSpace(subscriptionId))
{
    ConsoleTable.Warning("Could not resolve a subscription ID. Set AZURE_SUBSCRIPTION_ID and run az login.");
    Console.WriteLine("================================================================================");
    Console.WriteLine("                     Application 04 End");
    Console.WriteLine("================================================================================");
    Environment.ExitCode = 1;
    return;
}

ConsoleTable.Row("Resolved subscription", subscriptionId);

await PrintAzureCostEvidenceAsync(subscriptionId, config);
await PrintModelUsageEvidenceAsync(config);

ConsoleTable.Header("AI FinOps Interpretation");
Console.WriteLine("AI FinOps combines:");
Console.WriteLine("- Azure service cost by resource group/service.");
Console.WriteLine("- Model usage: prompt tokens, completion tokens, total tokens, latency, failures.");
Console.WriteLine("- Runtime posture: always-on resources, gateway, AKS, GPU/neocloud workers, and weekend shutdown.");
Console.WriteLine("- Budget control: AN-2607-101 should remain inside the agreed INR 20,000 batch ceiling.");

ConsoleTable.ApplicationEnd(04);

static async Task<SubscriptionResource> ResolveSubscriptionAsync(ArmClient arm, string configuredSubscriptionId)
{
    if (!string.IsNullOrWhiteSpace(configuredSubscriptionId))
    {
        var subscriptionResourceId = SubscriptionResource.CreateResourceIdentifier(configuredSubscriptionId);
        return arm.GetSubscriptionResource(subscriptionResourceId);
    }

    return await arm.GetDefaultSubscriptionAsync();
}

static async Task PrintAzureCostEvidenceAsync(string subscriptionId, Day05Config config)
{
    var now = DateTimeOffset.UtcNow;
    var from = now.AddDays(-config.EvidenceLookbackDays);
    var url = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.CostManagement/query?api-version=2025-03-01";

    var body = new
    {
        type = "ActualCost",
        timeframe = "Custom",
        timePeriod = new
        {
            from = from.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            to = now.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
        },
        dataset = new
        {
            granularity = "None",
            aggregation = new
            {
                totalCost = new
                {
                    name = "PreTaxCost",
                    function = "Sum"
                }
            },
            grouping = new object[]
            {
                new { type = "Dimension", name = "ResourceGroupName" },
                new { type = "Dimension", name = "ServiceName" }
            }
        }
    };

    ConsoleTable.Header("Azure Cost Management Evidence");
    Console.WriteLine($"Query: {from:yyyy-MM-dd} to {now:yyyy-MM-dd}");

    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
    using var request = await AzureRestClient.CreateJsonRequestAsync(
        HttpMethod.Post,
        url,
        "https://management.azure.com/.default",
        body);

    using var response = await client.SendAsync(request);
    var responseText = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        ConsoleTable.Warning($"Cost query failed: {(int)response.StatusCode} {response.ReasonPhrase}");
        Console.WriteLine(OpenAiResponse.TryPrettyJson(responseText));
        return;
    }

    var rows = ParseCostRows(responseText)
        .OrderByDescending(r => r.Cost)
        .Take(15)
        .ToList();

    if (rows.Count == 0)
    {
        Console.WriteLine("No cost rows found for the subscription. Cost data may take 24-48 hours to appear.");
        return;
    }

    foreach (var row in rows)
    {
        Console.WriteLine($"{row.ResourceGroup,-42} {row.ServiceName,-36} {row.Cost,10:F4} {row.Currency}");
    }
}

static async Task PrintModelUsageEvidenceAsync(Day05Config config)
{
    ConsoleTable.Header("Model / Gateway Usage Evidence From Log Analytics");

    if (string.IsNullOrWhiteSpace(config.LogAnalyticsWorkspaceId))
    {
        ConsoleTable.Warning("LOG_ANALYTICS_WORKSPACE_ID is not set. Run Lab 01-03 with App Insights/Log Analytics configured, then rerun Lab 04.");
        return;
    }

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
    StudentId = coalesce(tostring(cd.StudentId), column_ifexists("StudentId", "")),
    ModelDeployment = coalesce(tostring(cd['ai.model.deployment']), tostring(cd.ModelDeployment), column_ifexists("ModelDeployment", "")),
    Backend = coalesce(tostring(cd.Backend), column_ifexists("target", ""), column_ifexists("url", ""), column_ifexists("Backend", "")),
    GatewayRoute = coalesce(tostring(cd.GatewayRoute), column_ifexists("GatewayRoute", "")),
    PromptTokens = toint(coalesce(cd['ai.prompt.tokens'], cd.PromptTokens)),
    CompletionTokens = toint(coalesce(cd['ai.completion.tokens'], cd.CompletionTokens)),
    TotalTokens = toint(coalesce(cd['ai.total.tokens'], cd.TotalTokens, column_ifexists("TotalTokens", "0")))
| where BatchId == batchId or BatchId has batchId
| summarize Events=count(),
            PromptTokens=sum(PromptTokens),
            CompletionTokens=sum(CompletionTokens),
            TotalTokens=sum(TotalTokens),
            Failures=countif(column_ifexists("success", true) == false)
    by ModelDeployment, Backend, GatewayRoute, itemType
| order by TotalTokens desc, Events desc
""";

    var payload = new
    {
        query = kql,
        timespan = $"P{config.EvidenceLookbackDays}D"
    };

    var url = $"https://api.loganalytics.azure.com/v1/workspaces/{config.LogAnalyticsWorkspaceId}/query";
    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
    using var request = await AzureRestClient.CreateJsonRequestAsync(
        HttpMethod.Post,
        url,
        "https://api.loganalytics.io/.default",
        payload);

    using var response = await client.SendAsync(request);
    var responseText = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        ConsoleTable.Warning($"Log Analytics query failed: {(int)response.StatusCode} {response.ReasonPhrase}");
        Console.WriteLine(OpenAiResponse.TryPrettyJson(responseText));
        return;
    }

    var rows = ParseLogAnalyticsRows(responseText).Take(15).ToList();
    if (rows.Count == 0)
    {
        Console.WriteLine("No model/gateway telemetry rows found. Run Labs 01-03 first and confirm telemetry export.");
        return;
    }

    foreach (var row in rows)
    {
        Console.WriteLine($"{row.ModelDeployment,-18} events={row.Events,-4} totalTokens={row.TotalTokens,-8} failures={row.Failures,-3} route={row.GatewayRoute}");
    }
}

static IEnumerable<CostRow> ParseCostRows(string json)
{
    using var doc = JsonDocument.Parse(json);
    var properties = doc.RootElement.GetProperty("properties");
    var columns = properties.GetProperty("columns").EnumerateArray().Select(c => c.GetProperty("name").GetString() ?? "").ToList();
    var rows = properties.GetProperty("rows").EnumerateArray();

    var costIndex = IndexOf(columns, "PreTaxCost", "Cost", "totalCost");
    var resourceGroupIndex = IndexOf(columns, "ResourceGroupName", "ResourceGroup");
    var serviceIndex = IndexOf(columns, "ServiceName", "Service");
    var currencyIndex = IndexOf(columns, "Currency");

    foreach (var row in rows)
    {
        var values = row.EnumerateArray().ToList();
        yield return new CostRow(
            GetString(values, resourceGroupIndex),
            GetString(values, serviceIndex),
            GetDecimal(values, costIndex),
            GetString(values, currencyIndex));
    }
}

static IEnumerable<ModelUsageRow> ParseLogAnalyticsRows(string json)
{
    using var doc = JsonDocument.Parse(json);
    if (!doc.RootElement.TryGetProperty("tables", out var tables) || tables.GetArrayLength() == 0)
    {
        yield break;
    }

    var table = tables[0];
    var columns = table.GetProperty("columns").EnumerateArray().Select(c => c.GetProperty("name").GetString() ?? "").ToList();
    var modelIndex = IndexOf(columns, "ModelDeployment");
    var backendIndex = IndexOf(columns, "Backend");
    var routeIndex = IndexOf(columns, "GatewayRoute");
    var eventsIndex = IndexOf(columns, "Events");
    var tokensIndex = IndexOf(columns, "TotalTokens");
    var failuresIndex = IndexOf(columns, "Failures");

    foreach (var row in table.GetProperty("rows").EnumerateArray())
    {
        var values = row.EnumerateArray().ToList();
        yield return new ModelUsageRow(
            GetString(values, modelIndex),
            GetString(values, backendIndex),
            GetString(values, routeIndex),
            GetInt(values, eventsIndex),
            GetInt(values, tokensIndex),
            GetInt(values, failuresIndex));
    }
}

static int IndexOf(IReadOnlyList<string> columns, params string[] names)
{
    foreach (var name in names)
    {
        for (var index = 0; index < columns.Count; index++)
        {
            if (columns[index].Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
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

static decimal GetDecimal(IReadOnlyList<JsonElement> values, int index)
{
    return index >= 0 && index < values.Count && values[index].TryGetDecimal(out var result) ? result : 0;
}

internal sealed record CostRow(string ResourceGroup, string ServiceName, decimal Cost, string Currency);

internal sealed record ModelUsageRow(
    string ModelDeployment,
    string Backend,
    string GatewayRoute,
    int Events,
    int TotalTokens,
    int Failures);
