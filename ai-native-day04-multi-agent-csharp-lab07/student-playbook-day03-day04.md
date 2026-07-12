# Student Playbook - Day 3 and Day 4

Program: ProNative AI-Native Fullstack Engineering  
Weekend: Day 3 and Day 4  
Batch: `AN-2607-101`  
Primary stack: C#, Microsoft Agent Framework, Azure AI Foundry, Cosmos DB, Azure API Center, A2A, AG-UI/A2UI, MCP, UTCP, AgentGateway

This is your student reference for the live training session. Keep it open while the trainer walks through each lab. It explains what the lab is for, what to observe, how to run it, and how the pattern applies in enterprise AI-native engineering.

## 1. What This Weekend Is About

Day 3 shows how one agent becomes an enterprise-grade agentic application:

```text
reasoning loop
  -> flow engineering
  -> reusable skills
  -> state and memory
  -> harness
  -> retrieval-grounded workflow
  -> workflow agent
```

Day 4 shows how agentic applications work together:

```text
multi-agent topologies
  -> Magentic-style coordination
  -> A2A interoperability
  -> agent-user interaction
  -> tool protocols
  -> gateway routing
  -> observability and control
```

The main idea:

```text
AI-native engineering is not only calling a model.
It is designing the control, state, evidence, protocols, and operations around model-powered work.
```

## 2. Setup

Use Windows PowerShell.

Check .NET:

```powershell
dotnet --version
```

Expected:

```text
10.x
```

Check Azure login:

```powershell
az account show
```

If required:

```powershell
az login
```

## 3. Common Environment Variables

Use these unless the trainer gives you a different student ID:

```powershell
$env:BATCH_ID="AN-2607-101"
$env:STUDENT_ID="ST-2606-1000"
$env:AZURE_AI_PROJECT_ENDPOINT="https://proj-an2607101-default-resource.services.ai.azure.com/api/projects/proj-an2607101-default"
$env:AZURE_OPENAI_ENDPOINT="https://proj-an2607101-default-resource.openai.azure.com/"
$env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
$env:COSMOS_ENDPOINT="https://cosmos-an2607101.documents.azure.com:443/"
$env:COSMOS_DATABASE="db-an2607101-training"
$env:COSMOS_CONTAINER="training-knowledge"
$env:COSMOS_SESSION_CONTAINER="agent-session-checkpoints"
```

If your assigned student ID is different:

```powershell
$env:STUDENT_ID="ST-2606-1001"
```

Important endpoint note:

```text
AZURE_AI_PROJECT_ENDPOINT must be the project-scoped Foundry endpoint.
Do not use only the raw Azure OpenAI endpoint for Agent Framework project calls.
```

## 4. Repository Locations

Day 3:

```powershell
cd C:\Users\Madan\Documents\Codex\2026-06-29\we-are-now-focusing-on-pronative-2\outputs\starter-repositories\ai-native-day03-agent-framework-csharp
```

Day 4:

```powershell
cd C:\Users\Madan\Documents\Codex\2026-06-29\we-are-now-focusing-on-pronative-2\outputs\starter-repositories\ai-native-day04-multi-agent-csharp
```

## 5. How To Use This Playbook

For each lab:

1. Read "Before running, understand this."
2. Run the command when the trainer asks.
3. Watch the console for the listed evidence.
4. Answer the "You are complete when" checks.
5. Use the reference link if you want more detail during or after the session.

Poll questions are intentionally not included here. The trainer will use them live.

### 5.1 How To Use The `Microsoft.Agents.AI` API Reference

The `Microsoft.Agents.AI` namespace reference is the .NET API catalog for Microsoft Agent Framework. It is not a step-by-step tutorial. Use it when you want to understand what a class does, what options it exposes, and which related types are available.

Reference:

- https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest

Important note:

```text
Some Agent Framework APIs are still marked as prerelease in Microsoft Learn.
Class names, options, and behavior can change, so use the docs together with the starter code that has been verified for this batch.
```

How to read it during the labs:

