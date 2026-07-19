# Lab 03 - AgentGateway Runtime Control for AI Traffic

## Use Case

Route AI/model traffic through an AgentGateway proxy (local Docker or AKS-hosted) with batch/student/trace headers, then correlate the request across gateway logs, AKS pod logs, and Log Analytics.

## What It Does

1. Loads gateway configuration (endpoint, route, API key or bearer token)
2. Validates the route is not a health endpoint
3. Starts an activity span (`ai.native.agentgateway.chat_completion`) with gateway-specific tags
4. Constructs the full gateway URL and attaches `TraceHeaders` for distributed tracing
5. Authenticates via `api-key` header or `Authorization: Bearer` token
6. Prints all request headers (except auth) for inspection in gateway/aks logs
7. Sends the request to the gateway, captures response and latency
8. Attempts to parse token usage from the gateway response
9. Emits an `OperationalEvent` (`lab03.agentgateway_ai_request_completed` / `lab03.agentgateway_ai_request_failed`)
10. Supports optional body override via `AGENTGATEWAY_BODY` environment variable

## Key Concepts

- **Gateway-mediated routing**: traffic flows through AgentGateway, which applies policies (auth, rate limit, logging)
- **Trace correlation**: uses `x-batch-id`, `x-student-id`, `traceparent`, `x-trace-id` headers
- **API key authentication**: gateway validates requests against configured virtual API keys
- **Bearer token auth**: alternative auth via `Authorization: Bearer` header
- **Local vs AKS mode**: supports both local Docker Compose (`localhost:4000`) and remote AKS deployment

## What You Learn

- How to send AI requests through an AgentGateway proxy
- How to configure gateway authentication (API key vs bearer token)
- How to propagate trace context and batch/student metadata as HTTP headers
- How to correlate requests across gateway logs, AKS pod logs, and Log Analytics
- How to discuss route, backend, policy, rate limit, timeout, identity, and cost/token attribution

## Dependencies

- .NET 8
- AgentGateway instance (local Docker Compose or AKS-hosted)

## Required Environment Variables

| Variable | Description |
|---|---|
| `AGENTGATEWAY_ENDPOINT` | Gateway URL (e.g. `http://localhost:4000` for local) |
| `AGENTGATEWAY_ROUTE` | Gateway route (e.g. `/v1/chat/completions`) |
| `AGENTGATEWAY_API_KEY` or `AGENTGATEWAY_BEARER_TOKEN` | Authentication credential |
| `BATCH_ID`, `STUDENT_ID` | Batch identity for trace correlation |
| `AZURE_OPENAI_CHAT_DEPLOYMENT` | Model deployment name passed in the request body |
| `AGENTGATEWAY_BODY` | (Optional) Raw JSON body override |

## Run

```powershell
dotnet run --project src/Lab03AgentGatewayOperationalClient/Lab03AgentGatewayOperationalClient.csproj
```
