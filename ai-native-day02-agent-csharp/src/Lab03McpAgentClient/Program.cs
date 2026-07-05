using System.Text.Json;
using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 03 - MCP Integration");

Console.WriteLine("MS Learn mapping:");
Console.WriteLine("- Part A: trainer connects a Foundry agent to a remote MCP server such as Microsoft Learn Docs MCP.");
Console.WriteLine("- Part B: student code demonstrates a custom Inventory MCP server/client pattern in C#.");
Console.WriteLine("- The C# app discovers MCP tools, maps them to function-tool style calls, executes tools, and sends outputs back to the agent.");
Console.WriteLine();

var inventoryMcp = new InventoryMcpServer();
var foundry = new FoundryOpenAiV1Client(config);

Console.WriteLine("Choose mode:");
Console.WriteLine("1. Remote MCP agent approval flow");
Console.WriteLine("2. Custom Inventory MCP tools flow");
Console.Write("Mode [1/2]: ");
var mode = Console.ReadLine();

if (mode == "1")
{
    await RunRemoteMcpApprovalFlowAsync(config, foundry);
}
else
{
    await RunCustomInventoryMcpFlowAsync(config, foundry, inventoryMcp);
}

static async Task RunRemoteMcpApprovalFlowAsync(TrainingConfig config, FoundryOpenAiV1Client foundry)
{
    Console.WriteLine();
    Console.WriteLine("Trainer prerequisite:");
    Console.WriteLine("- A trainer-created Foundry agent is configured with an MCP tool.");
    Console.WriteLine("- Example remote server: https://learn.microsoft.com/api/mcp");
    Console.WriteLine("- The MCP tool uses require_approval='always'.");
    Console.WriteLine();

    Console.Write("Prompt for remote MCP agent: ");
    var prompt = Console.ReadLine()
        ?? "Give me the Azure CLI commands to create an Azure Container App with a managed identity.";

    if (!config.UseLiveFoundry)
    {
        Console.WriteLine();
        Console.WriteLine("[teaching adapter] Remote MCP approval flow:");
        Console.WriteLine("- agent receives prompt");
        Console.WriteLine("- agent returns mcp_approval_request");
        Console.WriteLine("- client returns mcp_approval_response approve=true");
        Console.WriteLine("- agent calls remote MCP server and produces final answer");
        return;
    }

    var conversationId = await foundry.CreateConversationAsync();
    await foundry.AddUserMessageAsync(conversationId, prompt);

    var responseJson = await foundry.CreateAgentResponseJsonAsync(conversationId, config.AgentName, prompt);
    var approvalOutputs = McpResponseParser.CreateApprovalResponses(responseJson, serverLabel: "api-specs");

    while (approvalOutputs.Count > 0)
    {
        var previousResponseId = McpResponseParser.GetResponseId(responseJson);
        responseJson = await foundry.CreateAgentFollowUpResponseJsonAsync(
            previousResponseId,
            config.AgentName,
            approvalOutputs);

        approvalOutputs = McpResponseParser.CreateApprovalResponses(responseJson, serverLabel: "api-specs");
    }

    TrainingConfigConsole.PrintLlmResponseHeader();
    Console.WriteLine(foundry.ExtractTextFromJson(responseJson));
    TrainingConfigConsole.PrintLlmResponseFooter();
}

static async Task RunCustomInventoryMcpFlowAsync(
    TrainingConfig config,
    FoundryOpenAiV1Client foundry,
    InventoryMcpServer inventoryMcp)
{
    Console.WriteLine();
    Console.WriteLine("Custom MCP server connected.");
    var tools = inventoryMcp.ListTools();
    Console.WriteLine("Discovered MCP tools:");
    foreach (var tool in tools)
    {
        Console.WriteLine($"- {tool.Name}: {tool.Description}");
    }
    Console.WriteLine();

    Console.Write("Ask inventory-agent: ");
    var prompt = Console.ReadLine()
        ?? "Show me the current inventory levels for all products.";

    if (!config.UseLiveFoundry)
    {
        Console.WriteLine();
        Console.WriteLine("[teaching adapter] Calling discovered MCP tools directly:");
        Console.WriteLine(await inventoryMcp.CallToolAsync("get_inventory_levels", "{}"));
        Console.WriteLine(await inventoryMcp.CallToolAsync("get_weekly_sales", "{}"));
        Console.WriteLine();
        Console.WriteLine("For live mode, the trainer-created inventory-agent returns function_call items, and this C# app calls the matching MCP tools.");
        return;
    }

    var conversationId = await foundry.CreateConversationAsync();
    await foundry.AddUserMessageAsync(conversationId, prompt);

    var responseJson = await foundry.CreateAgentResponseJsonAsync(
        conversationId,
        config.AgentName,
        Array.Empty<object>());

    var functionOutputs = await inventoryMcp.ExecuteFunctionCallsAsync(responseJson);

    if (functionOutputs.Count > 0)
    {
        var responseId = McpResponseParser.GetResponseId(responseJson);
        responseJson = await foundry.CreateAgentFollowUpResponseJsonAsync(
            responseId,
            config.AgentName,
            functionOutputs);
    }

    Console.WriteLine();
    TrainingConfigConsole.PrintLlmResponseHeader();
    Console.WriteLine();
    Console.WriteLine(foundry.ExtractTextFromJson(responseJson));
    Console.WriteLine();
    TrainingConfigConsole.PrintLlmResponseFooter();
}