| When you see this in code | Search the API reference for | What you are trying to learn |
|---|---|---|
| An agent is created or run | `AIAgent`, `AgentResponse`, `AgentRunOptions` | How Agent Framework represents an agent run and response. |
| A lab uses session state | `AgentSession`, `AgentSessionStateBag`, `ProviderSessionState<TState>` | How conversation/session-scoped state is stored. |
| A lab injects memory/context | `AIContext`, `AIContextProvider`, `MessageAIContextProvider` | How extra context is supplied to model calls. |
| A lab uses chat history | `ChatHistoryProvider`, `InMemoryChatHistoryProvider`, `CosmosChatHistoryProvider` | How conversation history can be stored and retrieved. |
| A lab uses skills | `AgentSkill`, `AgentFileSkill`, `AgentInlineSkill`, `AgentClassSkill<TSelf>`, `AgentSkillsProviderBuilder` | How reusable capabilities are discovered and exposed. |
| A lab uses reflection/retry | `LoopAgent`, `LoopEvaluator`, `DelegateLoopEvaluator`, `CompletionMarkerLoopEvaluator` | How bounded agent loops decide whether to continue. |
| A lab uses approval | `ToolApprovalAgent`, `ToolApprovalAgentOptions` | How tool execution can be paused for approval. |
| A lab uses files or todo state | `FileAccessProvider`, `FileMemoryProvider`, `TodoProvider`, `AgentModeProvider` | How long-running agent work gets operational support. |
| A lab discusses evaluation | `AgentEvaluationExtensions`, `EvalItem`, `EvalChecks`, `LocalEvaluator` | How agent outputs and runs can be evaluated. |
| A lab discusses telemetry | `OpenTelemetryAgent`, `OpenTelemetryAgentBuilderExtensions`, `LoggingAgent` | How agent execution can become observable. |
| A lab discusses RAG | `TextSearchProvider`, `ChatHistoryMemoryProvider` | How retrieved knowledge can be injected or exposed to an agent. |

Examples from this weekend:

| Lab | API areas to look up |
|---|---|
| Day 3 Lab 01 - Agentic reasoning loop | `AIAgent`, `AgentResponse`, `LoopAgent`, `DelegateLoopEvaluator`, `ToolApprovalAgent`, `AgentSession` |
| Day 3 Lab 03 - Skills | `AgentSkill`, `AgentFileSkill`, `AgentInlineSkill`, `AgentClassSkill<TSelf>`, `AgentSkillsProviderBuilder` |
| Day 3 Lab 04 - State and memory | `AgentSession`, `AgentSessionStateBag`, `ProviderSessionState<TState>`, `AIContextProvider`, `InMemoryChatHistoryProvider` |
| Day 3 Lab 05 - Harness | `FileMemoryProvider`, `FileAccessProvider`, `TodoProvider`, `AgentModeProvider`, loop evaluators, tool approval types |
| Day 3 Lab 06 - RAG workflow | `TextSearchProvider` as reference concept, plus `AIAgent` and tool/function boundaries in code |
| Day 3 Lab 07 - Workflow agent | `AIAgent`, `AgentResponse`, streaming response/update types |
| Day 4 Labs 01-02 - Multi-agent orchestration | `AIAgent`, `AgentResponse`, evaluation and logging types, plus workflow-specific APIs from the workflow docs |
| Day 4 Lab 07 - Gateway observability | `LoggingAgent`, `OpenTelemetryAgent`, `OpenTelemetryAgentBuilderExtensions` |

Use the API reference as a map. Use the starter code as the runnable path.

## 6. Day 3 Lab Map

| Lab | Topic | What you learn |
|---|---|---|
| Lab 01 | Agentic reasoning loop | Goal, plan, action, observation, reflection, checkpoint |
| Lab 02 | Flow engineering | Workflow steps, branching, approval, retry, termination |
| Lab 03 | Skills | Discovery, activation, execution, reuse |
| Lab 04 | State and memory | Session, context, storage, serialization, Cosmos checkpointing, compaction |
| Lab 05 | Harness | Runtime around the model: tools, evidence, approval, telemetry |
| Lab 06 | Retrieval-grounded RAG | Retrieval, grounding, verification, retry, citations |
| Lab 07 | Workflow agent | Specialist agents inside a structured workflow |

