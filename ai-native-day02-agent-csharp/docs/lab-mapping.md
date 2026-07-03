# Day 2 Lab Mapping - AI Agents

Program: ProNative AI-Native Fullstack Engineering  
Batch: AN-2607-101  
Core language: C# / .NET 10  
Cloud stack: Microsoft Foundry, Azure AI services, Azure observability

## How to Use This Pack

Day 2 follows Microsoft Learn AI-3026 as the authoritative training reference, but the delivery is adapted for the ProNative live Azure landing zone.

The rule for Day 2 is simple:

- Trainer performs Foundry portal, Foundry Toolkit for VS Code, project, model, agent, tool, MCP, workflow, and knowledge configuration.
- Students run C# code that maps to the programming portion of each Microsoft Learn lab.
- Workflows and multi-agent topics remain preview on Day 2 and become deeper topics on Day 3 and Day 4.

## Lab Coverage Summary

| MS Learn Lab | Official Focus | Trainer-Owned Work | C# Starter Project | Day 2 Depth |
|---|---|---|---|---|
| 01 - Build AI agents with portal and VS Code | Foundry agent, grounding files, code interpreter, client app | Create project, create `it-support-agent`, upload grounding data, test in portal, test in VS Code | `src/Lab01FoundryAgentClient` | Core hands-on |
| 02 - Use a custom function in an AI agent | Custom function/tool integration | Ensure model/project exists; explain tool design and approval boundary | `src/Lab02CustomToolsAgent` | Core hands-on |
| 03 - Extend agents with MCP tools | MCP tool connection and approval | Configure MCP server/tool in Foundry; demonstrate approval behavior | `src/Lab03McpAgentClient` | Core hands-on or trainer-led |
| 04 - Integrate an AI agent with Foundry IQ | Knowledge-enhanced agent | Configure Foundry IQ and connect it to an agent | `src/Lab04FoundryIqAgentClient` | Trainer-led/guided |
| 06 - Build a workflow in Microsoft Foundry | Workflow design and invocation | Create workflow in Foundry | `src/Lab06WorkflowClient` | Preview/trainer-led |
| 07 - Develop an Azure AI chat agent with Microsoft Agent Framework SDK | Single agent with custom tool | Explain Agent Framework concepts and package maturity | `src/Lab07AgentFrameworkSingleAgent` | Preview/optional hands-on |
| 08 - Develop a multi-agent solution with Microsoft Agent Framework | Sequential multi-agent orchestration | Explain orchestration and Day 4 continuation | `src/Lab08AgentFrameworkMultiAgent` | Preview/optional hands-on |

## Shared Configuration

Each lab contains an `appsettings.json` with this shape:

```json
{
  "BatchId": "AN-2607-101",
  "StudentId": "ST-2606-1000",
  "OpenAiV1Endpoint": "https://proj-an2607101-default-resource.openai.azure.com/openai/v1",
  "ModelDeployment": "gpt-5-mini",
  "AgentName": "it-support-agent",
  "UseLiveFoundry": false
}
```

`UseLiveFoundry=false` lets the trainer explain the code without requiring every project to call Azure immediately. Set it to `true` only after the trainer verifies endpoint, model, role assignments, and credentials.

Environment variables override file settings:

| Variable | Purpose |
|---|---|
| `BATCH_ID` | Batch identifier |
| `STUDENT_ID` | Student identifier |
| `AZURE_OPENAI_V1_ENDPOINT` | Foundry/OpenAI v1 endpoint |
| `AZURE_OPENAI_CHAT_DEPLOYMENT` | Model deployment, for example `gpt-5-mini` |
| `AZURE_OPENAI_API_KEY` | Key-based live call auth |
| `AZURE_OPENAI_BEARER_TOKEN` | Entra token live call auth |
| `FOUNDRY_AGENT_NAME` | Agent to invoke |
| `FOUNDRY_WORKFLOW_NAME` | Workflow to invoke |
| `MCP_SERVER_URL` | MCP endpoint configured by trainer |
| `USE_LIVE_FOUNDRY` | `true` or `false` |

## Lab 01 - Foundry Agent Client

MS Learn source: `01-build-agent-portal-and-vscode`

Trainer performs:

- create/select Microsoft Foundry project
- create `it-support-agent`
- set agent instructions
- attach grounding document
- enable file search and code interpreter
- test in Foundry portal
- test using Foundry Toolkit for VS Code

C# project:

```powershell
cd src/Lab01FoundryAgentClient
dotnet run
```

C# mapping:

| Python Lab Concept | C# Equivalent |
|---|---|
| `AIProjectClient` | `FoundryOpenAiV1Client` wrapper |
| `project_client.get_openai_client()` | direct HTTP calls to `/openai/v1` |
| `conversations.create()` | `InvokeFoundryAgentAsync` creates a conversation |
| `conversations.items.create()` | user message added to conversation |
| `responses.create(... agent_reference ...)` | response created with trainer-provided agent name |

Trainer talking points:

- the app does not create the agent
- the app is a runtime client
- agent identity, grounding, and tools are controlled in Foundry
- output may include grounded text, generated file references, or code interpreter artifacts depending on trainer setup

## Lab 02 - Custom Tools Agent

MS Learn source: `02-agent-custom-tools`

Trainer performs:

- explain tool function design
- explain tool descriptions as part of agent contract
- connect concept to skill-driven development: discovery, activation, execution

C# project:

```powershell
cd src/Lab02CustomToolsAgent
dotnet run
```

C# mapping:

