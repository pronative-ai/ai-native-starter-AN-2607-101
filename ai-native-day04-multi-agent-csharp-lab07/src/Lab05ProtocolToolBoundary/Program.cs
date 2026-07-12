using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Pronative.MultiAgentTraining.Shared;

Day04Console.PrintAppStart();

var json = new JsonSerializerOptions
{
    WriteIndented = true
};

ConsoleFormatting.Header("Day 4 Lab 05 - MCP vs UTCP Tool Boundary");

var toolArguments = new Dictionary<string, object?>
{
    ["studentId"] = "ST-2606-1004",
    ["requestedAction"] = "Allow one controlled model-call burst for 30 minutes.",
    ["riskLevel"] = "medium"
};

Day04Console.PrintLabStart(5);

ShowMcpBoundary(toolArguments, json);
ShowUtcpBoundary(toolArguments, json);
ShowDecisionMatrix();

Day04Console.PrintLabEnd(5);

Day04Console.PrintAppEnd();

static void ShowMcpBoundary(IReadOnlyDictionary<string, object?> toolArguments, JsonSerializerOptions json)
{
    ConsoleFormatting.Header("1. MCP boundary - tool is exposed through an MCP server");

    var method = typeof(TrainingEnvironmentMcpTools).GetMethod(nameof(TrainingEnvironmentMcpTools.RaiseTrainingException))
        ?? throw new InvalidOperationException("MCP tool method was not found.");

    var mcpTool = McpServerTool.Create(
        method,
        target: null,
        options: new McpServerToolCreateOptions
        {
            Name = "raise_training_exception",
            Description = "Creates a controlled Day 4 training exception request behind an MCP tool-server boundary."
        });

    Tool protocolTool = mcpTool.ProtocolTool;

    Console.WriteLine("Official SDK evidence in this lab:");
    Console.WriteLine("- PackageReference Include=\"ModelContextProtocol\" Version=\"1.4.0\"");
    Console.WriteLine("- [McpServerToolType] and [McpServerTool] annotate the tool class and method.");
    Console.WriteLine("- McpServerTool.Create(...) creates the MCP server tool metadata.");
    Console.WriteLine("- McpServerTool.ProtocolTool exposes the protocol-level Tool shape.");
    Console.WriteLine();

    Console.WriteLine("MCP protocol tool metadata produced by the official SDK:");
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        protocolTool.Name,
        protocolTool.Title,
        protocolTool.Description,
        protocolTool.InputSchema
    }, json));

    Console.WriteLine();
    Console.WriteLine("MCP JSON-RPC call shape a client would send to an MCP server:");
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        jsonrpc = "2.0",
        id = "mcp-call-001",
        method = "tools/call",
        @params = new
        {
            name = protocolTool.Name,
            arguments = toolArguments
        }
    }, json));

    Console.WriteLine();
    Console.WriteLine("MCP hosting path for a real stdio server:");
    Console.WriteLine("""
        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();
        """);

    Console.WriteLine("Teaching point: MCP introduces a tool-server boundary. The agent/host discovers tools from that server and calls tools through MCP.");
}

static void ShowUtcpBoundary(IReadOnlyDictionary<string, object?> toolArguments, JsonSerializerOptions json)
{
    ConsoleFormatting.Header("2. UTCP boundary - tool is an existing native endpoint");

    var manual = BuildUtcpManual();
    var tool = manual["tools"]![0]!.AsObject();
    var template = tool["tool_call_template"]!.AsObject();

    Console.WriteLine("UTCP manual exposed by the existing API owner:");
    Console.WriteLine(manual.ToJsonString(json));

    using var request = BuildUtcpHttpRequest(template, toolArguments);

    Console.WriteLine();
    Console.WriteLine("Native HTTP request generated from the UTCP manual:");
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        request.Method.Method,
        Url = request.RequestUri?.ToString(),
        Headers = request.Headers.ToDictionary(header => header.Key, header => string.Join(",", header.Value)),
        Body = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult()
    }, json));

    Console.WriteLine();
    Console.WriteLine("Teaching point: UTCP does not require a new wrapper server. The manual tells the agent how to call the existing endpoint directly.");
}