## 7. Day 3 Labs

### Lab 01 - Agentic AI Reasoning Loop

Project:

```text
src\Lab01AgenticReasoningLoop
```

Run:

```powershell
dotnet run --project .\src\Lab01AgenticReasoningLoop\Lab01AgenticReasoningLoop.csproj
```

Before running, understand this:

An agentic loop is:

```text
goal -> plan -> action -> observation -> reflection
```

In a normal chat app, the model may produce an answer directly. In an agentic system, the model is part of a larger execution loop. The application can provide tools, enforce approvals, observe tool results, ask the model to reflect, and store checkpoint state.

What "Foundry-backed agent" means:

- The app runs locally in C#.
- The agent is created with Microsoft Agent Framework.
- The model call goes to the live Azure AI Foundry project and `gpt-5-mini` deployment.
- Tools, middleware, loop evaluation, and session state are handled by the app.
- This is not a fake response and does not require a portal-created agent.

Key components:

| Component | Meaning |
|---|---|
| `AIProjectClient` | Connects to the Foundry project endpoint. |
| `AsAIAgent(...)` | Creates a Foundry-backed Agent Framework agent. |
| `AIFunctionFactory.Create(...)` | Turns C# functions into tools. |
| `UseToolApproval(...)` | Adds a trust boundary before tools run. |
| Middleware | Logs and governs agent/tool/model activity. |
| `LoopAgent` | Allows bounded reflection/retry. |
| `AgentSession.StateBag` | Stores local checkpoint metadata. |

Watch for:

- middleware log messages
- tool validation messages
- approval prompt
- final sections: `Goal`, `Plan`, `Action`, `Observation`, `Reflection`
- `Session Checkpoint Boundary`

Why this matters:

Enterprise agents should not be black boxes. You need to see the goal, plan, action, observation, and reflection so that the system can be reviewed, tested, and governed.

You are complete when:

- the live Foundry call succeeds
- you can identify goal, plan, action, observation, and reflection
- you can explain why a tool call is a trust boundary
- you can explain what the session checkpoint stores

Reference:

- https://learn.microsoft.com/en-us/agent-framework/agents/middleware/

### Lab 02 - Flow Engineering

Project:

```text
src\Lab02FlowEngineering
```

Run:

```powershell
dotnet run --project .\src\Lab02FlowEngineering\Lab02FlowEngineering.csproj
```

Before running, understand this:

Flow engineering means designing the process around the agent. A flow can classify work, branch, pause for approval, retry, and terminate with a typed result.

A prompt can say "ask for approval if needed", but a workflow can actually pause and wait for approval. That is the difference between instruction and engineered control.

Key components:

| Component | Meaning |
|---|---|
| `WorkflowBuilder` | Defines the workflow graph. |
| Typed executors | Give each step clear input/output contracts. |
| Branching | Makes decision paths explicit. |
| Human approval | Pauses execution for real input. |
| Workflow state | Carries information across steps. |
| Typed final output | Makes the final result inspectable. |

Watch for:

- workflow events
- branch decision
- approval prompt
- retry path if triggered
- final typed output

Why this matters:

Enterprise processes often require approvals, retry rules, and clear termination. These should not be hidden inside one large prompt.

You are complete when:

- you can explain the workflow steps
- you see the approval pause
- you can explain branching, retry, and termination

Reference:

- https://learn.microsoft.com/en-us/agent-framework/workflows/

### Lab 03 - Agent Skills

Project:

```text
src\Lab03SkillDrivenDevelopment
```

Run:

```powershell
dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
```

Optional API Center/MCP path, only when trainer asks:

```powershell
$env:ENABLE_APIC_MCP_SKILLS="true"
$env:APIC_RUNTIME_URL="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
$env:APIC_MCP_ENDPOINT="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
```

Before running, understand this:

Skills are reusable capabilities. They should have names, descriptions, versions, input/output expectations, and a way to be discovered and reused.

The skill lifecycle is:

```text
Discovery -> Activation -> Execution -> Reuse
```

Skill types in this lab:

