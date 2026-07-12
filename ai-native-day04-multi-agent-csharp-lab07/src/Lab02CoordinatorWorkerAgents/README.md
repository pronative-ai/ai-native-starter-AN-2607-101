# Lab 02 - Magentic-Style Coordinator-Worker Orchestration

## Use Case

This lab demonstrates Magentic-style orchestration using `AgentWorkflowBuilder.CreateMagenticBuilderWith(...)` where a coordinator agent plans, delegates to worker agents, monitors progress, handles stalls, and synthesizes the final answer.

Key components:

- **Manager Agent** - Plans tasks, assigns work to workers, tracks progress, re-plans when needed
- **Worker Agents** - Curriculum, Protocol, Gateway, Validation specialists
- **Progress Ledger** - Tracks which tasks are complete, in-progress, or blocked
- **Plan Signoff** - Optional requirement for manager to approve plan before execution

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- Azure CLI installed and authenticated (`az login`)
- Azure AI Foundry project with GPT-5-mini deployment

### Steps

1. **Authenticate with Azure:**
   ```powershell
   az login
   ```

2. **Set environment variables:**
   ```powershell
   $env:AZURE_AI_PROJECT_ENDPOINT="<your-project-endpoint>"
   $env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
   ```

3. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab02CoordinatorWorkerAgents\Lab02CoordinatorWorkerAgents.csproj
   ```

4. **Enter a complex delivery goal** when prompted, or press Enter for the default.

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| Azure CLI | Authentication |
| Azure AI Foundry Project | Hosts manager and worker agents |
| GPT-5-mini deployment | Powers all agents |

## Sample Input

**Default goal (press Enter):**
```
Prepare a Day 4 readiness plan for batch AN-2607-101.
The plan must help students move from workflow agents to multi-agent systems, include protocol decisions,
identify AgentGateway readiness, and produce acceptance checks before Day 5 operations/governance.
```

## Expected Output

```
================================================================================
Native Magentic workflow run
================================================================================
[magentic:event] plan_created
Plan:
- Task 1: Curriculum alignment (assign to CurriculumWorker)
- Task 2: Protocol coverage (assign to ProtocolWorker)
- Task 3: Gateway readiness (assign to GatewayWorker)
- Task 4: Validation checks (assign to ValidationWorker)

[magentic:event] progress_ledger_updated
{"tasks": [...], "completed": [...], "inProgress": [...]}

[workflow:event] agent_response=d4l2-curriculum-{student}
[workflow:event] agent_response=d4l2-protocol-{student}
[workflow:event] agent_response=d4l2-gateway-{student}
[workflow:event] agent_response=d4l2-validation-{student}

[summary] Final captured output:
execution plan: ...
worker responsibilities: ...
risks and mitigations: ...
acceptance checks: ...
evidence to capture: ...
```

## Key Learning Points

1. **Coordinator role** - Plans, delegates, tracks progress, handles stalls, synthesizes
2. **Worker roles** - Focused specialists (curriculum, protocol, gateway, validation)
3. **Progress tracking** - Ledger shows task status across rounds
4. **Stall/reset handling** - Manager re-plans when evidence is missing
5. **Plan signoff** - Optional requirement for manager approval

## Configuration

| Variable | Default | Purpose |
|----------|---------|---------|
| `MAGENTIC_MAX_ROUNDS` | 4 | Maximum planning rounds |
| `MAGENTIC_MAX_RESETS` | 1 | Maximum plan resets |
| `MAGENTIC_MAX_STALLS` | 1 | Maximum stall before escalation |
| `MAGENTIC_REQUIRE_PLAN_SIGNOFF` | false | Require manager to sign off plan |

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Authentication error | Run `az account show` to verify login |
| Too many rounds | Check `MAGENTIC_MAX_ROUNDS` setting |
| Workers don't execute | Ensure manager assigns tasks explicitly |

## Reference

- [Magentic Orchestration](https://learn.microsoft.com/en-us/agent-framework/workflows/)
