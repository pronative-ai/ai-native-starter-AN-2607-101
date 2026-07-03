using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 02 - Custom Tools Agent");

Console.WriteLine("This lab mirrors the MS Learn custom function pattern in C#.");
Console.WriteLine("The trainer configures the Foundry project; this app focuses on tool discovery, activation, execution, and answer shaping.");
Console.WriteLine();

var foundry = new FoundryOpenAiV1Client(config);
var tools = new AstronomyTools();

while (true)
{
    Console.Write("Ask about visible astronomy events or telescope rental cost, or type exit: ");
    var prompt = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(prompt))
    {
        continue;
    }

    if (prompt.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    // Teaching pattern:
    // Discovery: publish clear tool names and descriptions.
    // Activation: decide whether the user intent needs a tool.
    // Execution: run the C# method with explicit inputs.
    // Answer: send tool output to the model as trusted context.
    var toolResult = prompt.Contains("rent", StringComparison.OrdinalIgnoreCase)
        || prompt.Contains("cost", StringComparison.OrdinalIgnoreCase)
        ? tools.CalculateTelescopeRental(hours: 3, telescopeType: "tracking")
        : tools.NextVisibleEvent(location: "Bengaluru");

    var answer = await foundry.ChatAsync(
        systemInstruction: "You are an astronomy assistant. Explain tool results clearly and do not invent observations.",
        userPrompt: prompt,
        groundingContext: toolResult);

    Console.WriteLine();
    Console.WriteLine(answer);
    Console.WriteLine();
}

internal sealed class AstronomyTools
{
    public string NextVisibleEvent(string location)
    {
        return $"Tool next_visible_event(location='{location}') returned: Perseid meteor shower viewing window, 11:30 PM to 3:00 AM IST, clear northern sky preferred.";
    }

    public string CalculateTelescopeRental(int hours, string telescopeType)
    {
        var hourlyRate = telescopeType.Equals("tracking", StringComparison.OrdinalIgnoreCase) ? 900 : 500;
        return $"Tool calculate_telescope_rental(hours={hours}, telescopeType='{telescopeType}') returned: INR {hourlyRate * hours}.";
    }
}