| Skill type | Meaning |
|---|---|
| File-based skill | Skill definition and supporting content live in files. |
| Inline skill | Small skill is defined directly in code. |
| Class-based skill | Skill is implemented as reusable typed C# code. |
| MCP/API Center skill | Skill is discoverable or cataloged through an enterprise boundary. |

Important API Center distinction:

```text
Azure API Center is catalog and governance.
It is not the runtime executor.
```

Watch for:

- skills discovered from files
- inline skills
- class-based skills
- optional API Center/MCP path

Why this matters:

As systems grow, you do not want each agent to reinvent capabilities. Skills let teams package and govern reusable work.

You are complete when:

- you can name all four skill types
- you can explain discovery, activation, execution, and reuse
- you can explain why API Center is catalog/governance, not runtime execution

Reference:

- https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp

### Lab 04 - Conversations, State, and Memory

Project:

```text
src\Lab04ConversationsMemory
```

Run local session path:

```powershell
dotnet run --project .\src\Lab04ConversationsMemory\Lab04ConversationsMemory.csproj
```

If the trainer enables Cosmos DB checkpointing:

```powershell
$env:ENABLE_COSMOS_PERSISTENCE="true"
$env:COSMOS_SESSION_CONTAINER="agent-session-checkpoints"
dotnet run --project .\src\Lab04ConversationsMemory\Lab04ConversationsMemory.csproj
```

Before running, understand this:

Memory is not one feature. Separate these ideas:

| Concept | Meaning |
|---|---|
| Session continuity | The current conversation/run boundary. |
| Task state | Metadata such as batch ID, student ID, task ID, and status. |
| Context provider | Injects remembered facts into later turns. |
| Chat history | Stores conversation messages. |
| Serialization | Converts session state to a checkpoint payload. |
| Durable checkpoint | Stores checkpoint outside the process, for example in Cosmos DB. |
| Compaction | Reduces context growth so the system does not carry too much history. |

What Cosmos DB adds:

- It stores the serialized `AgentSession` checkpoint outside the process.
- It lets the app restore session state after a restart.
- It does not replace `AgentSession`, context providers, chat history providers, or compaction.

Watch for:

- `BatchId`, `StudentId`, `TaskId`, `TaskStatus`
- remembered preference such as C#
- local JSON checkpoint
- optional Cosmos checkpoint write/read
- compaction before and after message counts

Why this matters:

Long-running enterprise work needs recoverable state. You should know what is stored in memory, what is serialized, and what is persisted outside the process.

You are complete when:

- you can explain `AgentSession`
- you can explain what `StateBag` stores
- you can explain how context providers inject remembered facts
- you can explain what is written to Cosmos DB when checkpointing is enabled
- you can explain why compaction is needed

Reference:

- https://learn.microsoft.com/en-us/agent-framework/agents/conversations/?pivots=programming-language-csharp

### Lab 05 - Harness Engineering

Project:

```text
src\Lab05HarnessEngineering
```

Run:

```powershell
dotnet run --project .\src\Lab05HarnessEngineering\Lab05HarnessEngineering.csproj
```

Before running, understand this:

A harness is the runtime around the model. It helps with longer-running work by managing tools, approvals, files, todo/mode state, loop evaluation, compaction, evidence, and telemetry.

Think of it this way:

```text
model = reasoning and generation
harness = controls, memory/files, approvals, loop checks, evidence, telemetry
```

You should see:

- official Agent Framework Harness usage
- tool approval
- loop evaluator
- evidence output
- file memory/access
- `Evidence`, `Evaluation`, and `Completion` sections

Why this matters:

Enterprise teams need repeatable evidence of what happened during an agent run. A harness helps make agent work inspectable and safer.

You are complete when:

- you can explain what a harness adds beyond a model call
- the final response ends with `DONE`
- evidence file is written
- you can explain why approval and loop evaluation belong in the harness

Reference:

- https://learn.microsoft.com/en-us/agent-framework/agents/harness?pivots=programming-language-csharp

### Lab 06 - Retrieval-Grounded RAG for Agentic Workflow

Project:

```text
src\Lab06HybridRagWorkflow
```

Trainer may seed Cosmos DB first:

