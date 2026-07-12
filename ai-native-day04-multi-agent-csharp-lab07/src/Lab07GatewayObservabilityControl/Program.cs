using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Pronative.MultiAgentTraining.Shared;

Day04Console.PrintAppStart();

DotEnvLoader.Load();
var context = GatewayObservabilityContext.From(MultiAgentOptions.FromEnvironment(), args);
var liveMode = args.Contains("--live", StringComparer.OrdinalIgnoreCase)
    || string.Equals(Environment.GetEnvironmentVariable("PN_AGENTGATEWAY_LIVE"), "true", StringComparison.OrdinalIgnoreCase);

ConsoleFormatting.Header("Day 4 Lab 07 - Gateway Observability and Control");
Console.WriteLine("Official capability: AgentGateway rate limiting, request attribution, trace/log emission, and Azure Monitor/App Insights queryability.");
Console.WriteLine("Package/API availability: AgentGateway is not a .NET SDK surface. This lab uses official gateway configuration, HTTP traffic, and Azure Monitor KQL artifacts.");
Console.WriteLine($"Gateway endpoint: {context.GatewayEndpoint}");
Console.WriteLine($"Mode: {(liveMode ? "live gateway calls" : "dry run request preparation")}");

ConsoleFormatting.Header("Lab 07 Component Contract");
Console.WriteLine("- Official capability: AgentGateway traffic control and observability.");
Console.WriteLine("- Package: none for .NET; AgentGateway standalone config plus Azure Monitor/App Insights are the official surfaces.");
Console.WriteLine("- Required config evidence: localRateLimit, requestHeaderModifier, backendAuth.azure.explicitConfig.managedIdentity, config.tracing, config.logging.");
Console.WriteLine("- Required code evidence: x-request-id, x-correlation-id, traceparent, BatchId, StudentId, LabId, route/model headers, and live status handling.");
Console.WriteLine("- Forbidden substitutes: no fake gateway response, no custom rate limiter, no simulated App Insights data.");

ConsoleFormatting.Header("Observation Plan");
Console.WriteLine("1. Baseline request: verify a normal gateway model request and capture x-request-id.");
Console.WriteLine("2. Burst requests: send a short burst to exercise localRateLimit when the stricter Lab 07 route is deployed.");
Console.WriteLine("3. Failure probe: send a route-not-found request so students can inspect failure status and response details.");
Console.WriteLine("4. Query Azure Monitor/App Insights using the KQL files in observability/kql.");
Console.WriteLine();
Console.WriteLine("KQL files:");
Console.WriteLine("- observability/kql/01-recent-gateway-console-logs.kql");
Console.WriteLine("- observability/kql/02-rate-limited-requests.kql");
Console.WriteLine("- observability/kql/03-http-status-latency.kql");
Console.WriteLine("- observability/kql/04-trace-single-request.kql");
Console.WriteLine("- observability/kql/05-model-token-usage.kql");

var requestPlans = GatewayRequestPlan.Build(context).ToArray();

Day04Console.PrintLabStart(7);

ConsoleFormatting.Header("Prepared Gateway Requests");
foreach (var plan in requestPlans)
{
    PrintPlan(plan);
}

if (!liveMode)
{
    ConsoleFormatting.Header("Dry Run Complete");
    Console.WriteLine("No network call was made.");
    Console.WriteLine("To send traffic through the deployed gateway, set PN_AGENTGATEWAY_LIVE=true or pass --live.");
    Day04Console.PrintLabEnd(7);
    Day04Console.PrintAppEnd();
    return;
}

ConsoleFormatting.Header("Live Gateway Run");
var results = await SendRequestsAsync(requestPlans, CancellationToken.None);

ConsoleFormatting.Header("Run Summary");
foreach (var result in results)
{
    Console.WriteLine($"{result.Name} | Attempt {result.Attempt} | HTTP {(int?)result.StatusCode ?? 0} {result.StatusCode} | RequestId {result.RequestId} | {result.DurationMs} ms");
}

var outputPath = Path.GetFullPath($"gateway-observability-results-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.json");
await File.WriteAllTextAsync(outputPath, JsonSerializer.Serialize(results, GatewayJson.Options));
Console.WriteLine();
Console.WriteLine($"Saved live run evidence: {outputPath}");

Day04Console.PrintLabEnd(7);

Day04Console.PrintAppEnd();

