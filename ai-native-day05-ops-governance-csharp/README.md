# AI-Native Day 5 - Operations and Governance C# Starter Pack

Program: ProNative AI-Native Fullstack Engineering  
Day: Day 5  
Batch: `AN-2607-101`  
Purpose: compact code labs for AI Operations, Observability, AgentGateway, FinOps, Governance, Foundry Local, and Runpod

Day 5 remains trainer-led and walkthrough-heavy, but these labs create real evidence the trainer can use in Azure Monitor, Log Analytics, gateway logs, FinOps review, and hybrid AI walkthroughs.

## Labs

| Lab | Project | Purpose |
|---|---|---|
| Lab 01 | `Lab01ObservabilityTelemetry` | Call Foundry live and emit OpenTelemetry spans with AI-specific tags to console and optionally Application Insights. |
| Lab 02 | `Lab02FoundryOperationalTrace` | Call Foundry/OpenAI-compatible chat endpoint and capture latency, token usage, request IDs, model, and trace fields. |
| Lab 03 | `Lab03AgentGatewayOperationalClient` | Send AI/model traffic through AgentGateway with batch/student/trace headers and inspect gateway correlation. |
| Lab 04 | `Lab04AiFinOpsEvidence` | Query live Azure Cost Management and model/gateway usage evidence. |
| Lab 05 | `Lab05FoundryLocalCpu` | Run Foundry Local lifecycle: discover providers, download/load model, chat, unload. |
| Lab 06 | `Lab06RunpodNeocloudClient` | Call a Runpod vLLM OpenAI-compatible endpoint and capture latency/cold-start evidence. |
| Lab 07 | `Lab07GovernancePolicyCheck` | Query live telemetry and apply deterministic governance decisions to observed operations. |

## Setup

Use Windows PowerShell.

```powershell
cd C:\Users\Madan\Documents\Codex\2026-06-29\we-are-now-focusing-on-pronative-2\outputs\starter-repositories\ai-native-day05-ops-governance-csharp
dotnet --version
```

Copy the template if you want file-based config:

```powershell
Copy-Item .\appsettings.template.json .\appsettings.json
```

Environment variables override `appsettings.json`.

```powershell
$env:BATCH_ID="AN-2607-101"
$env:STUDENT_ID="ST-2606-1000"
$env:AZURE_OPENAI_ENDPOINT="https://proj-an2607101-default-resource.openai.azure.com/openai/v1"
$env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
```

## Run

```powershell
dotnet run --project .\src\Lab01ObservabilityTelemetry\Lab01ObservabilityTelemetry.csproj
dotnet run --project .\src\Lab02FoundryOperationalTrace\Lab02FoundryOperationalTrace.csproj
dotnet run --project .\src\Lab03AgentGatewayOperationalClient\Lab03AgentGatewayOperationalClient.csproj
dotnet run --project .\src\Lab04AiFinOpsEvidence\Lab04AiFinOpsEvidence.csproj
dotnet run --project .\src\Lab05FoundryLocalCpu\Lab05FoundryLocalCpu.csproj
dotnet run --project .\src\Lab06RunpodNeocloudClient\Lab06RunpodNeocloudClient.csproj
dotnet run --project .\src\Lab07GovernancePolicyCheck\Lab07GovernancePolicyCheck.csproj
```

## Notes

- Lab 05 targets `net8.0-windows10.0.18362` because the Microsoft Foundry Local WinML package currently targets that framework.
- Lab 01 sends data to Application Insights only when `APPLICATIONINSIGHTS_CONNECTION_STRING` is set.
- Lab 02 executes live model calls only when `AZURE_OPENAI_API_KEY` or `AZURE_OPENAI_BEARER_TOKEN` is set.
- Lab 03 should use an AI/model gateway route, not `/health`.
- Lab 04 and Lab 07 use `LOG_ANALYTICS_WORKSPACE_ID` for telemetry evidence.
- Lab 06 executes live Runpod vLLM calls only when `RUNPOD_ENDPOINT_ID`, `RUNPOD_API_KEY`, and `RUNPOD_MODEL` are set.
- Never commit real keys into `appsettings.json`.

## Environment Variables

| Group | Variable | Default | Labs |
|---|---|---|---|
| Batch identity | `BATCH_ID` | `AN-2607-101` | All |
| | `STUDENT_ID` | `ST-2606-1000` | All |
| | `ENVIRONMENT_ID` | `an2607101` | All |
| | `COST_CENTER` | `Training` | All |
| | `OWNER` | `pronative-ai` | All |
| Azure OpenAI | `AZURE_OPENAI_ENDPOINT` | `https://proj-an2607101-default-resource.openai.azure.com/openai/v1` | 01, 02, 03 |
| | `AZURE_OPENAI_CHAT_DEPLOYMENT` | `gpt-5-mini` | 01, 02, 03 |
| | `AZURE_OPENAI_API_KEY` | *(empty)* | 01, 02 |
| | `AZURE_OPENAI_BEARER_TOKEN` | *(empty)* | 01, 02 |
| AgentGateway | `AGENTGATEWAY_ENDPOINT` | `https://aks-an2607101-agw-dns-xywyud9k.hcp.centralindia.azmk8s.io` | 03 |
| | `AGENTGATEWAY_ROUTE` | `/openai/v1/chat/completions` | 03 |
| | `AGENTGATEWAY_API_KEY` | *(empty)* | 03 |
| | `AGENTGATEWAY_BEARER_TOKEN` | *(empty)* | 03 |
| | `AGENTGATEWAY_BODY` | *(empty)* | 03 |
| Azure Monitor | `APPLICATIONINSIGHTS_CONNECTION_STRING` | *(empty)* | 01 |
| | `LOG_ANALYTICS_WORKSPACE_ID` | *(empty)* | 04, 07 |
| Cost Management | `AZURE_SUBSCRIPTION_ID` | *(empty)* | 04 |
| | `EVIDENCE_LOOKBACK_DAYS` | `7` | 04, 07 |
| Runpod | `RUNPOD_ENDPOINT_ID` | *(empty)* | 06 |
| | `RUNPOD_API_KEY` | *(empty)* | 06 |
| | `RUNPOD_MODEL` | *(empty)* | 06 |
| | `RUNPOD_OPENAI_BASE_URL` | *(empty)* | 06 |
| | `RUNPOD_PROMPT` | *(empty)* | 06 |
| Foundry Local | `FOUNDRY_LOCAL_MODEL_ALIAS` | `qwen2.5-0.5b` | 05 |
| Custom prompts | `LAB02_PROMPT` | *(empty)* | 02 |

Copy `.env.example` to `.env` and fill in the values before running the labs.

## References

| Topic | Reference |
|---|---|
| Azure Monitor OpenTelemetry for .NET | https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=aspnetcore |
| Application Insights | https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview |
| Foundry Local C# quickstart | https://learn.microsoft.com/en-us/azure/foundry-local/get-started?tabs=windows&pivots=programming-language-csharp |
| Runpod serverless | https://docs.runpod.io/serverless/overview |
| Runpod vLLM OpenAI compatibility | https://docs.runpod.io/serverless/vllm/openai-compatibility |
| Azure Cost Management | https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/quick-acm-cost-analysis |
| Foundry Citadel Platform | https://github.com/Azure-Samples/foundry-citadel-platform |
| Agent Governance Toolkit | https://github.com/microsoft/agent-governance-toolkit |
