# ai-native-day02-agent-csharp

C# starter pack for Day 2: AI Agents.

This repository adapts the programming portions of the Microsoft Learn AI-3026 agent labs into a ProNative C# flow for a live Azure environment.

## Delivery Boundary

Trainer-led in Microsoft Foundry and VS Code:

- create or select the Foundry project
- configure agents, tools, knowledge, MCP, workflows, and model deployments
- perform Foundry Toolkit extension actions
- demonstrate M365/Teams publishing readiness separately

Student-facing C# starter pack:

- invoke an existing Foundry agent
- implement and reason about custom tools
- understand MCP approval flow
- call a Foundry IQ connected agent
- preview workflow invocation
- preview Agent Framework single-agent and sequential multi-agent patterns

## Lab Projects

| Lab | Project | Day 2 Depth |
|---|---|---|
| 01 | `src/Lab01FoundryAgentClient` | Core hands-on |
| 02 | `src/Lab02CustomToolsAgent` | Core hands-on |
| 03 | `src/Lab03McpAgentClient` | Core hands-on or trainer-led |
| 04 | `src/Lab04FoundryIqAgentClient` | Trainer-led/guided |
| 06 | `src/Lab06WorkflowClient` | Preview/trainer-led |
| 07 | `src/Lab07AgentFrameworkSingleAgent` | Preview/optional hands-on |
| 08 | `src/Lab08AgentFrameworkMultiAgent` | Preview/optional hands-on |

Shared utilities live in `src/Pronative.AgentTraining.Shared`.

Older helper projects are retained for compatibility:

- `src/Day02FoundryAgentClient`
- `src/Day02AgentApi`

The primary Day 2 delivery path is the seven lab folders above.

## Run

From any lab folder:

```powershell
dotnet run
```

By default, `UseLiveFoundry` is `false`, so labs run as teaching adapters and print the request/response shape. To use the live Foundry endpoint, update the lab `appsettings.json` or set:

```powershell
$env:USE_LIVE_FOUNDRY="true"
$env:AZURE_OPENAI_API_KEY="<key-or-use-bearer-token>"
$env:AZURE_OPENAI_V1_ENDPOINT="https://proj-an2607101-default-resource.openai.azure.com/openai/v1"
$env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
```

If using Entra authentication instead of an API key, set `AZURE_OPENAI_BEARER_TOKEN` to a valid access token for the resource.

## Trainer Reference

Start with [docs/lab-mapping.md](docs/lab-mapping.md). It explains:

- MS Learn source lab
- what the trainer does manually
- what C# code replaces from the Python starter
- run commands
- expected discussion points