internal sealed class InventoryMcpServer
{
    private readonly Dictionary<string, int> _inventory = new()
    {
        ["Moisturizer"] = 6,
        ["Shampoo"] = 8,
        ["Body Spray"] = 28,
        ["Sunscreen"] = 18,
        ["Face Wash"] = 4
    };

    private readonly Dictionary<string, int> _weeklySales = new()
    {
        ["Moisturizer"] = 22,
        ["Shampoo"] = 24,
        ["Body Spray"] = 3,
        ["Sunscreen"] = 12,
        ["Face Wash"] = 19
    };

    public IReadOnlyList<McpToolDefinition> ListTools()
    {
        return
        [
            new McpToolDefinition(
                "get_inventory_levels",
                "Return current inventory levels for all retail products."),
            new McpToolDefinition(
                "get_weekly_sales",
                "Return weekly sales counts for all retail products.")
        ];
    }

    public Task<string> CallToolAsync(string toolName, string argumentsJson)
    {
        var result = toolName switch
        {
            "get_inventory_levels" => JsonSerializer.Serialize(new { inventory = _inventory }),
            "get_weekly_sales" => JsonSerializer.Serialize(new { weekly_sales = _weeklySales }),
            _ => JsonSerializer.Serialize(new { error = $"Unknown MCP tool: {toolName}" })
        };

        return Task.FromResult(result);
    }

    public async Task<List<object>> ExecuteFunctionCallsAsync(string responseJson)
    {
        using var document = JsonDocument.Parse(responseJson);
        var root = document.RootElement;
        var outputs = new List<object>();

        if (!root.TryGetProperty("output", out var outputItems))
        {
            return outputs;
        }

        foreach (var item in outputItems.EnumerateArray())
        {
            if (!item.TryGetProperty("type", out var type) ||
                !type.ValueEquals("function_call"))
            {
                continue;
            }

            var functionName = item.GetProperty("name").GetString() ?? "";
            var callId = item.GetProperty("call_id").GetString() ?? "";
            var argumentsJson = item.TryGetProperty("arguments", out var args)
                ? args.GetString() ?? "{}"
                : "{}";

            var output = await CallToolAsync(functionName, argumentsJson);

            outputs.Add(new
            {
                type = "function_call_output",
                call_id = callId,
                output
            });
        }

        return outputs;
    }
}

internal static class McpResponseParser
{
    public static string GetResponseId(string responseJson)
    {
        using var document = JsonDocument.Parse(responseJson);
        return document.RootElement.GetProperty("id").GetString()
            ?? throw new InvalidOperationException("The response did not include an id.");
    }

    public static List<object> CreateApprovalResponses(string responseJson, string serverLabel)
    {
        using var document = JsonDocument.Parse(responseJson);
        var root = document.RootElement;
        var approvals = new List<object>();

        if (!root.TryGetProperty("output", out var outputItems))
        {
            return approvals;
        }

        foreach (var item in outputItems.EnumerateArray())
        {
            if (!item.TryGetProperty("type", out var type) ||
                !type.ValueEquals("mcp_approval_request"))
            {
                continue;
            }

            var itemServerLabel = item.TryGetProperty("server_label", out var label)
                ? label.GetString()
                : "";

            if (!string.Equals(itemServerLabel, serverLabel, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var approvalRequestId = item.GetProperty("id").GetString()
                ?? throw new InvalidOperationException("MCP approval request did not include an id.");

            approvals.Add(new
            {
                type = "mcp_approval_response",
                approve = true,
                approval_request_id = approvalRequestId
            });
        }

        return approvals;
    }
}

internal sealed record McpToolDefinition(string Name, string Description);