```powershell
$env:COSMOS_CREATE_IF_NOT_EXISTS="true"
$env:COSMOS_SEED_SAMPLE_DATA="true"
dotnet run --project .\src\Lab06HybridRagWorkflow\Lab06HybridRagWorkflow.csproj -- --seed
```

Run:

```powershell
dotnet run --project .\src\Lab06HybridRagWorkflow\Lab06HybridRagWorkflow.csproj
```

Before running, understand this:

RAG is not one Azure service. RAG means grounding an answer with retrieved evidence.

In this lab:

```text
question
  -> retrieval plan
  -> retrieve evidence from Cosmos DB
  -> answer with Foundry-backed agent
  -> verify grounding and citations
  -> retry if grounding is weak
  -> final typed result
```

Why Cosmos DB here:

- Azure AI Search is strong for document and hybrid search, but it was removed from Day 3 runtime because of cost across weekends.
- Cosmos DB is used hands-on for structured/semi-structured operational grounding.
- HorizonDB is covered as a concept for PostgreSQL-native vector/full-text/hybrid retrieval.

Watch for:

- Cosmos endpoint and container
- retrieval plan event
- retrieval tool call
- retrieved document IDs
- citations like `[doc:pn-d3-cosmos-grounding]`
- verification status
- final artifact path

Good test prompts:

```text
Why are we using Cosmos DB for Lab 06 instead of Azure AI Search?
```

```text
Compare Azure AI Search, Cosmos DB, and HorizonDB for retrieval-grounded agentic apps in this course.
```

```text
What is the relationship between agent skills, API Center, and MCP runtime loading?
```

Negative grounding test:

```text
What exact GPU model and hourly price are used by this batch for HorizonDB?
```

The negative test should not hallucinate. It should say the evidence is not available in the retrieved records.

Why this matters:

Agentic RAG should make retrieval, answering, verification, retry, and citations visible. The model should not receive hidden data access.

You are complete when:

- Cosmos DB records are retrieved
- answer cites retrieved records
- verification result is visible
- you can compare Cosmos DB, Azure AI Search, and HorizonDB at a high level

Reference:

- https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3

### Lab 07 - Workflow Agent

Project:

```text
src\Lab07WorkflowAgent
```

Run:

```powershell
dotnet run --project .\src\Lab07WorkflowAgent\Lab07WorkflowAgent.csproj
```

Before running, understand this:

A workflow agent composes specialist agents inside a structured workflow.

This lab uses specialist roles such as:

- Intent Analyst
- Planning Agent
- Risk Reviewer
- Finalizer Agent

The workflow preserves order and output structure while each specialist focuses on a clear role.

Watch for:

- specialist agent names
- intermediate outputs
- streaming workflow events
- final structured result

Why this matters:

This is the bridge to Day 4. Once you understand specialist agents inside a workflow, you are ready to compare multi-agent topologies and protocol boundaries.

You are complete when:

- live Foundry calls succeed
- specialist outputs are visible
- final structured result is visible
- you can explain why specialist agents were used

Reference:

- https://learn.microsoft.com/en-us/agent-framework/workflows/agents-in-workflows

## 8. Day 4 Lab Map

| Lab | Topic | What you learn |
|---|---|---|
| Lab 01 | Multi-agent architecture | Sequential, concurrent, handoff, and group-chat coordination |
| Lab 02 | Magentic-style orchestration | Coordinator-worker planning and execution |
| Lab 03 | A2A | Agent exposure, discovery, consumption, and API Center metadata |
| Lab 04 | AG-UI/A2UI | Agent-user interaction boundaries |
| Lab 05 | MCP vs UTCP | Tool-server vs direct API tool boundary |
| Lab 06 | AgentGateway | Runtime routing and policy boundary |
| Lab 07 | Gateway observability/control | Correlation, rate limits, traces, and logs |

## 9. Day 4 Labs

### Lab 01 - Multi-Agent Architecture

Project:

```text
src\Lab01MultiAgentArchitecture
```

Run:

```powershell
dotnet run --project .\src\Lab01MultiAgentArchitecture\Lab01MultiAgentArchitecture.csproj
```

Before running, understand this:

Multi-agent architecture is about choosing the coordination topology. It is not about adding more agents for the sake of it.

