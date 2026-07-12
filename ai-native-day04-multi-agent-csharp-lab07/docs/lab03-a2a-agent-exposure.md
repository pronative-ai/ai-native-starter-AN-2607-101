# Day 4 Lab 03 - A2A Agent Exposure

This lab exposes a Microsoft Agent Framework agent over the Agent-to-Agent protocol using the official ASP.NET Core A2A hosting integration.

## Component Contract

- Official capability: Agent Framework A2A ASP.NET Core hosting.
- Package: `Microsoft.Agents.AI.Hosting.A2A.AspNetCore` 1.13.0-preview.260703.1.
- Required classes/methods:
  - `AIProjectClient.GetProjectOpenAIClient()`
  - `GetProjectResponsesClient()`
  - `AsIChatClient(...)`
  - `builder.AddAIAgent(...)`
  - `app.MapA2AHttpJson(...)`
- Required endpoint evidence:
  - `GET /a2a/training-ops/v1/card`
  - `POST /a2a/training-ops/v1/message:stream`
- Forbidden substitutes:
  - No static-only Agent Card printer as the main lab.
  - No custom A2A-like JSON endpoint that bypasses `app.MapA2AHttpJson(...)`.
  - No fake agent response path.
- Build acceptance: `dotnet build src/Lab03A2AAgentExposure/Lab03A2AAgentExposure.csproj`.

## What Students Should Learn

| Concept | Meaning |
|---|---|
| A2A | Protocol boundary for agent-to-agent interoperability |
| Agent Card | Discovery document that tells another agent what this agent is and how to call it |
| Message stream | A2A invocation endpoint for sending a user/agent message and receiving agent output |
| Internal agent runtime | The exposed agent is still an Agent Framework agent backed by Foundry |
| Boundary discipline | A2A exposes capability, not private implementation details, prompts, secrets, or tenant internals |

## Run

```powershell
cd outputs\starter-repositories\ai-native-day04-multi-agent-csharp
$env:ASPNETCORE_URLS = "http://localhost:5000"
dotnet run --project .\src\Lab03A2AAgentExposure\Lab03A2AAgentExposure.csproj
```

The lab uses `AzureCliCredential`, so sign in first:

```powershell
az login
```

Override Foundry settings when needed:

```powershell
$env:STUDENT_ID = "ST-2606-1001"
$env:AZURE_AI_PROJECT_ENDPOINT = "https://proj-an2607101-default-resource.services.ai.azure.com/api/projects/proj-an2607101-default"
$env:AZURE_OPENAI_CHAT_DEPLOYMENT = "gpt-5-mini"
```

## Test

Use [requests/lab03-a2a.http](../src/Lab03A2AAgentExposure/requests/lab03-a2a.http) from VS Code REST Client for discovery:

```http
GET http://localhost:5000/a2a/training-ops/v1/card
```

Send an A2A message:

```http
POST http://localhost:5000/a2a/training-ops/v1/message:stream
Content-Type: application/json

{
  "message": {
    "kind": "message",
    "role": "user",
    "parts": [
      {
        "kind": "text",
        "text": "Assess Day 4 readiness for AgentGateway and protocol coverage.",
        "metadata": {}
      }
    ],
    "messageId": null,
    "contextId": "an2607101-day04-lab03"
  }
}
```

## Trainer Notes

- Start with the Agent Card and ask students what a remote agent can safely learn from it.
- Then invoke the message endpoint and emphasize that A2A is the external contract while Agent Framework remains the internal runtime.
- Contrast this with MCP: A2A is agent-to-agent; MCP is agent-to-tool/server.
- Keep deployment local in training unless the Azure Container Apps route is ready.
