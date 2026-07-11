# Day 3 Lab 02 - Flow Engineering

## Purpose

Lab 02 moves from the agentic reasoning loop in Lab 01 to explicit business-process control using Microsoft Agent Framework Workflows.

The lab shows that an AI-native system is not only prompt-driven. Enterprise flows need typed steps, deterministic routing, human approval, retry policy, state visibility, and a clear terminal output contract.

## Microsoft Agent Framework Components

| Capability | Component Used | Where To Look |
|---|---|---|
| Typed workflow graph | `WorkflowBuilder` | `TrainingOperationsWorkflowFactory.Build(...)` |
| Typed processing steps | `Executor<TInput,TOutput>` and `Executor` | `IntakeExecutor`, `ClassifyExecutor`, `RiskRouterExecutor`, `SummaryExecutor` |
| Branching | `WorkflowBuilder.AddSwitch(...)` | Risk route and retry route |
| Real HITL | `RequestPort`, `RequestInfoEvent`, `ExternalResponse` | `TrainerApproval` port and `ReadApprovalFromConsole(...)` |
| Workflow state | `QueueStateUpdateAsync(...)`, `ReadStateAsync(...)` | Shared scope `Day03Lab02Flow` |
| Retry | Conditional switch back to `RetryPlannerExecutor` | `ExecuteTrainingOperationExecutor` -> `RetryPlannerExecutor` |
| Termination | `WorkflowOutputEvent` from `SummaryExecutor` | `FinalFlowResult` |

## Flow

1. `IntakeExecutor` records the request and writes request metadata to workflow state.
2. `ClassifyExecutor` classifies the request as informational, operational, or sensitive.
3. `RiskRouterExecutor` branches:
   - informational requests go directly to execution.
   - operational or sensitive requests raise an approval request through `RequestPort`.
4. The console host handles `RequestInfoEvent` and sends an `ExternalResponse` back into the workflow.
5. `ExecuteTrainingOperationExecutor` performs the approved action.
6. Operational requests intentionally fail once when cleanup/cost-control tags are missing.
7. `RetryPlannerExecutor` adds the missing control evidence and retries once.
8. `SummaryExecutor` emits a typed `FinalFlowResult`.

## Run

```powershell
dotnet run --project .\src\Lab02FlowEngineering\Lab02FlowEngineering.csproj
```

Use a request containing `provision`, `deploy`, `delete`, or `scale` to trigger the HITL path.

Example:

```text
Provision Day 3 workflow training resources for AN-2607-101 and verify cleanup tags.
```

Use an informational request to see the no-approval path.

Example:

```text
Summarize the Day 3 flow engineering checklist for students.
```

## Trainer Notes

- This lab is local-first. It does not require a live Foundry call.
- The purpose is to teach workflow structure before adding agents inside workflows in later Day 3 labs.
- The approval step is not mocked. The workflow pauses through `RequestPort` and resumes only after the console host sends an `ExternalResponse`.
- The retry is deliberately deterministic so learners can inspect the route without relying on random failure.
