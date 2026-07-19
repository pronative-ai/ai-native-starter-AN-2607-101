# Lab 07 - AI Agent Governance Evidence and Policy Decisions

## Use Case

Query live telemetry from Log Analytics (emitted by Labs 01-06) and apply deterministic governance policy decisions to observed AI operations. This lab demonstrates how enterprise governance frameworks (Citadel, Agent Governance Toolkit, Agent 365) can consume operational evidence to make policy decisions.

## What It Does

1. Loads config and validates Log Analytics workspace is configured
2. Queries Log Analytics for all observed operations matching the batch ID, spanning traces, requests, dependencies, and custom events
3. Projects common fields: timestamp, event name, batch/student ID, trace ID, backend, gateway route, model deployment, total tokens, runtime success
4. Evaluates each operation against a `GovernancePolicy` that applies deterministic rules:
   - Which operations are allowed, denied, or require review
   - Whether the student/batch identity is authorized
   - Whether the operation exceeds token or latency thresholds
5. Prints a governance decision table: timestamp, outcome (Allowed/Denied/ReviewRequired), event name, student ID, reason
6. Displays an enterprise governance interpretation mapping to three reference architectures:
   - **Citadel Platform**: AI Control Plane, Security Fabric
   - **Agent Governance Toolkit**: inline policy enforcement, tool-call governance, sandboxing, audit
   - **Agent 365**: inventory, ownership, lifecycle, admin visibility

## Key Evidence Captured

- **Observed operations**: all telemetry rows for the batch, deduplicated and sorted by timestamp
- **Governance decisions**: per-operation outcome with deterministic reason
- **Enterprise interpretation**: mapping to Citadel, Agent Governance Toolkit, and Agent 365

## What You Learn

- How to query Log Analytics for AI operations telemetry using KQL via the REST API
- How to structure a deterministic governance policy engine that evaluates live evidence
- How to map governance decisions (allowed, denied, review required) to observed AI operations
- How to interpret governance evidence in the context of enterprise frameworks (Citadel, Agent Governance Toolkit, Agent 365)
- How to build the bridge from raw telemetry to actionable governance controls

## Dependencies

- .NET 8
- Log Analytics workspace with telemetry from Labs 01-06

## Required Environment Variables

| Variable | Description |
|---|---|
| `LOG_ANALYTICS_WORKSPACE_ID` | Log Analytics workspace ID |
| `EVIDENCE_LOOKBACK_DAYS` | Number of days to look back (default: 7) |
| `BATCH_ID`, `STUDENT_ID` | Batch identity for filtering |
| `AZURE_SUBSCRIPTION_ID` | (Optional) For Azure REST auth context |

## Run

```powershell
dotnet run --project src/Lab07GovernancePolicyCheck/Lab07GovernancePolicyCheck.csproj
```
