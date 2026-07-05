using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 01 - Foundry Agent Client");

Console.WriteLine("Trainer prerequisite:");
Console.WriteLine("- The trainer has created and configured the agent in Microsoft Foundry.");
Console.WriteLine("- The agent has grounding files and code interpreter/file search enabled as per the MS Learn lab.");
Console.WriteLine();

var client = new FoundryOpenAiV1Client(config);

while (true)
{
    Console.Write("Ask the Foundry agent, or type exit: ");
    var prompt = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(prompt))
    {
        continue;
    }

    if (prompt.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    // MS Learn Python mapping:
    // 1. get OpenAI client from the Foundry project client.
    // 2. create a conversation.
    // 3. add a user message to the conversation.
    // 4. create a response with agent_reference = the trainer-created agent name.
    var answer = await client.InvokeFoundryAgentAsync(config.AgentName, prompt);
    TrainingConfigConsole.PrintLlmResponseHeader();
    Console.WriteLine(answer);
    TrainingConfigConsole.PrintLlmResponseFooter();
}
