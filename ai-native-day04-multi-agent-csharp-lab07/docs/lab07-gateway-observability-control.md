# Lab 07 - Gateway Observability and Control

## Purpose

This lab shows how an AI-native team controls and observes shared gateway traffic after the baseline AgentGateway route is in place.

The focus is not to build a custom limiter, logger, or dashboard. Students use AgentGateway policy behavior, request IDs, trace headers, Container Apps logs, and Azure Monitor/App Insights queries.

## Component Contract

- Official capability: AgentGateway rate limiting, request/header attribution, trace/log emission, Azure Container Apps logs, and Azure Monitor/App Insights queryability.
- Package: none for .NET. AgentGateway standalone configuration and HTTP data plane are the official control surface.
- Required classes/methods: not applicable for AgentGateway. Required configuration and query evidence are `localRateLimit`, `requestHeaderModifier`, `backendAuth.azure.explicitConfig.managedIdentity`, `config.tracing`, `config.logging`, `ContainerAppHTTPLogs`, `ContainerAppConsoleLogs_CL`, and KQL.
- Required code evidence: `x-request-id`, `x-correlation-id`, `traceparent`, `x-batch-id`, `x-student-id`, `x-lab-id`, `x-cost-center`, `x-route-purpose`, and live status handling.
- Forbidden substitutes: no fake gateway response, no custom rate limiter, no simulated App Insights data.
- Build acceptance: `dotnet build src/Lab07GatewayObservabilityControl/Lab07GatewayObservabilityControl.csproj`.

## What Students Learn

| Concept | Practical meaning |
|---|---|
| Request ID | The value used to find a single request in Container Apps HTTP logs. |
| Correlation ID | The training/business identifier that connects client-side evidence with gateway logs. |
| Traceparent | W3C trace context carried through a distributed request path. |
| Local rate limit | Fast route-level control for training/shared-capacity protection. |
| Gateway console log | The gateway's request-level view, including route, status, duration, model, and token fields when available. |
| HTTP log | Azure Container Apps ingress-level view of path, status, latency, and request ID. |
| KQL | The query language used to inspect the evidence in Azure Monitor/Log Analytics. |

## Files

- `src/Lab07GatewayObservabilityControl/Program.cs`: prepares or sends baseline, burst, and failure-probe traffic.
- `src/Lab07GatewayObservabilityControl/config/agentgateway-observability-control.yaml`: stricter temporary route config to make local rate limiting visible during training.
- `src/Lab07GatewayObservabilityControl/observability/kql/*.kql`: KQL queries for trainer/student observation.

## Run

Dry run:

```powershell
dotnet run --project src/Lab07GatewayObservabilityControl/Lab07GatewayObservabilityControl.csproj
```

Live gateway run:

```powershell
$env:PN_AGENTGATEWAY_LIVE = "true"
$env:PN_AGENTGATEWAY_ENDPOINT = "https://agentgateway-an2607101.azurecontainerapps.io"
dotnet run --project src/Lab07GatewayObservabilityControl/Lab07GatewayObservabilityControl.csproj -- --burst-count 8
```

Optional authentication headers:

```powershell
$env:PN_AGENTGATEWAY_BEARER_TOKEN = "<token>"
$env:PN_AGENTGATEWAY_API_KEY = "<key>"
```

Useful route overrides:

```powershell
$env:PN_AGENTGATEWAY_MODEL_PATH = "/azure/v1/chat/completions"
$env:PN_AGENTGATEWAY_FAILURE_PATH = "/not-configured/day04-lab07"
$env:PN_RATE_LIMIT_BURST_COUNT = "8"
```

## Trainer Walkthrough

1. Run the lab in dry-run mode and explain the headers.
2. Show `x-request-id` and why it maps well to `ContainerAppHTTPLogs.RequestId`.
3. Show `x-correlation-id` as the batch/student/lab business correlation.
4. Show `traceparent` as the distributed tracing carrier.
5. Deploy or review the stricter Lab 07 gateway config with `localRateLimit`.
6. Run live mode to generate baseline, burst, and failure traffic.
7. Copy one `RequestId` from console output.
8. Open Log Analytics and run `04-trace-single-request.kql`.
9. Run `02-rate-limited-requests.kql` after the burst.
10. Run `05-model-token-usage.kql` to inspect model usage fields when the gateway emits them.

## Expected Outcomes

| Scenario | Expected observation |
|---|---|
| Baseline request | HTTP 2xx when the Foundry backend is healthy. |
| Burst request | HTTP 2xx until the route bucket is exhausted, then HTTP 429 when the stricter Lab 07 `localRateLimit` is deployed. |
| Failure probe | HTTP 404 or route-miss equivalent, visible in `ContainerAppHTTPLogs`. |
| Console logs | AgentGateway stdout entries show route, status, duration, and model/token fields when applicable. |
| HTTP logs | Container Apps ingress logs show request path, status, duration, and request ID. |

## Azure Monitor Notes

During training, use default/standard Azure visibility:

- Azure Container Apps HTTP logs through `ContainerAppHTTPLogs`.
- Azure Container Apps console logs through `ContainerAppConsoleLogs_CL`.
- Application Insights/OpenTelemetry for app-side and agent-side tracing where configured.

Custom ProNative workbooks stay for Day 6-8 live project delivery.

## References

- AgentGateway rate limiting: https://agentgateway.dev/docs/standalone/latest/configuration/resiliency/rate-limits/
- AgentGateway observability: https://agentgateway.dev/docs/standalone/latest/llm/observability/
- Azure Container Apps log monitoring: https://learn.microsoft.com/en-us/azure/container-apps/log-monitoring
- Application Insights OpenTelemetry overview: https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview
- Azure Monitor log queries: https://learn.microsoft.com/en-us/azure/azure-monitor/logs/log-query-overview
