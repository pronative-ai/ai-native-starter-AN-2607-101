# Lab 01 - Multi-Agent Architecture

## Use Case

This lab compares four multi-agent coordination topologies using Microsoft Agent Framework:

- **Sequential** - Fixed pipeline: intake → plan → risk review → final decision
- **Concurrent** - Independent specialists evaluate the same request in parallel
- **Handoff** - Router agent chooses which specialist should continue using native handoff tools
- **Group Chat** - Agents collaborate in a shared conversation under `RoundRobinGroupChatManager`

Each topology produces a final enterprise recommendation for a training operations request.

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

2. **Set environment variables** (see [Root README](../../README.md#labs) for values):
   ```powershell
   $env:AZURE_AI_PROJECT_ENDPOINT="<your-project-endpoint>"
   $env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
   ```

3. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab01MultiAgentArchitecture\Lab01MultiAgentArchitecture.csproj
   ```

4. **Enter a training operations request** when prompted, or press Enter for the default.

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| Azure CLI | Authentication |
| Azure AI Foundry Project | Hosts the specialist agents |
| GPT-5-mini deployment | Powers all agent participants |

## Sample Input

**Default request (press Enter):**
```
ProNative wants to add an optional Friday evening Day 3 support clinic for batch AN-2607-101.
Students are progressing unevenly on workflow agents, skill-driven development, and Azure AI Search grounding.
Compare the delivery impact, learner value, Azure cost impact, and governance risk before recommending a decision.
```

## Expected Output

The lab runs four workflows sequentially:

```
================================================================================
Sequential coordination
================================================================================
The request moves through a fixed pipeline: intake -> plan -> risk review -> final decision.
[workflow:event] agent_response=d4l1-intake-{student}
[workflow:event] agent_response=d4l1-planner-{student}
[workflow:event] agent_response=d4l1-risk-{student}
[workflow:event] agent_response=d4l1-final-{student}
[summary] Final captured output:
Decision: ...
Why: ...
Actions: ...

================================================================================
Concurrent coordination
================================================================================
The same request fans out to independent specialists...
[workflow:event] agent_response=d4l1-learner-{student}
[workflow:event] agent_response=d4l1-cost-{student}
[workflow:event] agent_response=d4l1-gov-{student}

================================================================================
Handoff coordination
================================================================================
The router agent chooses which specialist should continue...

================================================================================
Group-style coordination
================================================================================
Agents collaborate in a shared conversation...
```

## Key Learning Points

1. **Sequential** - Work must happen in order; each agent receives previous output
2. **Concurrent** - Independent specialists work in parallel; aggregator merges results
3. **Handoff** - Router chooses the best specialist; explicit routing decision
4. **Group Chat** - Shared conversation; agents take turns under a manager

## When to Use Each Topology

| Topology | Use When |
|----------|----------|
| Sequential | Work must happen in order |
| Concurrent | Independent specialists can work in parallel |
| Handoff | One agent routes work to the next responsible agent |
| Group Chat | Bounded discussion or critique is needed |

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Authentication error | Run `az account show` to verify login |
| Model deployment not found | Check `AZURE_OPENAI_CHAT_DEPLOYMENT=gpt-5-mini` |
| Handoff doesn't terminate | Ensure `HANDOFF_COMPLETE` marker appears in final response |
| Group chat runs too long | Check `MaximumIterationCount` (default: 4) |

## Reference

- [Agents in Workflows](https://learn.microsoft.com/en-us/agent-framework/workflows/agents-in-workflows)
