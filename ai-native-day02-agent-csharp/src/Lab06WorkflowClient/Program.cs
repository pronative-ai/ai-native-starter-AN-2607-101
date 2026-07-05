using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 06 - Foundry Workflow Client");

Console.WriteLine("Day 2 depth: preview/trainer-led.");
Console.WriteLine("The trainer creates the workflow in Foundry.");
Console.WriteLine("This C# client mirrors the MS Learn flow: create conversation, start workflow, read result, delete conversation.");
Console.WriteLine();

Console.Write("Enter a customer support issue to triage: ");
var prompt = Console.ReadLine() ?? "Customer reports failed wallet transfer and needs urgent help.";

var foundry = new FoundryOpenAiV1Client(config);
var answer = await foundry.InvokeFoundryWorkflowAsync(config.WorkflowName, prompt);

TrainingConfigConsole.PrintLlmResponseHeader();
Console.WriteLine(answer);
TrainingConfigConsole.PrintLlmResponseFooter();
