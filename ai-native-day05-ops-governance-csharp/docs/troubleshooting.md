# Day 5 Troubleshooting

## Lab 01

| Symptom | Likely cause | Fix |
|---|---|---|
| lab exits before model call | Foundry auth missing | set `AZURE_OPENAI_API_KEY` or `AZURE_OPENAI_BEARER_TOKEN` |
| spans only appear in console | `APPLICATIONINSIGHTS_CONNECTION_STRING` not set | copy connection string from App Insights overview |
| no App Insights data immediately | ingestion delay | wait a few minutes, then query logs |
| no Log Analytics rows | telemetry is not connected to a workspace | confirm workspace-based Application Insights configuration |

## Lab 02

| Symptom | Likely cause | Fix |
|---|---|---|
| lab exits before model call | Foundry auth missing | set key or bearer token |
| 401/403 | wrong key or identity boundary | verify endpoint, resource, key, and RBAC |
| 404 | endpoint path or deployment mismatch | use OpenAI v1 endpoint ending in `/openai/v1` and a valid deployment |
| no token usage | response does not include usage | inspect raw response and model/API behavior |

## Lab 03

| Symptom | Likely cause | Fix |
|---|---|---|
| lab exits because route is `/health` | health check is not AI traffic | set `AGENTGATEWAY_ROUTE` to the configured chat/model route |
| gateway request fails | endpoint or route not configured | set `AGENTGATEWAY_ENDPOINT` and `AGENTGATEWAY_ROUTE` |
| 401/403 | gateway auth missing | set `AGENTGATEWAY_API_KEY` or `AGENTGATEWAY_BEARER_TOKEN` if required |
| no gateway logs | logs not routed or wrong AKS namespace/pod | use AKS pod logs, Container Insights tables, or gateway runtime logs |
| cannot correlate | trace headers not propagated by gateway | make trace propagation a Day 6 Foundation Pod requirement |

## Lab 04

| Symptom | Likely cause | Fix |
|---|---|---|
| authentication failure | Azure CLI not logged in | run `az login` |
| cost query fails | missing Cost Management permission or wrong subscription | set `AZURE_SUBSCRIPTION_ID` and verify Cost Management Reader access |
| no cost rows | cost data not yet available or batch filter does not match | confirm resource group names include `an2607101` and wait for cost ingestion |
| no model usage rows | telemetry not available | run Labs 01-03, set `LOG_ANALYTICS_WORKSPACE_ID`, then rerun |

## Lab 05

| Symptom | Likely cause | Fix |
|---|---|---|
| package requires Windows target | Foundry Local WinML package target | project targets `net8.0-windows10.0.18362` intentionally |
| first run is slow | execution providers/model download | pre-run before class |
| model not found | alias unavailable | change `FOUNDRY_LOCAL_MODEL_ALIAS` |
| local inference is slow | CPU-based runtime | use small model and focus on lifecycle |

## Lab 06

| Symptom | Likely cause | Fix |
|---|---|---|
| lab exits before call | missing Runpod endpoint/key/model | set `RUNPOD_ENDPOINT_ID`, `RUNPOD_API_KEY`, and `RUNPOD_MODEL` |
| invalid model | Runpod vLLM served model name mismatch | set `RUNPOD_MODEL` to the deployed Hugging Face model or served-name override |
| high latency | cold start | show worker lifecycle and cost/latency tradeoff |
| 401/403 | key issue | rotate or verify API key |
| timeout | worker/model slow | reduce prompt/model or inspect active/max worker settings |

## Lab 07

| Symptom | Likely cause | Fix |
|---|---|---|
| lab exits before query | missing workspace | set `LOG_ANALYTICS_WORKSPACE_ID` |
| no rows | prior labs did not emit telemetry or ingestion has not completed | run Labs 01-06 and wait for ingestion |
| students ask why no LLM | governance decisions should not depend only on model judgment | explain allow/deny/approval must be deterministic |
| policy says `Review` | missing trace/student/failure/non-allowlisted model | use the result as governance evidence, not as a lab-code failure |
