# Lab 06 - AgentGateway Baseline

## Use Case

This lab demonstrates AgentGateway as a runtime control point for model, MCP tool, and A2A agent traffic.

Key concepts:

- **Gateway Routes** - Model (`/azure`), MCP (`/mcp`), A2A (`/a2a`) paths
- **Policies** - Rate limiting, header injection, backend authentication
- **Managed Identity** - Gateway authenticates to Azure AI services using managed identity
- **Dry Run vs Live** - Prepare requests without sending; send when gateway is deployed

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed

### Steps

1. **Run dry run (no network calls):**
   ```powershell
   dotnet run --project .\src\Lab06AgentGatewayClient\Lab06AgentGatewayClient.csproj
   ```

2. **Run live (requires deployed gateway):**
   ```powershell
   $env:PN_AGENTGATEWAY_ENDPOINT="https://agentgateway-an2607101.azurecontainerapps.io"
   $env:PN_AGENTGATEWAY_LIVE="true"
   dotnet run --project .\src\Lab06AgentGatewayClient\Lab06AgentGatewayClient.csproj
   ```

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| AgentGateway (optional) | Live gateway deployment for live mode |

## Sample Input

No user input required. The lab prepares and optionally sends gateway requests.

## Expected Output

**Dry Run Mode:**
```
AgentGateway Configuration
==========================
config:
  tracing:
    otlpEndpoint: http://localhost:4317
    randomSampling: true

binds:
- port: 3000
  listeners:
  - name: pronative-ai-native-gateway
    routes:
    - name: an2607101-azure-foundry-openai
      matches:
      - path: {pathPrefix: /azure}
      policies:
        localRateLimit: [{maxTokens: 60, tokensPerFill: 60, fillInterval: 60s}]
      backends:
      - ai: {name: azure-foundry, provider: {azure: {resourceName: proj-an2607101-default}}}

Prepared Gateway Requests
=========================
Model route - Azure AI Foundry through AgentGateway
POST http://localhost:3000/azure/v1/chat/completions
x-correlation-id: trace-{guid}
x-batch-id: AN-2607101
x-route-purpose: model

MCP route - tool discovery through AgentGateway
POST http://localhost:3000/mcp
x-route-purpose: mcp-tools

A2A route - agent message through AgentGateway
POST http://localhost:3000/a2a/training-ops/v1/message:stream
x-route-purpose: a2a-agent

Dry run complete. No network call was made.
```

## Key Learning Points

1. **Gateway routes** - Model, MCP, and A2A traffic through single control point
2. **Rate limiting** - `localRateLimit` policy on each route
3. **Header injection** - `requestHeaderModifier` adds batch/cost metadata
4. **Managed identity** - `backendAuth.azure.explicitConfig.managedIdentity` for Azure auth
5. **Tracing** - OTLP endpoint for distributed traces

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Gateway call fails | Check `PN_AGENTGATEWAY_ENDPOINT`, gateway deployment, route config |
| 404 response | Verify route matches request path prefix |
| Authentication error | Check managed identity or bearer token configuration |

## Reference

- [AgentGateway Documentation](https://agentgateway.dev/docs/standalone/latest/)
