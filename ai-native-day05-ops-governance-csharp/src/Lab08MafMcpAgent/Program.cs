using System.ClientModel;
using System.Net.Http.Headers;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;
using OpenAI.Chat;
using Pronative.Day05.Shared;

ConsoleTable.ApplicationStart(08);

var config = Day05Config.Load();

ConsoleTable.Header("Day 5 Lab 08 - MAF Agent with MCP via Agent Gateway");
ConsoleTable.Row("LLM Gateway", config.AgentGatewayEndpoint);
ConsoleTable.Row("LLM Model", config.AzureOpenAiChatDeployment);
ConsoleTable.Row("Batch", config.BatchId);
ConsoleTable.Row("Student", config.StudentId);

// Agent Gateway endpoints
var llmEndpoint = Environment.GetEnvironmentVariable("AGENTGATEWAY_ENDPOINT") ?? "http://localhost:4000";
var mcpEndpoint = Environment.GetEnvironmentVariable("AGENTGATEWAY_MCP_ENDPOINT") ?? "http://localhost:3000";
var apiKey = Environment.GetEnvironmentVariable("AGENTGATEWAY_API_KEY") ?? "";

Console.WriteLine();
Console.WriteLine("Connecting to Agent Gateway MCP at: " + mcpEndpoint);
Console.WriteLine("Connecting to Agent Gateway LLM at: " + llmEndpoint);

// Step 1: Connect to MCP server via Agent Gateway
Console.WriteLine();
Console.WriteLine("--- Connecting to MCP Server (Microsoft Learn via Agent Gateway) ---");

var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
if (!string.IsNullOrWhiteSpace(apiKey))
{
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
}

var transportOptions = new HttpClientTransportOptions
{
    Endpoint = new Uri(mcpEndpoint),
    Name = "AgentGateway-MCP",
};

await using var mcpClient = await McpClient.CreateAsync(
    new HttpClientTransport(transportOptions, httpClient));

Console.WriteLine("MCP Client connected successfully.");

// Step 2: List available MCP tools
Console.WriteLine();
Console.WriteLine("--- Available MCP Tools ---");
var mcpTools = await mcpClient.ListToolsAsync().ConfigureAwait(false);

var toolList = mcpTools.Cast<AITool>().ToList();
foreach (var tool in toolList)
{
    var desc = tool.Description ?? "";
    var preview = desc.Length > 80 ? desc[..80] + "..." : desc;
    Console.WriteLine($"  - {tool.Name}: {preview}");
}
Console.WriteLine($"Total tools found: {toolList.Count}");

// Step 3: Create agent using Agent Gateway LLM + MCP tools
Console.WriteLine();
Console.WriteLine("--- Creating MAF Agent ---");

var modelDeployment = config.AzureOpenAiChatDeployment;

// OpenAI SDK v2 appends /chat/completions to the Endpoint.
// Agent Gateway serves at /v1/chat/completions, so the base must include /v1.
var llmBaseUri = llmEndpoint.TrimEnd('/') + "/v1";
var openAiClient = new OpenAIClient(
    new ApiKeyCredential(apiKey),
    new OpenAIClientOptions { Endpoint = new Uri(llmBaseUri) });

ChatClient chatClient = openAiClient.GetChatClient(modelDeployment);
IChatClient iChatClient = chatClient.AsIChatClient();

AIAgent agent = iChatClient.AsAIAgent(
    name: "LearnResearchAgent",
    instructions: "You are a Microsoft Learn Research Agent. Use the MCP tools to search Microsoft documentation when users ask technical questions. Always use the search tools to find accurate, up-to-date information. Synthesize the results into clear, helpful answers.",
    tools: [.. toolList]);

Console.WriteLine("Agent created successfully.");

// Step 4: Interactive chat loop
Console.WriteLine();
Console.WriteLine("--- Interactive Chat ---");
Console.WriteLine("Ask questions about Microsoft technologies. The agent will search Microsoft Learn via MCP.");
Console.WriteLine("Type 'exit' to quit, 'tools' to list MCP tools again.");
Console.WriteLine();

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input))
        continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    if (input.Equals("tools", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine();
        Console.WriteLine("--- Available MCP Tools ---");
        foreach (var tool in toolList)
        {
            Console.WriteLine($"  - {tool.Name}: {tool.Description}");
        }
        Console.WriteLine();
        continue;
    }

    try
    {
        Console.Write("Agent: ");
        var response = await agent.RunAsync(input);
        Console.WriteLine(response.Text);
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine();
    }
}

Console.WriteLine();
Console.WriteLine("Thank you for using the Microsoft Learn Research Agent!");
ConsoleTable.ApplicationEnd(08);
