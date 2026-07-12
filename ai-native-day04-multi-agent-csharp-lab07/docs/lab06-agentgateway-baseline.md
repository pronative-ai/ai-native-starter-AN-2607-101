# Lab 06 - AgentGateway Baseline

## Purpose

This lab introduces AgentGateway as the enterprise control point between AI-native applications and model, tool, and agent backends.

Students do not build another gateway abstraction in C#. They review the official AgentGateway configuration shape and run a small C# verification client that prepares or sends real gateway requests.

## Component Contract

- Official capability: AgentGateway listener, route, backend, route policy, Azure managed identity backend authentication, rate limiting, and OpenTelemetry tracing.
- Package: none for .NET. AgentGateway standalone configuration and HTTP data plane are the official lab surface.
- Required classes/methods: not applicable. Required configuration fields are `config.tracing`, `config.logging`, `binds`, `listeners`, `routes`, `matches.path.pathPrefix`, `backends`, `requestHeaderModifier`, `localRateLimit`, and `backendAuth.azure.explicitConfig.managedIdentity`.
- Required code evidence: the C# client sends OpenAI-compatible, MCP JSON-RPC, and A2A-shaped requests to `PN_AGENTGATEWAY_ENDPOINT`.
- Forbidden substitutes: no fake gateway response, no custom gateway SDK, no direct Foundry call in the main lab path.
- Build acceptance: `dotnet build src/Lab06AgentGatewayClient/Lab06AgentGatewayClient.csproj`.

## What This Lab Shows

| Route | Gateway path | Backend type | Why it matters |
|---|---|---|---|
| Model route | `/azure/v1/chat/completions` | Azure AI Foundry LLM backend | Applications use an OpenAI-compatible request while the gateway owns Foundry routing and backend identity. |
| Tool route | `/mcp` | MCP backend | Tool discovery and tool calls can be routed through a shared gateway instead of every client connecting directly to tool servers. |
| Agent route | `/a2a/training-ops/v1/message:stream` | A2A service backend | Agent-to-agent calls can pass through the same policy and observability control point. |

## ProNative Batch Defaults

| Setting | Default |
|---|---|
| Batch | `AN-2607-101` |
| Student | `ST-2606-1000` |
| Gateway endpoint | `https://agentgateway-an2607101.azurecontainerapps.io` |
| Foundry resource name | `proj-an2607101-default` |
| Foundry project name | `proj-an2607101-default` |
| Model deployment | `gpt-5-mini` |
| MCP backend | `https://mcp-tools-an2607101.azurecontainerapps.io/mcp` |
| A2A backend | `a2a-training-ops-an2607101.azurecontainerapps.io:443` |

Override these with environment variables when you use a different batch or gateway deployment.

## Files

- `src/Lab06AgentGatewayClient/Program.cs`: prepares gateway-bound model, MCP, and A2A requests.
- `src/Lab06AgentGatewayClient/config/agentgateway-an2607101-lab06.yaml`: deployable baseline AgentGateway configuration for the current batch.

## Run

Dry run:

```powershell
dotnet run --project src/Lab06AgentGatewayClient/Lab06AgentGatewayClient.csproj
```

Live gateway calls:

```powershell
$env:PN_AGENTGATEWAY_LIVE = "true"
$env:PN_AGENTGATEWAY_ENDPOINT = "https://agentgateway-an2607101.azurecontainerapps.io"
dotnet run --project src/Lab06AgentGatewayClient/Lab06AgentGatewayClient.csproj
```

If the gateway front door requires authentication, set one of:

```powershell
$env:PN_AGENTGATEWAY_BEARER_TOKEN = "<token>"
$env:PN_AGENTGATEWAY_API_KEY = "<key>"
```

## Deployment Notes

For local Docker verification, mount the YAML file into the official AgentGateway image:

```bash
docker run \
  --user "$(id -u):$(id -g)" \
  -v "$PWD/config/agentgateway-an2607101-lab06.yaml:/config.yaml" \
  -p 3000:3000 -p 127.0.0.1:15000:15000 \
  -e ADMIN_ADDR=0.0.0.0:15000 \
  cr.agentgateway.dev/agentgateway:v1.3.1 \
  -f /config.yaml
```

For ProNative Azure delivery, deploy AgentGateway into Azure Container Apps in `rg-ai-governance-hub-an2607101` with managed identity enabled. The gateway identity needs permission to call the Foundry/Azure AI backend. Use the same Log Analytics workspace/App Insights pattern used by the batch shared observability resources.

## Trainer Walkthrough

1. Open the YAML and point out `config.tracing` and `config.logging`.
2. Show the HTTP listener under `binds`.
3. Walk through the three route groups: model, MCP, and A2A.
4. Explain `requestHeaderModifier` as the place where batch/lab/cost attribution enters the request path.
5. Explain `localRateLimit` as the first-level classroom control for shared gateway routes.
6. Explain `backendAuth.azure.explicitConfig.managedIdentity` as the reason student apps do not need direct Foundry secrets.
7. Run the C# client in dry-run mode.
8. If the live gateway is deployed, run with `PN_AGENTGATEWAY_LIVE=true` and inspect status codes/logs.

## References

- AgentGateway standalone overview: https://agentgateway.dev/docs/standalone/latest/
- AgentGateway routes: https://agentgateway.dev/docs/standalone/latest/configuration/routes/
- AgentGateway backends: https://agentgateway.dev/docs/standalone/latest/configuration/backends/
- AgentGateway header manipulation: https://agentgateway.dev/docs/standalone/latest/configuration/traffic-management/manipulation/
- AgentGateway rate limiting: https://agentgateway.dev/docs/standalone/latest/configuration/resiliency/rate-limits/
- AgentGateway Azure provider: https://agentgateway.dev/docs/standalone/latest/llm/providers/azure/
- AgentGateway observability: https://agentgateway.dev/docs/standalone/latest/llm/observability/
- AgentGateway Docker deployment: https://agentgateway.dev/docs/standalone/latest/deployment/docker/
