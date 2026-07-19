# Lab 04 - AI FinOps Evidence

## Use Case

Query live Azure Cost Management and Log Analytics to produce cost, token usage, and runtime posture evidence for AI FinOps review. This lab bridges cloud financial operations with AI-specific cost drivers (token consumption, GPU workers, gateway infrastructure).

## What It Does

1. Authenticates to Azure using `DefaultAzureCredential` and resolves the subscription
2. Queries Azure Cost Management API for actual cost data grouped by resource group and service name, filtered to the batch environment
3. Queries Log Analytics for model/gateway usage telemetry previously emitted by Labs 01-03, aggregated by model deployment, backend, and gateway route
4. Prints cost rows (resource group, service, cost, currency) and model usage rows (model, events, total tokens, failures, route)
5. Displays an AI FinOps interpretation covering:
   - Azure service cost by resource group/service
   - Model usage: prompt tokens, completion tokens, total tokens, latency, failures
   - Runtime posture: always-on resources, gateway, AKS, GPU/neocloud workers, weekend shutdown
   - Budget control: INR 20,000 batch ceiling

## Key Evidence Captured

- **Cost data**: per-resource-group and per-service cost breakdown with currency
- **Model usage**: total events, prompt/completion/total tokens per model deployment and backend
- **Failures**: count of failed operations per model/route
- **FinOps interpretation**: structured discussion of cost drivers and budget controls

## What You Learn

- How to query Azure Cost Management programmatically to surface AI workload costs
- How to query Log Analytics for AI-specific telemetry (token usage, failures, routes)
- How to correlate cost data with model usage evidence
- How to interpret AI FinOps signals: always-on runtime cost, token burn rate, weekend shutdown opportunities
- How to build a budget-control narrative around batch identity and cost center tagging

## Dependencies

- .NET 8
- `Azure.Identity`, `Azure.ResourceManager`
- Active Azure subscription with cost data
- Log Analytics workspace with telemetry from Labs 01-03

## Required Environment Variables

| Variable | Description |
|---|---|
| `AZURE_SUBSCRIPTION_ID` | Azure subscription ID (optional if default subscription works) |
| `LOG_ANALYTICS_WORKSPACE_ID` | Log Analytics workspace ID for model usage query |
| `EVIDENCE_LOOKBACK_DAYS` | Number of days to look back (default: 7) |
| `BATCH_ID`, `ENVIRONMENT_ID` | Filtering identity for cost and telemetry queries |

## Run

```powershell
dotnet run --project src/Lab04AiFinOpsEvidence/Lab04AiFinOpsEvidence.csproj
```