| Topology | Use when |
|---|---|
| Sequential | Work must happen in order. |
| Concurrent | Independent specialists can work in parallel. |
| Handoff | One agent routes work to the next responsible agent. |
| Group chat | Bounded discussion or critique is needed. |

Watch for:

- four coordination styles
- agent outputs
- where work is merged or handed off

Why this matters:

Each additional agent adds coordination cost. Use multiple agents when responsibility, data, tools, policy, or evidence boundaries are different.

You are complete when:

- you can compare all four topologies
- you can explain when concurrent orchestration is useful
- you can explain when not to use multiple agents

Reference:

- https://learn.microsoft.com/en-us/agent-framework/workflows/agents-in-workflows

### Lab 02 - Magentic-Style Coordinator-Worker Orchestration

Project:

```text
src\Lab02CoordinatorWorkerAgents
```

Run:

```powershell
dotnet run --project .\src\Lab02CoordinatorWorkerAgents\Lab02CoordinatorWorkerAgents.csproj
```

Before running, understand this:

Magentic-style orchestration uses a coordinator that plans, delegates to workers, monitors progress, handles stalls, and synthesizes the final answer.

This is useful when:

- the task is open-ended
- multiple specialist roles are needed
- the system needs to track progress
- intermediate results must be reviewed

Watch for:

- coordinator behavior
- worker roles
- rounds
- stall/reset behavior
- final synthesis

Why this matters:

Coordinator-worker systems are powerful, but they can be expensive and complex. Use them when the work justifies planning and delegation.

You are complete when:

- you can explain coordinator vs worker responsibilities
- you can explain why rounds/stalls/resets matter
- you can explain when Magentic-style orchestration is too heavy

Reference:

- https://learn.microsoft.com/en-us/agent-framework/workflows/

### Lab 03 - A2A Exposure, Discovery, and Consumption

Projects:

```text
src\Lab03A2AAgentExposure
src\Lab03A2AAgentConsumer
```

Start provider:

```powershell
dotnet run --project .\src\Lab03A2AAgentExposure\Lab03A2AAgentExposure.csproj
```

Run consumer in a second terminal:

```powershell
$env:A2A_BASE_URL="http://localhost:5063"
$env:A2A_AGENT_CARD_PATH="/a2a/training-ops/v1/card"
dotnet run --project .\src\Lab03A2AAgentConsumer\Lab03A2AAgentConsumer.csproj
```

Before running, understand this:

A2A is for agent-to-agent interoperability. One agent exposes an Agent Card and message endpoint. Another agent or client discovers the card and sends a message.

Important distinction:

| Item | Meaning |
|---|---|
| A2A provider | Runtime agent endpoint. |
| Agent Card | Describes agent identity, endpoint, capabilities, and contract. |
| A2A consumer | Resolves the Agent Card and sends a message. |
| Azure API Center | Catalog/governance metadata. It is not the runtime broker. |
| AgentGateway | Optional runtime route/control point. |

Watch for:

- provider starts on `http://localhost:5063`
- Agent Card resolves
- consumer sends message
- API Center metadata is discussed

Why this matters:

Enterprise agents may need to interoperate across apps, teams, or runtimes. A2A defines the agent-to-agent boundary.

You are complete when:

- you can explain provider vs consumer
- you can explain Agent Card purpose
- you can explain API Center vs runtime
- you can explain how AgentGateway can sit in front

Reference:

- https://a2a-protocol.org/latest/specification/

### Lab 04 - AG-UI / A2UI Agent-User Interaction Boundary

Project:

```text
src\Lab04AgentUserInteractionBoundary
```

Run:

```powershell
dotnet run --project .\src\Lab04AgentUserInteractionBoundary\Lab04AgentUserInteractionBoundary.csproj
```

Before running, understand this:

Agent-user interaction is not only final text. A useful agent UI may need to show progress, tool calls, approval requests, interrupts, and resumed work.

Comparison:

| Boundary | Focus |
|---|---|
| A2A | Agent-to-agent discovery and messaging. |
| AG-UI | Event-driven agent-to-UI runtime interaction. |
| A2UI | Declarative/adaptive agent-to-UI payload concept. |