static async Task<IReadOnlyList<GatewayRunResult>> SendRequestsAsync(IEnumerable<GatewayRequestPlan> requestPlans, CancellationToken cancellationToken)
{
    using var httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(60)
    };

    var results = new List<GatewayRunResult>();

    foreach (var plan in requestPlans)
    {
        using var request = plan.CreateHttpRequestMessage();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            stopwatch.Stop();

            var trimmedBody = TrimForConsole(body);
            Console.WriteLine();
            Console.WriteLine($"{plan.Name} attempt {plan.Attempt}: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.WriteLine($"RequestId: {plan.RequestId}");
            Console.WriteLine($"CorrelationId: {plan.CorrelationId}");
            Console.WriteLine(trimmedBody);

            results.Add(new GatewayRunResult(
                plan.Name,
                plan.Attempt,
                plan.RequestId,
                plan.CorrelationId,
                plan.TraceParent,
                response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                trimmedBody));
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"{plan.Name} attempt {plan.Attempt}: request failed");
            Console.WriteLine(ex.Message);

            results.Add(new GatewayRunResult(
                plan.Name,
                plan.Attempt,
                plan.RequestId,
                plan.CorrelationId,
                plan.TraceParent,
                null,
                stopwatch.ElapsedMilliseconds,
                ex.Message));
        }
    }

    return results;
}

static void PrintPlan(GatewayRequestPlan plan)
{
    Console.WriteLine();
    Console.WriteLine($"{plan.Name} attempt {plan.Attempt}");
    Console.WriteLine($"Expected observation: {plan.ExpectedObservation}");
    Console.WriteLine($"{plan.Method} {plan.RequestUri}");
    foreach (var header in plan.Headers)
    {
        Console.WriteLine($"{header.Key}: {MaskIfSensitive(header.Key, header.Value)}");
    }

    Console.WriteLine("Body:");
    Console.WriteLine(plan.Body);
}

static string TrimForConsole(string value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return "<empty>";
    }

    const int limit = 2000;
    return value.Length <= limit ? value : value[..limit] + Environment.NewLine + "... trimmed ...";
}

static string MaskIfSensitive(string key, string value)
{
    if (key.Contains("authorization", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("api-key", StringComparison.OrdinalIgnoreCase))
    {
        return "<set>";
    }

    return value;
}

public sealed record GatewayObservabilityContext(
    string BatchId,
    string StudentId,
    string LabId,
    string CostCenter,
    string GatewayEndpoint,
    string ModelPath,
    string FailurePath,
    string ModelDeployment,
    int BurstCount)
{
    public static GatewayObservabilityContext From(MultiAgentOptions options, string[] args) => new(
        options.BatchId,
        options.StudentId,
        Env("PN_LAB_ID", "day04-lab07"),
        Env("PN_COST_CENTER", "ProNative-AI-Native-Training"),
        Env("PN_AGENTGATEWAY_ENDPOINT", options.GatewayEndpoint).TrimEnd('/'),
        Env("PN_AGENTGATEWAY_MODEL_PATH", "/azure/v1/chat/completions"),
        Env("PN_AGENTGATEWAY_FAILURE_PATH", "/not-configured/day04-lab07"),
        Env("PN_MODEL_DEPLOYMENT", "gpt-5-mini"),
        IntEnv("PN_RATE_LIMIT_BURST_COUNT", args, "--burst-count", 8));

    private static string Env(string name, string fallback) =>
        string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name))
            ? fallback
            : Environment.GetEnvironmentVariable(name)!;

    private static int IntEnv(string envName, string[] args, string argName, int fallback)
    {
        var fromEnv = Environment.GetEnvironmentVariable(envName);
        if (int.TryParse(fromEnv, out var envValue) && envValue > 0)
        {
            return envValue;
        }

        var index = Array.IndexOf(args, argName);
        return index >= 0 && index + 1 < args.Length && int.TryParse(args[index + 1], out var argValue) && argValue > 0
            ? argValue
            : fallback;
    }
}

