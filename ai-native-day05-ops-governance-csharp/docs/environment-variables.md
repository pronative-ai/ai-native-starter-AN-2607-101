# Day 5 Environment Variables

Use these variables from Windows PowerShell.

## Common

```powershell
$env:BATCH_ID="AN-2607-101"
$env:STUDENT_ID="ST-2606-1000"
$env:ENVIRONMENT_ID="an2607101"
$env:COST_CENTER="Training"
$env:OWNER="pronative-ai"
```

## Observability

```powershell
$env:APPLICATIONINSIGHTS_CONNECTION_STRING="<copy-from-application-insights-overview>"
$env:LOG_ANALYTICS_WORKSPACE_ID="<copy-from-log-analytics-workspace-overview>"
```

Lab 01 can print console spans without Application Insights, but the Day 5 observability lab is complete only when telemetry reaches Application Insights / Log Analytics.

Lab 04 and Lab 07 use `LOG_ANALYTICS_WORKSPACE_ID` to query model, gateway, and governance evidence from live telemetry.

## Foundry / Azure OpenAI

```powershell
$env:AZURE_OPENAI_ENDPOINT="https://proj-an2607101-default-resource.openai.azure.com/openai/v1"
$env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
$env:AZURE_OPENAI_API_KEY="<trainer-or-demo-key>"
```

For bearer-token testing, use:

```powershell
$env:AZURE_OPENAI_BEARER_TOKEN="<entra-token>"
```

Lab 02 exits if both API key and bearer token are missing because it must produce live Foundry evidence.

## AgentGateway

```powershell
$env:AGENTGATEWAY_ENDPOINT="https://aks-an2607101-agw-dns-xywyud9k.hcp.centralindia.azmk8s.io"
$env:AGENTGATEWAY_ROUTE="/openai/v1/chat/completions"
```

Optional gateway authentication:

```powershell
$env:AGENTGATEWAY_API_KEY="<gateway-api-key-if-required>"
$env:AGENTGATEWAY_BEARER_TOKEN="<gateway-bearer-token-if-required>"
```

Optional body override for custom gateway routes:

```powershell
$env:AGENTGATEWAY_BODY='{"model":"gpt-5-mini","messages":[{"role":"user","content":"hello"}],"max_tokens":100}'
```

Do not use `/health` as the main Lab 03 route. Lab 03 is complete only when the gateway handles AI/model traffic.

## Azure Resource Manager

```powershell
$env:AZURE_SUBSCRIPTION_ID="<subscription-id>"
$env:EVIDENCE_LOOKBACK_DAYS="7"
```

Lab 04 uses `DefaultAzureCredential` and Azure REST APIs for Cost Management and Log Analytics, so run:

```powershell
az login
```

## Foundry Local

```powershell
$env:FOUNDRY_LOCAL_MODEL_ALIAS="qwen2.5-0.5b"
```

Lab 05 can take time on first run because execution providers and model artifacts may be downloaded.

## Runpod

```powershell
$env:RUNPOD_ENDPOINT_ID="<endpoint-id>"
$env:RUNPOD_API_KEY="<api-key>"
$env:RUNPOD_MODEL="<deployed-vllm-model-name>"
$env:RUNPOD_PROMPT="Explain one cost-control practice for neocloud inference."
```

Lab 06 uses the Runpod OpenAI-compatible vLLM endpoint:

```text
https://api.runpod.ai/v2/{RUNPOD_ENDPOINT_ID}/openai/v1/chat/completions
```

Do not commit real Runpod API keys.
