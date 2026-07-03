using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 07 - Agent Framework Single Agent");

Console.WriteLine("Day 2 depth: preview/optional hands-on. Day 3 will deepen workflow and harness engineering.");
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

var emailTool = new ExpenseEmailTool();
var toolOutput = emailTool.SubmitClaim(
    to: "expenses@contoso.com",
    subject: "Expense Claim",
    body: $"Claim requested by {config.StudentId}\n\n{expenses}");

var foundry = new FoundryOpenAiV1Client(config);
var answer = await foundry.ChatAsync(
    systemInstruction: "You are an expense claim agent. Use the email tool output as completed action evidence.",
    userPrompt: prompt,
    groundingContext: toolOutput);

Console.WriteLine();
Console.WriteLine(answer);

internal sealed class ExpenseEmailTool
{
    public string SubmitClaim(string to, string subject, string body)
    {
        // This is the C# equivalent of the Python lab's tool-decorated function.
        // In production training, this boundary becomes the place for approval,
        // policy checks, audit logging, and a real enterprise integration.
        return $"Email tool invoked\nTo: {to}\nSubject: {subject}\nBody:\n{body}";
    }
}