| Python Lab Concept | C# Equivalent |
|---|---|
| Python custom functions | `AstronomyTools` methods |
| function metadata | method names and trainer explanation |
| tool activation | simple intent check before invoking tool |
| tool result passed to model | `groundingContext` passed to `ChatAsync` |

Trainer talking points:

- a tool is not just a method; it is a governed capability
- tool names, parameter names, and descriptions must be stable
- production tool execution needs identity, audit logs, rate control, and tests

## Lab 03 - MCP Agent Client

MS Learn source: `03-mcp-integration`

Trainer performs:

- configure MCP server/tool for the agent
- show what an approval request looks like
- discuss why MCP externalizes tool discovery and execution

C# project:

```powershell
cd src/Lab03McpAgentClient
dotnet run
```

C# mapping:

| Python Lab Concept | C# Equivalent |
|---|---|
| MCP tool object | `McpApprovalRequest` record |
| approval response | console approval decision |
| external tool result | approved tool context sent into answer generation |
| denied tool call | answer proceeds without external data |

Trainer talking points:

- MCP is a tool integration protocol, not an agent framework
- approval is part of governance, not just UX
- Day 4 will compare MCP with UTCP, A2A, AG-UI, and A2UI

## Lab 04 - Foundry IQ Agent Client

MS Learn source: `04-integrate-agent-with-foundry-iq`

Trainer performs:

- configure Foundry IQ knowledge source
- connect the knowledge source to the agent
- demonstrate approval/knowledge behavior in Foundry

C# project:

```powershell
cd src/Lab04FoundryIqAgentClient
dotnet run
```

C# mapping:

| Python Lab Concept | C# Equivalent |
|---|---|
| retrieve configured agent | lab config `AgentName` |
| create conversation | shared `InvokeFoundryAgentAsync` |
| ask knowledge question | console prompt |
| handle Foundry IQ behavior | trainer demonstrates in live Foundry setup |

Trainer talking points:

- Foundry IQ is positioned as managed knowledge integration
- students should understand runtime invocation and where knowledge is configured

## Lab 06 - Workflow Client

MS Learn source: `06-build-workflow-ms-foundry`

Trainer performs:

- create workflow in Foundry
- show workflow steps and routing
- explain why Day 2 treats workflow as preview

C# project:

```powershell
cd src/Lab06WorkflowClient
dotnet run
```

C# mapping:

| Python Lab Concept | C# Equivalent |
|---|---|
| workflow name | `WorkflowName` configuration |
| create conversation | shared workflow invocation |
| start workflow | `InvokeFoundryWorkflowAsync` |
| collect response | console output |

Trainer talking points:

- workflow agents belong to Day 3 depth
- Day 2 only shows how an application triggers a configured workflow

## Lab 07 - Agent Framework Single Agent

MS Learn source: `07-agent-framework`

Trainer performs:

- introduce Microsoft Agent Framework
- explain agent, tool, instruction, client, and harness boundary
- keep implementation lightweight on Day 2

C# project:

```powershell
cd src/Lab07AgentFrameworkSingleAgent
dotnet run
```

C# mapping:

| Python Lab Concept | C# Equivalent |
|---|---|
| Agent Framework agent | C# single-agent teaching structure |
| tool-decorated Python function | `ExpenseEmailTool.SubmitClaim` |
| expense data prompt | embedded expense CSV |
| model response | `ChatAsync` with tool output as context |

Trainer talking points:

- this project is a conceptual bridge to Day 3
- Day 3 adds flow engineering and harness engineering depth

## Lab 08 - Agent Framework Multi-Agent

MS Learn source: `08-agent-framework-multi-agents`

Trainer performs:

- explain sequential orchestration
- connect to Day 4 multi-agent systems

C# project:

```powershell
cd src/Lab08AgentFrameworkMultiAgent
dotnet run
```

C# mapping:

| Python Lab Concept | C# Equivalent |
|---|---|
| summarizer agent | `AgentStep("summarizer", ...)` |
| classifier agent | `AgentStep("classifier", ...)` |
| recommended action agent | `AgentStep("recommended-action", ...)` |
| sequential builder | visible C# loop over agent steps |

Trainer talking points:

- sequential orchestration is the simplest multi-agent pattern
- Day 4 expands to Magentic-style orchestration, protocols, gateway, observability, and control

## Recommended Day 2 Delivery Order

1. Portal and Foundry Toolkit overview
2. Lab 01 runtime agent client
3. Lab 02 custom tool pattern
4. Lab 03 MCP approval pattern
5. Lab 04 Foundry IQ guided demonstration
6. Lab 06 workflow preview
7. Lab 07 Agent Framework preview
8. Lab 08 multi-agent preview and transition to Days 3-4

## Build Check

From the repo root:

```powershell
dotnet build src/Lab01FoundryAgentClient/Lab01FoundryAgentClient.csproj
dotnet build src/Lab02CustomToolsAgent/Lab02CustomToolsAgent.csproj
dotnet build src/Lab03McpAgentClient/Lab03McpAgentClient.csproj
dotnet build src/Lab04FoundryIqAgentClient/Lab04FoundryIqAgentClient.csproj
dotnet build src/Lab06WorkflowClient/Lab06WorkflowClient.csproj
dotnet build src/Lab07AgentFrameworkSingleAgent/Lab07AgentFrameworkSingleAgent.csproj
dotnet build src/Lab08AgentFrameworkMultiAgent/Lab08AgentFrameworkMultiAgent.csproj
```
