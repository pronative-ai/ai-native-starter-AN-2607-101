# Lab 02 - Flow Engineering

## Use Case

This lab demonstrates how to design explicit business processes around AI agents using Microsoft Agent Framework Workflows. Instead of relying on prompts to control behavior, the workflow defines:

- **Typed executors** with clear input/output contracts
- **Branching** based on request classification (informational, operational, sensitive)
- **Human-in-the-loop (HITL) approval** for high-risk operations
- **Bounded retry** when execution needs correction
- **Workflow state** to carry information across steps

The agent processes training operations requests, classifies them by risk, routes through approval when needed, and produces a typed final result.

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- No Azure authentication required (local-first execution)

### Steps

1. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab02FlowEngineering\Lab02FlowEngineering.csproj
   ```

2. **Enter a training operations request** when prompted, or press Enter for the default.

3. **Approve or reject** when the workflow pauses for human approval.

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| Console input | For HITL approval decisions |

## Sample Input

**Default request (press Enter):**
```
Prepare Day 3 workflow-agent practicals for batch AN-2607-101 and provision a short-lived shared approval path.
```

**Custom request examples:**
```
Delete all student progress data for batch AN-2607-101  (triggers Sensitive route → approval)
Provision a new Cosmos DB container for training data  (triggers Operational route → approval)
What are the current batch assignments?  (triggers Informational route → auto-approve)
```

## Expected Output

The workflow emits typed events as it progresses:

```
[workflow:event] executor_completed=Intake
[Intake record JSON]

[workflow:event] executor_completed=Classify
[Classification result: Operational/Sensitive/Informational]

Human Approval Required
=======================
[Approval request details]
Trainer approval [approve/reject]: approve

[workflow:event] executor_completed=ApprovalRoute
[workflow:event] executor_completed=ExecuteTrainingOperation
[retry attempt if needed]

Final Flow Engineering Result
=============================
{
  "terminalStatus": "completed",
  "route": "hitl-approval-route",
  "attempts": 2,
  "approval": "console-hitl",
  "summary": "...",
  "nextAction": "..."
}
```

## Key Learning Points

1. **Explicit workflows** - Business processes should be defined in code, not hidden in prompts
2. **Risk-based routing** - Classify requests and route based on sensitivity
3. **HITL approval** - Real console pause for operational/sensitive requests
4. **Bounded retry** - First attempt deliberately fails to demonstrate retry as workflow control
5. **Typed termination** - Final result is inspectable and structured

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Approval prompt doesn't appear | Use Windows PowerShell (not old CMD) |
| Workflow ends with `no_output` | Check console for executor errors; rerun |
| Retry doesn't trigger | First operational attempt always fails by design |

## Reference

- [Microsoft Agent Framework Workflows](https://learn.microsoft.com/en-us/agent-framework/workflows/)
