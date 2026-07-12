using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Pronative.MultiAgentTraining.Shared;

Day04Console.PrintAppStart();

DotEnvLoader.Load();
var context = GatewayLabContext.From(MultiAgentOptions.FromEnvironment());
var liveMode = args.Contains("--live", StringComparer.OrdinalIgnoreCase)
    || string.Equals(Environment.GetEnvironmentVariable("PN_AGENTGATEWAY_LIVE"), "true", StringComparison.OrdinalIgnoreCase);

ConsoleFormatting.Header("Day 4 Lab 06 - AgentGateway Baseline");
Console.WriteLine("Official capability: AgentGateway standalone listener, route, backend, policy, managed identity, and tracing configuration.");
Console.WriteLine("Package/API availability: AgentGateway is not a .NET SDK surface. This lab uses the official gateway YAML configuration model and real HTTP requests through the gateway data plane.");
Console.WriteLine($"Gateway endpoint: {context.GatewayEndpoint}");
Console.WriteLine($"Mode: {(liveMode ? "live gateway calls" : "dry run request preparation")}");

ConsoleFormatting.Header("Lab 06 Component Contract");
Console.WriteLine("- Official capability: AgentGateway baseline for model, MCP tool, and A2A agent traffic.");
Console.WriteLine("- Package: none for .NET; AgentGateway standalone container and configuration schema are the official surface.");
Console.WriteLine("- Required config evidence: binds, listeners, routes, backends, requestHeaderModifier, localRateLimit, backendAuth.azure.explicitConfig.managedIdentity, tracing.");
Console.WriteLine("- Required code evidence: OpenAI-compatible, MCP JSON-RPC, and A2A-shaped requests sent to the gateway endpoint.");
Console.WriteLine("- Forbidden substitutes: no fake gateway response, no custom gateway abstraction, no direct Foundry bypass in this lab.");

ConsoleFormatting.Header("AgentGateway Configuration");
Console.WriteLine(BuildAgentGatewayConfig(context));

Day04Console.PrintLabStart(6);

ConsoleFormatting.Header("Prepared Gateway Requests");
foreach (var request in BuildPreparedRequests(context))
{
    PrintRequest(request);
}

ConsoleFormatting.Header("Execution");
if (!liveMode)
{
    Console.WriteLine("Dry run complete. No network call was made.");
    Console.WriteLine("To send these requests through the deployed gateway, set PN_AGENTGATEWAY_LIVE=true or pass --live.");
    Day04Console.PrintLabEnd(6);
    Day04Console.PrintAppEnd();
    return;
}

await SendRequestsAsync(BuildPreparedRequests(context), CancellationToken.None);

Day04Console.PrintLabEnd(6);

Day04Console.PrintAppEnd();

static IEnumerable<GatewayPreparedRequest> BuildPreparedRequests(GatewayLabContext context)
{
    yield return new GatewayPreparedRequest(
        "Model route - Azure AI Foundry through AgentGateway",
        HttpMethod.Post,
        $"{context.GatewayEndpoint}/azure/v1/chat/completions",
        new Dictionary<string, string>(BuildBaseHeaders(context))
        {
            ["x-route-purpose"] = "model",
            ["x-model-deployment"] = context.ModelDeployment
        },
        JsonSerializer.Serialize(
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
                        content = "Summarize why AgentGateway is useful for routing model, MCP tool, and A2A agent traffic in an enterprise training batch."
                    }
                },
                stream = false
            },
            JsonOptions()));

    yield return new GatewayPreparedRequest(
        "MCP route - tool discovery through AgentGateway",
        HttpMethod.Post,
        $"{context.GatewayEndpoint}/mcp",
        new Dictionary<string, string>(BuildBaseHeaders(context))
        {
            ["x-route-purpose"] = "mcp-tools"
        },
        JsonSerializer.Serialize(
            new
            {
                jsonrpc = "2.0",
                id = "day04-lab06-tools-list",
                method = "tools/list",
                @params = new { }
            },
            JsonOptions()));

    yield return new GatewayPreparedRequest(
        "A2A route - agent message through AgentGateway",
        HttpMethod.Post,
        $"{context.GatewayEndpoint}/a2a/training-ops/v1/message:stream",
        new Dictionary<string, string>(BuildBaseHeaders(context))
        {
            ["x-route-purpose"] = "a2a-agent"
        },
        JsonSerializer.Serialize(
            new
            {
                message = new
                {
                    kind = "message",
                    role = "user",
                    parts = new[]
                    {
                        new
                        {
                            kind = "text",
                            text = "Assess Day 4 readiness for AgentGateway and protocol coverage.",
                            metadata = new { }
                        }
                    },
                    messageId = (string?)null,
                    contextId = $"{context.BatchId.ToLowerInvariant()}-day04-lab06"
                }
            },
            JsonOptions()));
}

