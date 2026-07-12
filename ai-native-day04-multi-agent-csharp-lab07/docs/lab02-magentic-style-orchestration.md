# Day 4 Lab 02 - Magentic-Style Coordinator-Worker Orchestration

This lab uses the native Microsoft Agent Framework Magentic workflow builder to show how an LLM-powered manager coordinates worker agents through planning, progress tracking, and adaptive execution.

## Component Contract

- Official capability: Magentic One-style orchestration.
- Package: `Microsoft.Agents.AI.Workflows` 1.13.0, `Microsoft.Agents.AI.Foundry` 1.12.0-preview.260629.1.
- Required classes/methods:
  - `AIProjectClient.AsAIAgent(...)`
  - `AgentWorkflowBuilder.CreateMagenticBuilderWith(...)`
  - `MagenticWorkflowBuilder.AddParticipants(...)`
  - `MagenticWorkflowBuilder.WithMaxRounds(...)`
  - `MagenticWorkflowBuilder.WithMaxResets(...)`
  - `MagenticWorkflowBuilder.WithMaxStalls(...)`
  - `MagenticWorkflowBuilder.RequirePlanSignoff(...)`
  - `MagenticWorkflowBuilder.Build()`
  - `InProcessExecution.RunStreamingAsync(...)`
  - `MagenticPlanCreatedEvent`
  - `MagenticReplannedEvent`
  - `MagenticProgressLedgerUpdatedEvent`
- Required code evidence:
  - Package references for `Microsoft.Agents.AI`, `Microsoft.Agents.AI.Foundry`, and `Microsoft.Agents.AI.Workflows`.
  - `CreateMagenticBuilderWith(team.Manager)` appears in code.
  - Worker agents are registered through `AddParticipants(...)`.
  - Magentic runtime events are printed during `WatchStreamAsync()`.
- Forbidden substitutes:
  - No `ITrainingAgent`, `RoleAgent`, scripted coordinator loop, or manual worker dispatch.
  - No fake worker outputs.
  - No hand-built graph that approximates Magentic planning.
- Build acceptance: `dotnet build src/Lab02CoordinatorWorkerAgents/Lab02CoordinatorWorkerAgents.csproj`.

## What Students Should Learn

| Concept | What It Means |
|---|---|
| Manager agent | The LLM-powered coordinator that understands the goal, creates a plan, selects workers, tracks progress, and decides completion |
| Worker agents | Specialists that execute scoped parts of the plan |
| Progress ledger | Runtime state that shows whether the work started, whether progress is being made, who should speak next, and whether the request is satisfied |
| Re-planning | The manager can adjust when the plan stalls or evidence is missing |
| Plan signoff | Optional human-in-the-loop review of the manager's plan before execution |

## Run

```powershell
cd outputs\starter-repositories\ai-native-day04-multi-agent-csharp
dotnet run --project .\src\Lab02CoordinatorWorkerAgents\Lab02CoordinatorWorkerAgents.csproj
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
$env:MAGENTIC_MAX_ROUNDS = "4"
$env:MAGENTIC_REQUIRE_PLAN_SIGNOFF = "false"
dotnet run --project .\src\Lab02CoordinatorWorkerAgents\Lab02CoordinatorWorkerAgents.csproj
```

## Trainer Notes

- Keep `RequirePlanSignoff` as `false` for the first classroom run so students can see a complete Magentic execution without extra handling.
- After students understand the flow, discuss `RequirePlanSignoff(true)` as the Magentic human-in-the-loop option for reviewing manager plans before execution.
- Keep `MaxRounds` low during training to control model calls.
- Ask students to identify where the manager planned, where workers contributed, and where the progress ledger changed.
