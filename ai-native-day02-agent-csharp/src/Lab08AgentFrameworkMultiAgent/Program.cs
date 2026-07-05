using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 08 - Agent Framework Multi-Agent");

Console.WriteLine("Day 2 depth: preview/optional hands-on. Day 4 will deepen multi-agent systems and protocols.");
Console.WriteLine("This lab uses Microsoft Agent Framework agents and an explicit sequential flow for C# trainer readability.");
Console.WriteLine();

var feedback = """
I use the dashboard every day to monitor support metrics, and it works well overall.
But late at night, the bright screen is hard on my eyes.
Please add a dark mode option.
""";

var agents = new[]
{
    new AgentStep("feedback-summarizer", "Summarize customer feedback neutrally in two bullets."),
    new AgentStep("feedback-classifier", "Classify the feedback as Positive, Negative, or Feature request and explain the label briefly."),
    new AgentStep("action-recommender", "Recommend the next action for the product team with owner and priority.")
};

var current = feedback;

if (config.UseLiveFoundry)
{
    // Agent Framework uses the Foundry project endpoint, not the /openai/v1 endpoint.
    // This explicit sequence mirrors the MS Learn Python SequentialBuilder flow:
    // summarizer -> classifier -> action recommender.
    var projectClient = new AIProjectClient(new Uri(config.ProjectEndpoint), new AzureCliCredential());

    foreach (var step in agents)
    {
        var agent = projectClient.AsAIAgent(
            model: config.ModelDeployment,
            instructions: step.Instruction,
            name: step.Name,
            description: $"Day 2 multi-agent preview participant: {step.Name}");

        var run = await agent.RunAsync(current);
        current = run.ToString();

        TrainingConfigConsole.PrintLlmResponseHeader();
        Console.WriteLine($"--- {step.Name} ---");
        Console.WriteLine(current);
        TrainingConfigConsole.PrintLlmResponseFooter();
    }
}
else
{
    foreach (var step in agents)
    {
        current = $"""
        [teaching adapter] {step.Name}
        Instruction: {step.Instruction}
        Input received:
        {current}
        """;

        Console.WriteLine();
        TrainingConfigConsole.PrintLlmResponseHeader();
        Console.WriteLine();
        Console.WriteLine($"--- {step.Name} ---");
        Console.WriteLine(current);
        Console.WriteLine();
        TrainingConfigConsole.PrintLlmResponseFooter();
        Console.WriteLine();
        Console.WriteLine();
    }
}

internal sealed record AgentStep(string Name, string Instruction);