Watch for:

- run started event
- tool/action event
- interrupt or approval shape
- resume payload
- A2UI comparison payload

Why this matters:

Users need visibility and control when agents take actions. This becomes especially important for approval workflows and long-running tasks.

You are complete when:

- you can explain interrupt/resume
- you can compare AG-UI and A2UI
- you can explain why this is different from A2A

Reference:

- https://docs.ag-ui.com/introduction

### Lab 05 - MCP vs UTCP Tool Boundary

Project:

```text
src\Lab05ProtocolToolBoundary
```

Run:

```powershell
dotnet run --project .\src\Lab05ProtocolToolBoundary\Lab05ProtocolToolBoundary.csproj
```

Before running, understand this:

MCP and UTCP both relate to tool calling, but they solve different integration problems.

| Area | MCP | UTCP |
|---|---|---|
| Primary style | Tool server protocol | Direct API/tool calling style |
| Best when | Tools/resources/prompts live behind a server boundary | Existing APIs should be invoked directly |
| Discovery | Server exposes tool metadata | API/tool description drives discovery |

Watch for:

- MCP tool metadata
- MCP server/tool attributes
- UTCP-style request shape

Why this matters:

You need to choose the right boundary for tool integration. Not every internal API needs MCP, and not every tool should be an ungoverned direct API call.

You are complete when:

- you can explain MCP as a tool-server boundary
- you can explain UTCP as direct API/tool calling style
- you can explain how this differs from A2A

Reference:

- https://modelcontextprotocol.io/docs/getting-started/intro

### Lab 06 - AgentGateway Baseline

Project:

```text
src\Lab06AgentGatewayClient
```

Dry run:

```powershell
$env:PN_AGENTGATEWAY_LIVE="false"
dotnet run --project .\src\Lab06AgentGatewayClient\Lab06AgentGatewayClient.csproj
```

Live run only if trainer asks:

```powershell
$env:PN_AGENTGATEWAY_ENDPOINT="https://agentgateway-an2607101.azurecontainerapps.io"
$env:PN_AGENTGATEWAY_LIVE="true"
dotnet run --project .\src\Lab06AgentGatewayClient\Lab06AgentGatewayClient.csproj
```

Before running, understand this:

AgentGateway is a runtime control point for model, tool, and agent traffic.

It can centralize:

- routes
- identity
- policy
- logs
- observability
- rate limits
- token/cost attribution

Important distinction:

```text
Azure API Center catalogs.
AgentGateway controls runtime traffic.
```

Watch for:

- model route request shape
- MCP-shaped request
- A2A-shaped request
- route/backend/policy config
- dry-run vs live mode

Why this matters:

As AI-native systems grow, teams need a runtime place to govern and observe model/tool/agent traffic.

You are complete when:

- you can explain what routes are needed
- you can explain why gateway logs matter
- you can explain why dry-run mode is acceptable when live gateway is unavailable

Reference:

- https://agentgateway.dev/docs/standalone/latest/

### Lab 07 - Gateway Observability and Control

Project:

```text
src\Lab07GatewayObservabilityControl
```

Dry run:

```powershell
$env:PN_AGENTGATEWAY_LIVE="false"
dotnet run --project .\src\Lab07GatewayObservabilityControl\Lab07GatewayObservabilityControl.csproj
```

Live run only if trainer asks:

```powershell
$env:PN_AGENTGATEWAY_ENDPOINT="https://agentgateway-an2607101.azurecontainerapps.io"
$env:PN_AGENTGATEWAY_LIVE="true"
$env:PN_RATE_LIMIT_BURST_COUNT="8"
dotnet run --project .\src\Lab07GatewayObservabilityControl\Lab07GatewayObservabilityControl.csproj
```

Before running, understand this:

Control without observability is incomplete. In AI-native systems, the answer is not the only artifact. The request path, trace IDs, policy decisions, rate limits, and cost attribution are also important.

Watch for:

| Signal | Meaning |
|---|---|
| `x-request-id` | Identifies one gateway request. |
| `x-correlation-id` | Connects related service calls. |
| `traceparent` | Connects distributed traces. |
| Rate-limit behavior | Shows runtime control. |
| Logs/KQL | Shows operations investigation path. |

