# AI-Native Day 4 - Multi-Agent Systems C# Starter Pack

This starter pack supports Day 4: Multi-Agent Systems, Protocols, and AgentGateway.

It focuses on architecture clarity and implementation boundaries:

- coordinator-worker orchestration
- A2A exposure awareness
- MCP vs UTCP tool boundary decisions
- AgentGateway routing and policy shape
- gateway observability and traffic control

## Labs

| Lab | Project | Purpose |
|---|---|---|
| Lab 01 | `Lab01MultiAgentArchitecture` | Native sequential, concurrent, handoff, and group-chat orchestration comparison |
| Lab 02 | `Lab02CoordinatorWorkerAgents` | Native Magentic-style manager and worker orchestration |
| Lab 03 | `Lab03A2AAgentExposure` | Official A2A ASP.NET Core exposure for an Agent Framework agent |
| Lab 04 | `Lab04AgentUserInteractionBoundary` | AG-UI event stream, A2UI declarative payloads, and interrupt/resume approval boundary |
| Lab 05 | `Lab05ProtocolToolBoundary` | MCP vs UTCP decisioning |
| Lab 06 | `Lab06AgentGatewayClient` | AgentGateway baseline routes, policies, managed identity auth, and request verification |
| Lab 07 | `Lab07GatewayObservabilityControl` | Gateway observability, rate-limit behavior, request correlation, and Azure Monitor KQL |

## Build

```powershell
dotnet build
```

## Design Note

Before regenerating Day 4 code, review `docs/day04-labs-component-mapping.md`.

The next code pass must use Microsoft Agent Framework and protocol-specific SDKs/components directly wherever available. Avoid custom agent abstractions, fake orchestration, or simulated protocol behavior unless the mapping explicitly marks a topic as concept-only.

For Labs 01-02 specifically, native Agent Framework orchestration builders are the primary implementation target in the installed C# package:

- `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)`
- `AgentWorkflowBuilder.CreateConcurrentBuilderWith(...)`
- `AgentWorkflowBuilder.CreateHandoffBuilderWith(...)`
- `AgentWorkflowBuilder.CreateGroupChatBuilderWith(...)`
- `AgentWorkflowBuilder.CreateMagenticBuilderWith(...)`

Do not replace these with hand-built workflow edges or custom agent loops unless the package version is unavailable and a fallback is explicitly approved.
