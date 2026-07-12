```sh
[User Input] 
     │
 ┌───▼────────────────────────────────────────┐
 │ PHASE 1: Sequential Chain                  │
 │ Agent 1 ──► Agent 2 ──► Agent 3            │
 └───┬────────────────────────────────────────┘
     │ (Output passes to parallel group)
 ┌───▼────────────────────────────────────────┐
 │ PHASE 2: Concurrent/Parallel Group         │
 │          ┌──► Agent 4 (Parallel) ──┐       │
 │ Agent 3 ─┼──► Agent 5 (Parallel) ──┼─► [Aggregator / Single Agent]
 │          └──► Agent 6 (Parallel) ──┘       │
 └───┬────────────────────────────────────────┘
     │ (Combined context passes forward)
 ┌───▼────────────────────────────────────────┐
 │ PHASE 3: Final Sequential Step             │
 │ Agent 7 (Sequential)                       │
 └───┬────────────────────────────────────────┘
     │
  [Final Output]

```


```c#

using System;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI;

class Program
{
    static async Task Main(string[] args)
    {
        // 1. Initialise the underlying LLM Client (Using Azure OpenAI or OpenAI)
        IChatClient chatClient = new ChatClient(
            "gpt-4o", 
            new System.ClientModel.ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
        ).AsIChatClient();

        // 2. Define the Phase 1: Sequential Agents
        AIAgent agent1 = chatClient.AsAIAgent("You are Agent 1: Data Gatherer. Extensively fetch background details.");
        AIAgent agent2 = chatClient.AsAIAgent("You are Agent 2: Filter. Clean up the data gathered by Agent 1.");
        AIAgent agent3 = chatClient.AsAIAgent("You are Agent 3: Structurer. Organise filtered data into strict JSON sections.");

        // 3. Define Phase 2: Parallel Agents
        AIAgent agent4 = chatClient.AsAIAgent("You are Agent 4: Risk Assessor. Evaluate risks from Agent 3's structure.");
        AIAgent agent5 = chatClient.AsAIAgent("You are Agent 5: Financial Analyst. Project costs from Agent 3's structure.");
        AIAgent agent6 = chatClient.AsAIAgent("You are Agent 6: Compliance Auditor. Verify legal rules from Agent 3's structure.");

        // 4. Define Phase 3: Final Sequential Agent
        AIAgent agent7 = chatClient.AsAIAgent("You are Agent 7: Executive Summarizer. Compile the concurrent risk, cost, and compliance reports into one single final decision brief.");

        Console.WriteLine("🤖 Building Agent Workflow Topology...");

        // 5. Construct the Parallel Block (Phase 2)
        // This automatically combines/aggregates outputs of 4, 5, and 6 into a composite state context
        var parallelBlock = new ConcurrentBuilder()
            .WithParticipant(agent4)
            .WithParticipant(agent5)
            .WithParticipant(agent6);

        // 6. Construct the Master Workflow by nesting the Parallel Block sequentially
        var masterWorkflow = new SequentialBuilder()
            .WithAgent(agent1)        // Step 1 (Sequential)
            .WithAgent(agent2)        // Step 2 (Sequential)
            .WithAgent(agent3)        // Step 3 (Sequential)
            .AddStep(parallelBlock)   // Step 4 (Parallel Broadcast & Auto-Combine)
            .WithAgent(agent7)        // Step 5 (Final Sequential Aggregator)
            .Build();

        // 7. Execute the Composite Workflow
        string userPrompt = "Analyze the feasibility of launching a drone delivery network in London.";
        Console.WriteLine($"🚀 Running Workflow with Input: '{userPrompt}'\n");

        var workflowResult = await masterWorkflow.RunAsync(userPrompt);

        // 8. Output results
        Console.WriteLine("================ FINAL OUTPUT (AGENT 7) ================");
        Console.WriteLine(workflowResult.GetFinalAnswer());
    }
}

```