static JsonObject BuildUtcpManual()
{
    return new JsonObject
    {
        ["manual_version"] = "1.0.0",
        ["utcp_version"] = "1.1.0",
        ["name"] = "pronative-training-ops-api",
        ["description"] = "Direct native API access for ProNative training operations.",
        ["tools"] = new JsonArray
        {
            new JsonObject
            {
                ["name"] = "raise_training_exception",
                ["description"] = "Creates a controlled Day 4 training exception request by calling the existing training operations API directly.",
                ["inputs"] = new JsonObject
                {
                    ["type"] = "object",
                    ["required"] = new JsonArray("studentId", "requestedAction", "riskLevel"),
                    ["properties"] = new JsonObject
                    {
                        ["studentId"] = new JsonObject { ["type"] = "string" },
                        ["requestedAction"] = new JsonObject { ["type"] = "string" },
                        ["riskLevel"] = new JsonObject
                        {
                            ["type"] = "string",
                            ["enum"] = new JsonArray("low", "medium", "high")
                        }
                    }
                },
                ["tool_call_template"] = new JsonObject
                {
                    ["call_template_type"] = "http",
                    ["url"] = "https://training-ops.pronative.ai/api/training-exceptions",
                    ["http_method"] = "POST",
                    ["content_type"] = "application/json"
                },
                ["auth"] = new JsonObject
                {
                    ["auth_type"] = "api_key",
                    ["var_name"] = "PRONATIVE_TRAINING_OPS_API_KEY",
                    ["location"] = "header",
                    ["header_name"] = "x-api-key"
                },
                ["tags"] = new JsonArray("training", "approval", "operations"),
                ["average_response_size"] = 1024
            }
        }
    };
}

static HttpRequestMessage BuildUtcpHttpRequest(JsonObject template, IReadOnlyDictionary<string, object?> arguments)
{
    var url = template["url"]?.GetValue<string>()
        ?? throw new InvalidOperationException("UTCP template is missing url.");
    var method = template["http_method"]?.GetValue<string>()
        ?? throw new InvalidOperationException("UTCP template is missing http_method.");

    var request = new HttpRequestMessage(new HttpMethod(method), url);
    request.Headers.Add("x-api-key", "${PRONATIVE_TRAINING_OPS_API_KEY}");
    request.Headers.Add("x-correlation-id", "an2607101-day04-lab05");
    request.Headers.Add("x-batch-id", "AN-2607-101");

    if (!string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
    {
        request.Content = JsonContent.Create(arguments);
    }

    return request;
}

static void ShowDecisionMatrix()
{
    ConsoleFormatting.Header("3. Enterprise decision matrix");

    var decisions = new[]
    {
        new ToolBoundaryDecision(
            Scenario: "GitHub, Azure, enterprise systems already exposed as MCP servers",
            Prefer: "MCP",
            Why: "Use standardized tool discovery, host approval, and tool-server governance."),
        new ToolBoundaryDecision(
            Scenario: "Existing internal REST API with mature auth, logging, and billing",
            Prefer: "UTCP",
            Why: "Keep native endpoint, auth, audit, and latency profile. Publish a manual instead of building a wrapper."),
        new ToolBoundaryDecision(
            Scenario: "Tool requires server-side aggregation, secrets, or multi-step normalization",
            Prefer: "MCP",
            Why: "A tool server can own secret handling and compose multiple backend calls safely."),
        new ToolBoundaryDecision(
            Scenario: "Simple direct API call already safe for automation",
            Prefer: "UTCP",
            Why: "The agent can call the API directly using the existing protocol and security model."),
        new ToolBoundaryDecision(
            Scenario: "Shared training environment needs rate limits, tags, and observability",
            Prefer: "Either, through AgentGateway",
            Why: "MCP and UTCP calls should still carry BatchId, StudentId, route, policy, and correlation headers.")
    };

    foreach (var decision in decisions)
    {
        Console.WriteLine($"Scenario: {decision.Scenario}");
        Console.WriteLine($"Prefer:   {decision.Prefer}");
        Console.WriteLine($"Why:      {decision.Why}");
        Console.WriteLine();
    }
}

[McpServerToolType]
public static class TrainingEnvironmentMcpTools
{
    [McpServerTool]
    [Description("Creates a controlled Day 4 training exception request behind an MCP server boundary.")]
    public static string RaiseTrainingException(
        [Description("Student identifier, for example ST-2606-1004.")] string studentId,
        [Description("Requested temporary environment action.")] string requestedAction,
        [Description("Risk level: low, medium, or high.")] string riskLevel)
    {
        return JsonSerializer.Serialize(new
        {
            status = "pending-trainer-approval",
            studentId,
            requestedAction,
            riskLevel,
            policy = "trainer-approval-required",
            expiresInMinutes = 30
        });
    }
}

public sealed record ToolBoundaryDecision(string Scenario, string Prefer, string Why);
