using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 07 - Agent Framework Single Agent");

Console.WriteLine("Day 2 depth: preview/optional hands-on. Day 3 will deepen flow and harness engineering.");
Console.WriteLine("This lab uses Microsoft Agent Framework for .NET, aligned to the MS Learn Agent Framework lab.");
Console.WriteLine();

var expenses = """
Date,Category,Amount,Notes
2026-07-01,Taxi,1200,Airport to customer office
2026-07-02,Meal,850,Client lunch
2026-07-02,Hotel,6200,One night stay
""";

Console.WriteLine("Loaded expense data:");
Console.WriteLine(expenses);
Console.Write("Instruction for the expense agent: ");
var prompt = Console.ReadLine() ?? "Submit an expense claim";

var fullPrompt = $"""
{prompt}

Expense rows:
{expenses}

Submit the claim to expenses@contoso.com when the claim is ready.
""";

string answer;
if (config.UseLiveFoundry)
{
    // Agent Framework uses the Foundry project endpoint, not the /openai/v1 endpoint.
    // Authentication follows the MS Learn lab pattern: az login / AzureCliCredential.
    var projectClient = new AIProjectClient(new Uri(config.ProjectEndpoint), new AzureCliCredential());

    // This is the C# equivalent of the Python @tool function in the MS Learn lab.
    // The framework exposes the function schema to the model and invokes the .NET method when selected.
    var submitClaimTool = AIFunctionFactory.Create(
        (Func<string, string, string, string>)ExpenseClaimTools.SubmitClaim,
        name: "submit_claim",
        description: "Submit an expense claim email after reviewing the expense rows.");

    var agent = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        instructions: """
        You are an expense claim agent.
        Review the submitted expense rows, identify the total claim amount, and use the submit_claim tool when the claim is ready.
        Explain the action you took in a concise trainer-friendly response.
        """,
        name: config.AgentName,
        description: "Day 2 Agent Framework single-agent training agent",
        tools: [submitClaimTool]);

    var run = await agent.RunAsync(fullPrompt);
    answer = run.ToString();
}
else
{
    var toolOutput = ExpenseClaimTools.SubmitClaim(
        to: "expenses@contoso.com",
        subject: "Expense Claim",
        body: $"Claim requested by {config.StudentId}\n\n{expenses}");

    answer = $"""
    [teaching adapter] Microsoft Agent Framework live call is disabled.

    The live path creates an AIProjectClient, wraps it with AsAIAgent, registers submit_claim as an AIFunction, and runs the agent.

    Tool output that would be produced:
    {toolOutput}
    """;
}

TrainingConfigConsole.PrintLlmResponseHeader();
Console.WriteLine(answer);
TrainingConfigConsole.PrintLlmResponseFooter();

internal static class ExpenseClaimTools
{
    public static string SubmitClaim(string to, string subject, string body)
    {
        // In real delivery this boundary becomes the place for approval checks,
        // policy validation, audit logging, and enterprise system integration.
        return $"Email tool invoked\nTo: {to}\nSubject: {subject}\nBody:\n{body}";
    }
}
