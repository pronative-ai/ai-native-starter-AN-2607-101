using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 03 - MCP Agent Client");

Console.WriteLine("Trainer prerequisite:");
Console.WriteLine("- The MCP server/tool is configured in Foundry.");
Console.WriteLine("- The agent is allowed to call the MCP tool with explicit approval when needed.");
Console.WriteLine($"Configured MCP endpoint: {config.McpServerUrl}");
Console.WriteLine();

Console.Write("Ask a question that needs an MCP tool: ");
var prompt = Console.ReadLine() ?? "List available customer support tools.";

var approval = new McpApprovalRequest(
    ToolName: "support-ticket-lookup",
    Reason: "The agent needs to call an external MCP tool to retrieve operational data.",
    ServerUrl: config.McpServerUrl);

Console.WriteLine();
Console.WriteLine("MCP approval requested");
Console.WriteLine($"Tool: {approval.ToolName}");
Console.WriteLine($"Reason: {approval.Reason}");
Console.Write("Approve tool call? y/n: ");
var approved = (Console.ReadLine() ?? "").Equals("y", StringComparison.OrdinalIgnoreCase);

var toolContext = approved
    ? $"MCP tool {approval.ToolName} approved and returned: ticket PN-2042 is open, priority P2, owner cloud-ops."
    : $"MCP tool {approval.ToolName} was not approved. The agent must answer without external ticket data.";

var foundry = new FoundryOpenAiV1Client(config);
var answer = await foundry.ChatAsync(
    systemInstruction: "You are an MCP-aware support agent. Respect approval decisions before using external tool data.",
    userPrompt: prompt,
    groundingContext: toolContext);

Console.WriteLine();
Console.WriteLine(answer);

internal sealed record McpApprovalRequest(string ToolName, string Reason, string ServerUrl);