public sealed record GatewayRequestPlan(
    string Name,
    int Attempt,
    string ExpectedObservation,
    HttpMethod Method,
    string RequestUri,
    IReadOnlyDictionary<string, string> Headers,
    string Body,
    string RequestId,
    string CorrelationId,
    string TraceParent)
{
    public static IEnumerable<GatewayRequestPlan> Build(GatewayObservabilityContext context)
    {
        yield return CreateModelRequest(
            context,
            "Baseline model request",
            1,
            "HTTP 2xx when the model backend is healthy; gateway console logs show route, status, model, duration, and token fields when available.");

        for (var i = 1; i <= context.BurstCount; i++)
        {
            yield return CreateModelRequest(
                context,
                "Rate-limit burst request",
                i,
                "HTTP 2xx until the route bucket is exhausted, then HTTP 429 when the stricter Lab 07 localRateLimit policy is deployed.");
        }

        yield return CreateFailureProbe(context);
    }

    private static GatewayRequestPlan CreateModelRequest(
        GatewayObservabilityContext context,
        string name,
        int attempt,
        string expectedObservation)
    {
        var ids = GatewayTraceIds.Create(context, name, attempt);
        var headers = BuildHeaders(context, ids, "model", "gateway-observability");
        var body = JsonSerializer.Serialize(
            new
            {
                model = context.ModelDeployment,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a concise ProNative AI training operations assistant."
                    },
                    new
                    {
                        role = "user",
                        content = $"Day 4 Lab 07 attempt {attempt}: return one sentence explaining how gateway observability helps control shared model usage."
                    }
                },
                stream = false
            },
            GatewayJson.Options);

        return new GatewayRequestPlan(
            name,
            attempt,
            expectedObservation,
            HttpMethod.Post,
            $"{context.GatewayEndpoint}{context.ModelPath}",
            headers,
            body,
            ids.RequestId,
            ids.CorrelationId,
            ids.TraceParent);
    }

    private static GatewayRequestPlan CreateFailureProbe(GatewayObservabilityContext context)
    {
        var ids = GatewayTraceIds.Create(context, "Failure probe", 1);
        var headers = BuildHeaders(context, ids, "failure-probe", "route-not-found");
        var body = JsonSerializer.Serialize(
            new
            {
                lab = context.LabId,
                purpose = "route-not-found-observation",
                expected = "404 or equivalent gateway route miss response"
            },
            GatewayJson.Options);

        return new GatewayRequestPlan(
            "Failure probe",
            1,
            "HTTP 404 or gateway route-miss outcome; ContainerAppHTTPLogs should show the failing path and response details.",
            HttpMethod.Post,
            $"{context.GatewayEndpoint}{context.FailurePath}",
            headers,
            body,
            ids.RequestId,
            ids.CorrelationId,
            ids.TraceParent);
    }

    private static IReadOnlyDictionary<string, string> BuildHeaders(
        GatewayObservabilityContext context,
        GatewayTraceIds ids,
        string routePurpose,
        string scenario)
    {
        var headers = new Dictionary<string, string>
        {
            ["x-request-id"] = ids.RequestId,
            ["x-correlation-id"] = ids.CorrelationId,
            ["traceparent"] = ids.TraceParent,
            ["x-batch-id"] = context.BatchId,
            ["x-student-id"] = context.StudentId,
            ["x-lab-id"] = context.LabId,
            ["x-cost-center"] = context.CostCenter,
            ["x-route-purpose"] = routePurpose,
            ["x-observation-scenario"] = scenario,
            ["x-model-deployment"] = context.ModelDeployment
        };

        var bearerToken = Environment.GetEnvironmentVariable("PN_AGENTGATEWAY_BEARER_TOKEN");
        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            headers["Authorization"] = $"Bearer {bearerToken}";
        }

        var apiKey = Environment.GetEnvironmentVariable("PN_AGENTGATEWAY_API_KEY");
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            headers["x-api-key"] = apiKey;
        }

        return headers;
    }

    public HttpRequestMessage CreateHttpRequestMessage()
    {
        var request = new HttpRequestMessage(Method, RequestUri)
        {
            Content = new StringContent(Body, Encoding.UTF8, "application/json")
        };

        foreach (var header in Headers)
        {
            if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                request.Headers.Authorization = AuthenticationHeaderValue.Parse(header.Value);
                continue;
            }

            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return request;
    }
}

public sealed record GatewayTraceIds(string RequestId, string CorrelationId, string TraceParent)
{
    public static GatewayTraceIds Create(GatewayObservabilityContext context, string scenarioName, int attempt)
    {
        var requestId = Guid.NewGuid().ToString("N");
        var correlationId = $"{context.BatchId}-{context.StudentId}-{context.LabId}-{Slug(scenarioName)}-{attempt}-{requestId[..8]}";
        var traceParent = $"00-{ActivityTraceId.CreateRandom()}-{ActivitySpanId.CreateRandom()}-01";
        return new GatewayTraceIds(requestId, correlationId, traceParent);
    }

    private static string Slug(string value) =>
        new(value.ToLowerInvariant().Where(ch => char.IsLetterOrDigit(ch) || ch == '-').ToArray());
}

public sealed record GatewayRunResult(
    string Name,
    int Attempt,
    string RequestId,
    string CorrelationId,
    string TraceParent,
    System.Net.HttpStatusCode? StatusCode,
    long DurationMs,
    string Body);

public static class GatewayJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
}