Why this matters:

If you cannot trace a request across app, gateway, model, and logs, you cannot operate the system confidently.

You are complete when:

- you can explain why request and correlation IDs matter
- you can explain what `traceparent` is for
- you can explain where gateway logs fit
- you can explain how this prepares Day 5 operations and governance

Reference:

- https://learn.microsoft.com/en-us/azure/azure-monitor/logs/log-query-overview

## 10. Quick Troubleshooting

| Problem | Try this |
|---|---|
| Azure login fails | Run `az login`, then `az account show`. |
| Foundry project call fails | Check `AZURE_AI_PROJECT_ENDPOINT`; it must be project-scoped. |
| Model deployment error | Check `AZURE_OPENAI_CHAT_DEPLOYMENT=gpt-5-mini`. |
| Lab 01 does not call tools | Use the default prompt first. |
| Lab 02 approval does not pause | Use Windows PowerShell. |
| Lab 04 Cosmos checkpoint fails | Check `ENABLE_COSMOS_PERSISTENCE`, `COSMOS_SESSION_CONTAINER`, and RBAC with the trainer. |
| Cosmos returns no data | Check `COSMOS_DATABASE`, `COSMOS_CONTAINER`, and ask trainer if data is seeded. |
| Harness lab does not end | Check final output includes `DONE`. |
| A2A consumer fails | Make sure provider is running on `http://localhost:5063`. |
| Gateway live call fails | Use dry-run first; live gateway may not be enabled. |
| Expected rate limit does not happen | Ask trainer whether stricter gateway config is deployed. |

## 11. Completion Checklist

### Day 3

| Lab | Self-check |
|---|---|
| Lab 01 | I can explain goal, plan, action, observation, reflection, tools, middleware, and session checkpoint. |
| Lab 02 | I can explain flow, branching, approval, retry, and typed termination. |
| Lab 03 | I can explain four skill types and discovery/activation/execution. |
| Lab 04 | I can explain session, state, context, storage, serialization, Cosmos DB checkpointing, and compaction. |
| Lab 05 | I can explain what a harness does around a model. |
| Lab 06 | I can explain retrieval, grounding, verification, citation, retry, and Cosmos vs Azure AI Search vs HorizonDB. |
| Lab 07 | I can explain workflow agents and specialist agent roles. |

### Day 4

| Lab | Self-check |
|---|---|
| Lab 01 | I can compare sequential, concurrent, handoff, and group-chat coordination. |
| Lab 02 | I can explain coordinator-worker Magentic-style orchestration. |
| Lab 03 | I can explain Agent Card, A2A consumer, API Center, and AgentGateway. |
| Lab 04 | I can compare AG-UI and A2UI. |
| Lab 05 | I can compare MCP and UTCP. |
| Lab 06 | I can explain why traffic goes through AgentGateway. |
| Lab 07 | I can explain request ID, correlation ID, `traceparent`, rate limiting, and log evidence. |

## 12. Reference Links

These links are curated for student use. The trainer playbook has the full reference library.

| Topic | Link |
|---|---|
| Microsoft.Agents.AI namespace API reference | https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest |
| Agent Framework overview | https://learn.microsoft.com/en-us/agent-framework/overview/?pivots=programming-language-csharp |
| Agent Framework workflows | https://learn.microsoft.com/en-us/agent-framework/workflows/ |
| Agent Framework skills | https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp |
| Agent Framework conversations | https://learn.microsoft.com/en-us/agent-framework/agents/conversations/?pivots=programming-language-csharp |
| Agent Framework harness | https://learn.microsoft.com/en-us/agent-framework/agents/harness?pivots=programming-language-csharp |
| Cosmos DB .NET SDK | https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3 |
| A2A specification | https://a2a-protocol.org/latest/specification/ |
| AG-UI | https://docs.ag-ui.com/introduction |
| A2UI | https://a2ui.org/ |
| MCP | https://modelcontextprotocol.io/docs/getting-started/intro |
| AgentGateway | https://agentgateway.dev/docs/standalone/latest/ |
