# Day 2 C# Client Mapping for MS Learn Lab 01

## Scope

This starter section maps only to:

```text
Create a client application to interact with your agent
```

from:

```text
01-build-agent-portal-and-vscode
```

The following parts remain manual in Foundry portal and VS Code:

- Create a Microsoft Foundry Project
- Configure your agent with instructions and grounding data
- Test your agent
- Interact with your agent using VS Code

## C# Project

Use:

```text
src/Day02FoundryAgentClient
```

This is the C# equivalent of the Python `agent_with_functions.py` section.

## Configuration

Open:

```text
src/Day02FoundryAgentClient/appsettings.json
```

Set:

```json
"AzureAiAgent": {
  "ProjectEndpoint": "https://<project-or-openai-v1-endpoint>",
  "AgentName": "it-support-agent",
  "BearerToken": "",
  "ApiKey": ""
}
```

For the current ProNative environment, the endpoint may look like:

```text
https://proj-an2607101-default-resource.openai.azure.com/openai/v1
```

## Python to C# Mapping

| MS Learn Python | ProNative C# |
|---|---|
| `load_dotenv()` | `TrainingConfig.Load("appsettings.json")` |
| `PROJECT_ENDPOINT` | `AzureAiAgent:ProjectEndpoint` |
| `AGENT_NAME` | `AzureAiAgent:AgentName` |
| `AIProjectClient(...)` | `FoundryAgentClient` |
| `get_openai_client()` | `GetOpenAiV1Endpoint()` |
| `conversations.create(items=[])` | `CreateConversationAsync()` |
| `conversations.items.create(...)` | `AddUserMessageAsync()` |
| `responses.create(... agent_reference ...)` | `CreateAgentResponseAsync()` |
| Python terminal loop | C# console loop |

## Run

```powershell
cd src/Day02FoundryAgentClient
dotnet run
```

Try the same prompts from MS Learn:

```text
What's the policy for password resets?
```

```text
Analyze the system performance data and identify any periods where CPU usage exceeded 80%
```

```text
Create a line chart showing memory usage trends over time
```

## Trainer Notes

The current C# starter prints text output. The MS Learn Python sample also downloads generated files/images from code interpreter output. Add file/image download handling later if needed.

The main Day 2 teaching goal is:

```text
The agent is created and configured in Foundry. The C# app is only the client application that interacts with that existing agent.
```