static IReadOnlyDictionary<string, string> BuildBaseHeaders(GatewayLabContext context)
{
    var headers = new Dictionary<string, string>
    {
        ["x-correlation-id"] = $"trace-{Guid.NewGuid():N}",
        ["x-batch-id"] = context.BatchId,
        ["x-student-id"] = context.StudentId,
        ["x-lab-id"] = context.LabId,
        ["x-cost-center"] = context.CostCenter
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

static async Task SendRequestsAsync(IEnumerable<GatewayPreparedRequest> preparedRequests, CancellationToken cancellationToken)
{
    using var httpClient = new HttpClient();

    foreach (var preparedRequest in preparedRequests)
    {
        using var request = preparedRequest.CreateHttpRequestMessage();
        Console.WriteLine($"Sending: {preparedRequest.Name}");

        try
        {
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            Console.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.WriteLine("Body:");
            Console.WriteLine(TrimForConsole(body));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Gateway call failed: {ex.Message}");
            Console.WriteLine("Check PN_AGENTGATEWAY_ENDPOINT, gateway deployment state, route config, frontend auth, and managed identity access to Azure AI services.");
        }

        Console.WriteLine();
    }
}

static void PrintRequest(GatewayPreparedRequest preparedRequest)
{
    Console.WriteLine();
    Console.WriteLine(preparedRequest.Name);
    Console.WriteLine($"{preparedRequest.Method} {preparedRequest.RequestUri}");
    foreach (var header in preparedRequest.Headers)
    {
        Console.WriteLine($"{header.Key}: {MaskIfSensitive(header.Key, header.Value)}");
    }

    Console.WriteLine("Body:");
    Console.WriteLine(preparedRequest.Body);
}

static string BuildAgentGatewayConfig(GatewayLabContext context) =>
    $$"""
    # yaml-language-server: $schema=https://agentgateway.dev/schema/config
    # ProNative AI-Native Day 4 Lab 06 baseline.
    # Deploy this gateway in Azure Container Apps with managed identity enabled.
    config:
      tracing:
        otlpEndpoint: {{context.OtlpEndpoint}}
        randomSampling: true
      logging:
        level: info
        format: json

    binds:
    - port: 3000
      listeners:
      - name: pronative-ai-native-gateway
        protocol: HTTP
        routes:
        - name: an2607101-azure-foundry-openai
          matches:
          - path:
              pathPrefix: /azure
          policies:
            requestHeaderModifier:
              set:
                x-batch-id: {{context.BatchId}}
                x-lab-id: {{context.LabId}}
                x-cost-center: {{context.CostCenter}}
            localRateLimit:
            - maxTokens: 60
              tokensPerFill: 60
              fillInterval: 60s
              type: requests
            backendAuth:
              azure:
                explicitConfig:
                  managedIdentity: {}
          backends:
          - ai:
              name: azure-foundry
              provider:
                azure:
                  resourceName: {{context.FoundryResourceName}}
                  projectName: {{context.FoundryProjectName}}
                  resourceType: foundry
                  model: {{context.ModelDeployment}}

        - name: an2607101-mcp-tools
          matches:
          - path:
              pathPrefix: /mcp
          policies:
            requestHeaderModifier:
              set:
                x-batch-id: {{context.BatchId}}
                x-lab-id: {{context.LabId}}
                x-route-purpose: mcp-tools
            localRateLimit:
            - maxTokens: 120
              tokensPerFill: 120
              fillInterval: 60s
              type: requests
          backends:
          - mcp:
              statefulMode: Stateless
              targets:
              - name: training-tools-http
                mcp:
                  host: {{context.McpBackendHost}}

        - name: an2607101-a2a-training-ops
          matches:
          - path:
              pathPrefix: /a2a
          policies:
            requestHeaderModifier:
              set:
                x-batch-id: {{context.BatchId}}
                x-lab-id: {{context.LabId}}
                x-route-purpose: a2a-agent
            localRateLimit:
            - maxTokens: 60
              tokensPerFill: 60
              fillInterval: 60s
              type: requests
          backends:
          - host: {{context.A2aBackendHost}}
            weight: 1
    """;

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

static JsonSerializerOptions JsonOptions() => new(JsonSerializerDefaults.Web)
{
    WriteIndented = true
};

public sealed record GatewayLabContext(
    string BatchId,
    string StudentId,
    string GatewayEndpoint,
    string LabId,
    string CostCenter,
    string FoundryResourceName,
    string FoundryProjectName,
    string ModelDeployment,
    string McpBackendHost,
    string A2aBackendHost,
    string OtlpEndpoint)
{
    public static GatewayLabContext From(MultiAgentOptions options) => new(
        options.BatchId,
        options.StudentId,
        Env("PN_AGENTGATEWAY_ENDPOINT", options.GatewayEndpoint).TrimEnd('/'),
        Env("PN_LAB_ID", "day04-lab06"),
        Env("PN_COST_CENTER", "ProNative-AI-Native-Training"),
        Env("PN_FOUNDRY_RESOURCE_NAME", "proj-an2607101-default"),
        Env("PN_FOUNDRY_PROJECT_NAME", "proj-an2607101-default"),
        Env("PN_MODEL_DEPLOYMENT", "gpt-5-mini"),
        Env("PN_MCP_BACKEND_HOST", "https://mcp-tools-an2607101.azurecontainerapps.io/mcp"),
        Env("PN_A2A_BACKEND_HOST", "a2a-training-ops-an2607101.azurecontainerapps.io:443"),
        Env("PN_OTLP_ENDPOINT", "http://localhost:4317"));

    private static string Env(string name, string fallback) =>
        string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name))
            ? fallback
            : Environment.GetEnvironmentVariable(name)!;
}

public sealed record GatewayPreparedRequest(
    string Name,
    HttpMethod Method,
    string RequestUri,
    IReadOnlyDictionary<string, string> Headers,
    string Body)
{
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
