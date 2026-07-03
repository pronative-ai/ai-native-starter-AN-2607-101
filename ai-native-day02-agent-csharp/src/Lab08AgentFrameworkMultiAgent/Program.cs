using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 08 - Agent Framework Multi-Agent");

Console.WriteLine("Day 2 depth: preview/optional hands-on. Day 4 will deepen multi-agent systems and protocols.");
Console.WriteLine();

var feedback = """
I use the dashboard every day to monitor support metrics, and it works well overall.
But late at night, the bright screen is hard on my eyes.
Please add a dark mode option.
""";

var agents = new[]
{
    new AgentStep("summarizer", "Summarize customer feedback neutrally."),
    new AgentStep("classifier", "Classify the feedback as Positive, Negative, or Feature request."),
    new AgentStep("recommended-action", "Recommend the next action for the product team.")
};

var foundry = new FoundryOpenAiV1Client(config);
var current = feedback;

foreach (var agent in agents)
{
    // This mirrors the MS Learn sequential orchestration pattern:
    // summarizer -> classifier -> recommended action. The local loop makes
    // each step visible before students meet deeper orchestration on Day 4.
    current = await foundry.ChatAsync(agent.Instruction, current);
    Console.WriteLine($"--- {agent.Name} ---");
    Console.WriteLine(current);
    Console.WriteLine();
}

internal sealed record AgentStep(string Name, string Instruction);
