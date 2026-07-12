# Lab 07 - Gateway Observability and Control

## Use Case

This lab demonstrates AgentGateway observability and traffic control including rate limiting, request attribution, trace/log emission, and Azure Monitor/App Insights queryability.

Key concepts:

- **Request IDs** - `x-request-id` identifies one gateway request
- **Correlation IDs** - `x-correlation-id` connects related service calls
- **Trace Parent** - `traceparent` connects distributed traces
- **Rate Limiting** - Burst requests to exercise `localRateLimit` policy
- **KQL Queries** - Azure Monitor/Log Analytics queries for investigation

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed

### Steps

1. **Run dry run (no network calls):**
   ```powershell
   dotnet run --project .\src\Lab07GatewayObservabilityControl\Lab07GatewayObservabilityControl.csproj
   ```

2. **Run live (requires deployed gateway):**
   ```powershell
   $env:PN_AGENTGATEWAY_ENDPOINT="https://agentgateway-an2607101.azurecontainerapps.io"
   $env:PN_AGENTGATEWAY_LIVE="true"
   $env:PN_RATE_LIMIT_BURST_COUNT="8"
   dotnet run --project .\src\Lab07GatewayObservabilityControl\Lab07GatewayObservabilityControl.csproj
   ```

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| AgentGateway (optional) | Live gateway deployment for live mode |
| Azure Monitor/App Insights | KQL query targets (optional) |

## Sample Input

No user input required. The lab prepares baseline, burst, and failure probe requests.

## Expected Output

**Dry Run Mode:**
```
Observation Plan
================
1. Baseline request: verify a normal gateway model request and capture x-request-id.
2. Burst requests: send a short burst to exercise localRateLimit.
3. Failure probe: send a route-not-found request for failure inspection.
4. Query Azure Monitor/App Insights using the KQL files.

KQL files:
- observability/kql/01-recent-gateway-console-logs.kql
- observability/kql/02-rate-limited-requests.kql
- observability/kql/03-http-status-latency.kql
- observability/kql/04-trace-single-request.kql
- observability/kql/05-model-token-usage.kql

Prepared Gateway Requests
=========================
Baseline model request attempt 1
Expected observation: HTTP 2xx when model backend is healthy...
POST http://localhost:3000/azure/v1/chat/completions
x-request-id: {guid}
x-correlation-id: AN-2607101-ST-2606-1000-day04-lab07-baseline-1-{guid}
traceparent: 00-{traceId}-{spanId}-01

Rate-limit burst request attempt 1-8
Expected observation: HTTP 2xx until bucket exhausted, then HTTP 429...

Failure probe attempt 1
Expected observation: HTTP 404 or gateway route-miss...
POST http://localhost:3000/not-configured/day04-lab07
```

**Live Mode:**
```
Live Gateway Run
================
Baseline model request attempt 1: 200 OK
RequestId: {guid}
CorrelationId: AN-2607101-ST-2606-1000-day04-lab07-baseline-1-{guid}
[Response body]

Rate-limit burst request attempt 1: 200 OK
Rate-limit burst request attempt 8: 429 Too Many Requests

Failure probe attempt 1: 404 Not Found

Run Summary
===========
Baseline model request | Attempt 1 | HTTP 200 OK | RequestId {guid} | 1234 ms
Rate-limit burst request | Attempt 8 | HTTP 429 Too Many Requests | RequestId {guid} | 56 ms
Failure probe | Attempt 1 | HTTP 404 Not Found | RequestId {guid} | 23 ms

Saved live run evidence: gateway-observability-results-{timestamp}.json
```

## Key Learning Points

1. **Request attribution** - `x-request-id`, `x-correlation-id`, `traceparent` for tracing
2. **Rate limiting** - Burst requests demonstrate `localRateLimit` behavior
3. **Failure observation** - Route-not-found returns 404; inspectable in logs
4. **KQL queries** - Azure Monitor/Log Analytics for operational investigation
5. **Evidence capture** - Live run results saved to JSON artifact

## KQL Query Files

| File | Purpose |
|------|---------|
| `01-recent-gateway-console-logs.kql` | Recent gateway console logs |
| `02-rate-limited-requests.kql` | Rate-limited requests (HTTP 429) |
| `03-http-status-latency.kql` | HTTP status codes and latency |
| `04-trace-single-request.kql` | Trace a single request by ID |
| `05-model-token-usage.kql` | Model token usage metrics |

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Gateway call fails | Check `PN_AGENTGATEWAY_ENDPOINT` and gateway deployment |
| Rate limit doesn't trigger | Ask trainer if stricter config is deployed |
| KQL queries return no data | Verify Azure Monitor/App Insights connection |

## Reference

- [Azure Monitor Log Query](https://learn.microsoft.com/en-us/azure/azure-monitor/logs/log-query-overview)
