# Day 4 Lab 01 - Multi-Agent Architecture

This lab compares four native Microsoft Agent Framework coordination styles for the same ProNative enterprise training operations request.

## Component Contract

- Official capability: Multi-agent orchestration using Agent Framework workflow builders.
- Package: `Microsoft.Agents.AI.Workflows` 1.13.0, `Microsoft.Agents.AI.Foundry` 1.12.0-preview.260629.1.
- Required classes/methods:
  - `AIProjectClient.AsAIAgent(...)`
  - `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)`
  - `AgentWorkflowBuilder.CreateConcurrentBuilderWith(...)`
  - `AgentWorkflowBuilder.CreateHandoffBuilderWith(...)`
  - `AgentWorkflowBuilder.CreateGroupChatBuilderWith(...)`
  - `RoundRobinGroupChatManager`
  - `InProcessExecution.RunStreamingAsync(...)`
- Required code evidence:
  - Package references for `Microsoft.Agents.AI`, `Microsoft.Agents.AI.Foundry`, and `Microsoft.Agents.AI.Workflows`.
  - Four separate workflow builds in `Program.cs`.
  - Native workflow events printed from `WatchStreamAsync()`.
- Forbidden substitutes:
  - No `ITrainingAgent`, `RoleAgent`, or hand-built multi-agent loop.
  - No manual graph/edge implementation for sequential, concurrent, handoff, or group chat.
  - No fake agent response data.
- Build acceptance: `dotnet build src/Lab01MultiAgentArchitecture/Lab01MultiAgentArchitecture.csproj`.

## What Students Should Observe

| Coordination Style | What It Teaches | Enterprise Fit |
|---|---|---|
| Sequential | Fixed chain where each agent refines the previous output | Approval pipelines, structured review, known process order |
| Concurrent | Fan-out to specialists and aggregate outputs | Independent risk/cost/learner reviews |
| Handoff | Router or active agent chooses the next specialist | Dynamic routing based on task type or missing expertise |
| Group chat | Shared conversation managed by a group-chat manager | Collaborative review where multiple specialists react to each other |

## Run

```powershell
cd outputs\starter-repositories\ai-native-day04-multi-agent-csharp
dotnet run --project .\src\Lab01MultiAgentArchitecture\Lab01MultiAgentArchitecture.csproj
```

The lab uses `AzureCliCredential`, so sign in with an account that can access the Foundry project:

```powershell
az login
```

Override configuration with environment variables when needed:

```powershell
$env:STUDENT_ID = "ST-2606-1001"
$env:AZURE_AI_PROJECT_ENDPOINT = "https://proj-an2607101-default-resource.services.ai.azure.com/api/projects/proj-an2607101-default"
$env:AZURE_OPENAI_CHAT_DEPLOYMENT = "gpt-5-mini"
dotnet run --project .\src\Lab01MultiAgentArchitecture\Lab01MultiAgentArchitecture.csproj
```

## Trainer Notes

- Keep the same request for all four runs so students compare topology, not prompt differences.
- In handoff mode, look for the route decision and specialist transition. Handoff is implemented through the framework-provided handoff tools.
- In group-chat mode, the lab uses `RoundRobinGroupChatManager` with a small turn limit to avoid runaway model calls.
- Use this lab to prepare Day 4 Lab 02, where Magentic-style orchestration goes deeper into planner/coordinator-worker behavior.
