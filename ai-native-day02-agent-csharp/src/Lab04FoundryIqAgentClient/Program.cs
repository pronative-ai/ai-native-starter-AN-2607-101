using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 04 - Foundry IQ Agent Client");

Console.WriteLine("Trainer prerequisite:");
Console.WriteLine("- The trainer has integrated the agent with Foundry IQ.");
Console.WriteLine("- Students observe the client-call shape and how approval-aware knowledge access changes the response.");
Console.WriteLine();

Console.Write("Ask a knowledge question for the Foundry IQ connected agent: ");
var prompt = Console.ReadLine() ?? "Summarize the latest approved product support policy.";

// This mirrors the Python lab's client shape: create or reuse a conversation,
// send the user message, and invoke the Foundry agent. In a live tenant,
// Foundry IQ and its approval/knowledge behavior is configured by the trainer.
var foundry = new FoundryOpenAiV1Client(config);
var answer = await foundry.InvokeFoundryAgentAsync(config.AgentName, prompt);

TrainingConfigConsole.PrintLlmResponseHeader();
Console.WriteLine(answer);
TrainingConfigConsole.PrintLlmResponseFooter();
