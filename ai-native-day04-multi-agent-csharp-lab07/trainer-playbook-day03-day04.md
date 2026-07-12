# Trainer Playbook - Day 3 and Day 4

Program: ProNative AI-Native Fullstack Engineering  
Weekend: Day 3 and Day 4  
Batch: `AN-2607-101`  
Primary stack: C#, Microsoft Agent Framework, Azure AI Foundry, Cosmos DB, Azure API Center, A2A, AG-UI/A2UI, MCP, UTCP, AgentGateway on Azure Container Apps

This is the trainer's single delivery reference for Day 3 and Day 4. It absorbs the previous section-by-section content and the lab polls. The only other active markdown files should be the dry-run checklist and the student playbook.

## 1. Weekend Storyline

Day 3 and Day 4 are one connected progression:

```text
single agent
  -> agentic reasoning loop
  -> typed flow engineering
  -> reusable skills
  -> state and memory
  -> harness and evidence
  -> retrieval-grounded workflow
  -> workflow agent
  -> multi-agent topologies
  -> Magentic-style coordination
  -> A2A/API Center interoperability
  -> AG-UI/A2UI user boundary
  -> MCP/UTCP tool boundary
  -> AgentGateway runtime control
  -> observability, rate limits, and cost attribution
```

The weekend goal is not to show isolated samples. The goal is to teach students how enterprise agentic systems are structured, controlled, observed, and evolved.

Trainer opening:

```text
Day 1 gave us model access and grounding.
Day 2 gave us agents, instructions, tools, and MCP.
Day 3 asks how we make agentic work controlled, repeatable, observable, and safe.
Day 4 asks how those controlled systems collaborate across agents, protocols, and gateways.
```

## 2. Non-Negotiable Delivery Rules

| Rule | Trainer action |
|---|---|
| Use official Microsoft Agent Framework capabilities | Do not replace official APIs with custom loops, fake agents, or hand-built substitutes. |
| Keep live Azure labs honest | If Foundry, Cosmos DB, A2A, or AgentGateway is not ready, fail clearly and explain the blocker. |
| Keep local-first labs purposeful | Some labs are local to reduce cost and isolate framework concepts; do not call them simulations. |
| Explain the boundary | For every lab, name the boundary: model, tool, flow, skill, memory, user, protocol, gateway, or observability. |
| Separate catalog from runtime | Azure API Center is catalog/governance/discovery. A2A and AgentGateway are runtime paths. |
| Do not overrun Day 4 concepts into Day 3 | Day 3 prepares the single-agent and workflow base; Day 4 expands into multi-agent and protocol boundaries. |
| Use standard observability during training | Custom ProNative workbooks are for live project days. |
| Polls are trainer-only | Use polls to diagnose understanding; do not put the poll bank into the student playbook. |

### 2.1 `Microsoft.Agents.AI` API Reference Orientation

Use this section when introducing students to the official .NET API reference:

- https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest

Trainer framing:

```text
The Microsoft.Agents.AI namespace page is the API catalog for Agent Framework building blocks.
It is not a tutorial. Use it to recognize the official types behind agents, sessions, skills, context providers, loop evaluators, approvals, memory, evaluation, and telemetry.
```

Important trainer note:

```text
Microsoft Learn marks portions of the Agent Framework API documentation as prerelease.
Teach students to use the API reference together with the verified starter code for this batch.
Do not improvise against unverified APIs during delivery.
```

Student focus areas to highlight:

| Focus area | API types to point students to | Why students should care |
|---|---|---|
| Agent abstraction | `AIAgent`, `AgentResponse`, `AgentResponse<T>`, `AgentRunOptions`, `AIAgentBuilder` | Core mental model for running agents and receiving responses. |
| Sessions and state | `AgentSession`, `AgentSessionStateBag`, `ProviderSessionState<TState>` | Explains how state travels across agent runs. |
| Context providers | `AIContext`, `AIContextProvider`, `MessageAIContextProvider` | Explains how extra context is injected into agent calls. |
| Chat history and memory | `ChatHistoryProvider`, `InMemoryChatHistoryProvider`, `CosmosChatHistoryProvider`, `ChatHistoryMemoryProvider` | Explains local, Cosmos-backed, and retrieval-oriented memory patterns. |
| Skills | `AgentSkill`, `AgentFileSkill`, `AgentInlineSkill`, `AgentClassSkill<TSelf>`, `AgentSkillsProvider`, `AgentSkillsProviderBuilder`, `AgentSkillsSource` | Connects directly to Day 3 Lab 03. |
| Loop and reflection | `LoopAgent`, `LoopEvaluator`, `DelegateLoopEvaluator`, `CompletionMarkerLoopEvaluator`, `AIJudgeLoopEvaluator` | Explains bounded reflection and retry. |
| Tool approval | `ToolApprovalAgent`, `ToolApprovalAgentOptions`, `AlwaysApproveToolApprovalResponseContent` | Explains trust boundaries around tool execution. |
| Harness-adjacent providers | `FileAccessProvider`, `FileMemoryProvider`, `TodoProvider`, `AgentModeProvider` | Supports Day 3 Lab 05 harness explanation. |
| Evaluation | `AgentEvaluationExtensions`, `AgentEvaluationResults`, `EvalItem`, `EvalChecks`, `LocalEvaluator`, `ExpectedToolCall` | Connects to optional evaluation and Day 5 operations. |
| Telemetry and logging | `LoggingAgent`, `OpenTelemetryAgent`, `OpenTelemetryAgentBuilderExtensions` | Connects to Day 4 observability and Day 5 operations. |
| RAG support concepts | `TextSearchProvider`, `ChatHistoryMemoryProvider` | Helps students see where retrieval/context providers fit, even though Lab 06 uses Cosmos DB directly. |

Lab-to-API examples:

| Lab | API reference areas to mention |
|---|---|
| Day 3 Lab 01 - Agentic reasoning loop | `AIAgent`, `AgentResponse`, `LoopAgent`, `DelegateLoopEvaluator`, `ToolApprovalAgent`, `AgentSession`, `AgentSessionStateBag` |
| Day 3 Lab 02 - Flow engineering | Agent APIs plus workflow APIs from Agent Framework workflow docs; keep `Microsoft.Agents.AI` as agent/session support reference |
| Day 3 Lab 03 - Skill-driven development | `AgentSkill`, `AgentFileSkill`, `AgentInlineSkill`, `AgentClassSkill<TSelf>`, `AgentSkillsProviderBuilder`, `AgentSkillsSource` |
| Day 3 Lab 04 - Conversations, state, memory | `AgentSession`, `AgentSessionStateBag`, `ProviderSessionState<TState>`, `AIContextProvider`, `InMemoryChatHistoryProvider`, `CosmosChatHistoryProvider` |
| Day 3 Lab 05 - Harness engineering | `ToolApprovalAgent`, `LoopEvaluator`, `FileAccessProvider`, `FileMemoryProvider`, `TodoProvider`, `AgentModeProvider`, plus Harness package docs |
| Day 3 Lab 06 - Retrieval-grounded RAG | `TextSearchProvider` and `ChatHistoryMemoryProvider` as reference concepts; starter code uses explicit Cosmos DB retrieval for the alpha batch |
| Day 3 Lab 07 - Workflow agent | `AIAgent`, `AgentResponse`, `AgentResponseUpdate`, sessions, and workflow-specific docs |
| Day 4 Lab 01 - Multi-agent architecture | `AIAgent`, response/session types, plus workflow orchestration docs |
| Day 4 Lab 02 - Magentic-style orchestration | `AIAgent`, response/session types, plus Magentic workflow builder docs |
| Day 4 Lab 03 - A2A | A2A JSON utility and A2A packages, plus A2A protocol/docs |
| Day 4 Lab 07 - Gateway observability | `LoggingAgent`, `OpenTelemetryAgent`, `OpenTelemetryAgentBuilderExtensions` |

Suggested trainer narration:

```text
When you are reading the starter code, do not only ask "what does this line do?"
Ask which Agent Framework capability it belongs to: agent, session, context, skill, loop, approval, memory, evaluation, or telemetry.
The API reference is your map of those capabilities.
```

## 3. Environment Baseline

### 3.1 Local Machine

| Requirement | Check |
|---|---|
| Windows PowerShell | Use PowerShell from the repo root. |
| .NET SDK 10 | `dotnet --version` returns `10.x`. |
| Azure CLI | `az account show` succeeds. |
| VS Code | Useful for walkthrough, not required for console run. |
| Network | NuGet restore and Azure endpoints must be reachable. |

### 3.2 Batch Defaults

| Item | Value |
|---|---|
| Batch ID | `AN-2607-101` |
| Default student ID | `ST-2606-1000` |
| Region | `Central India` |
| Foundry project endpoint | `https://proj-an2607101-default-resource.services.ai.azure.com/api/projects/proj-an2607101-default` |
| Azure OpenAI endpoint | `https://proj-an2607101-default-resource.openai.azure.com/` |
| Model deployment | `gpt-5-mini` |
| Cosmos DB endpoint | `https://cosmos-an2607101.documents.azure.com:443/` |
| Cosmos DB knowledge container | `db-an2607101-training` / `training-knowledge` |
| Cosmos DB Lab 04 checkpoint container | `db-an2607101-training` / `agent-session-checkpoints` |
| API Center | `apic-an2607101-fec2ed` |
| API Center runtime URL | `https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms` |
| Local A2A provider URL | `http://localhost:5063` |
| AgentGateway endpoint | `https://agentgateway-an2607101.azurecontainerapps.io` |

### 3.3 Resource Groups

| Resource group | Purpose | Trainer dry-run check |
|---|---|---|
| `rg-ai-shared-platform-an2607101` | Foundry, model deployment, Cosmos DB, API Center | Confirm Foundry/model/Cosmos/API Center exist. |
| `rg-ai-observability-an2607101` | Log Analytics and Application Insights | Confirm KQL access for Day 4 Lab 07. |
| `rg-ai-governance-hub-an2607101` | AgentGateway and control plane | Confirm gateway endpoint and managed identity permissions if live gateway path is used. |
| Student-specific RGs | Student attribution and later live-project isolation | Confirm student IDs and RBAC where applicable. |

### 3.4 Access

| Principal | Scope | Access required | Used by |
|---|---|---|---|
| Trainer | Shared platform RG | Contributor | Setup and troubleshooting |
| Trainer/student | Foundry project/resource | Azure AI Developer plus Cognitive Services OpenAI User or equivalent | Day 3 Labs 01, 06, 07; Day 4 Labs 01, 02, 03 |
| Trainer/student | Azure OpenAI endpoint | Cognitive Services OpenAI User or equivalent | Day 3 Lab 05 |
| Trainer | Cosmos DB | Cosmos DB Built-in Data Contributor | Day 3 Lab 04 checkpoint setup and Day 3 Lab 06 seed/setup |
| Student/test identity | Cosmos DB | Cosmos DB Built-in Data Reader or Data Contributor based on run mode | Day 3 Lab 04 checkpoint read/write and Day 3 Lab 06 retrieval |
| Trainer/platform owner | Azure API Center | Permission to register/update inventory | Day 3 Lab 03 optional and Day 4 Lab 03 |
| AgentGateway managed identity | Foundry/Azure AI backend | Model-call permission and project access | Day 4 Labs 06-07 |

## 4. Common Setup Commands

From the workspace root:

```powershell
cd C:\Users\Madan\Documents\Codex\2026-06-29\we-are-now-focusing-on-pronative-2
az account show
dotnet --version
```

Day 3 repo:

```powershell
cd C:\Users\Madan\Documents\Codex\2026-06-29\we-are-now-focusing-on-pronative-2\outputs\starter-repositories\ai-native-day03-agent-framework-csharp
```

Day 4 repo:

```powershell
cd C:\Users\Madan\Documents\Codex\2026-06-29\we-are-now-focusing-on-pronative-2\outputs\starter-repositories\ai-native-day04-multi-agent-csharp
```

Common live-lab variables:

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

Day 3 Lab 03 optional API Center/MCP:

```powershell
$env:ENABLE_APIC_MCP_SKILLS="true"
$env:APIC_RUNTIME_URL="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
$env:APIC_MCP_ENDPOINT="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
```

Day 4 Lab 03 A2A:

```powershell
$env:A2A_BASE_URL="http://localhost:5063"
$env:A2A_AGENT_CARD_PATH="/a2a/training-ops/v1/card"
```

Day 4 Labs 06-07 AgentGateway:

```powershell
$env:PN_AGENTGATEWAY_ENDPOINT="https://agentgateway-an2607101.azurecontainerapps.io"
$env:PN_AGENTGATEWAY_LIVE="false"
$env:PN_MODEL_DEPLOYMENT="gpt-5-mini"
$env:PN_RATE_LIMIT_BURST_COUNT="8"
```

Use `PN_AGENTGATEWAY_LIVE="true"` only when the gateway is deployed and ready.

## 5. Build Dry Run

Day 3:

```powershell
dotnet build .\src\Lab01AgenticReasoningLoop\Lab01AgenticReasoningLoop.csproj
dotnet build .\src\Lab02FlowEngineering\Lab02FlowEngineering.csproj
dotnet build .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
dotnet build .\src\Lab04ConversationsMemory\Lab04ConversationsMemory.csproj
dotnet build .\src\Lab05HarnessEngineering\Lab05HarnessEngineering.csproj
dotnet build .\src\Lab06HybridRagWorkflow\Lab06HybridRagWorkflow.csproj
dotnet build .\src\Lab07WorkflowAgent\Lab07WorkflowAgent.csproj
```

Day 4:

```powershell
dotnet build .\src\Lab01MultiAgentArchitecture\Lab01MultiAgentArchitecture.csproj
dotnet build .\src\Lab02CoordinatorWorkerAgents\Lab02CoordinatorWorkerAgents.csproj
dotnet build .\src\Lab03A2AAgentExposure\Lab03A2AAgentExposure.csproj
dotnet build .\src\Lab03A2AAgentConsumer\Lab03A2AAgentConsumer.csproj
dotnet build .\src\Lab04AgentUserInteractionBoundary\Lab04AgentUserInteractionBoundary.csproj
dotnet build .\src\Lab05ProtocolToolBoundary\Lab05ProtocolToolBoundary.csproj
dotnet build .\src\Lab06AgentGatewayClient\Lab06AgentGatewayClient.csproj
dotnet build .\src\Lab07GatewayObservabilityControl\Lab07GatewayObservabilityControl.csproj
dotnet build .\src\Lab07MultiAgentEvaluation\Lab07MultiAgentEvaluation.csproj
```

## 6. Day 3 Delivery Structure

| Time | Section | Mode |
|---:|---|---|
| 30 min | Opening and continuity | Concept + setup |
| 55 min | Lab 01: Agentic AI reasoning loop | Concept + lab |
| 65 min | Lab 02: Flow engineering | Concept + lab |
| 55 min | Lab 03: Skill-driven development | Concept + lab |
| 55 min | Lab 04: Conversations, state, and memory | Concept + lab |
| 60 min | Lab 05: Harness engineering | Concept + lab |
| 75 min | Lab 06: Retrieval-grounded RAG workflow | Concept + lab |
| 55 min | Lab 07: Workflow agent | Concept + lab |
| 30 min | Review and Day 4 bridge | Discussion |

## 7. Day 3 Detailed Trainer Guide

### Day 3 Opening

Board flow:

```text
model call
  -> agent with tools
  -> agentic loop
  -> typed workflow
  -> reusable skills
  -> state and memory
  -> harness
  -> grounded workflow
  -> workflow agent
```

Opening message:

```text
We start small: one agent, one goal, one reasoning loop.
Then every section adds one enterprise control layer.
```

### Lab 01 - Agentic AI Reasoning Loop

Project:

```text
src\Lab01AgenticReasoningLoop
```

Run:

```powershell
dotnet run --project .\src\Lab01AgenticReasoningLoop\Lab01AgenticReasoningLoop.csproj
```

Core concept:

```text
goal -> plan -> action -> observation -> reflection -> checkpoint
```

What "Foundry-backed agent" means:

- The C# app runs locally.
- The agent is created in code with Microsoft Agent Framework.
- `AIProjectClient` connects to the Azure AI Foundry project endpoint.
- `projectClient.AsAIAgent(...)` creates an Agent Framework agent powered by the live `gpt-5-mini` deployment.
- The app owns tools, middleware, approval, loop evaluation, and local session state.
- No portal-created Foundry Agent is required.
- This is not a fake local response.

Trainer-only poll:

```text
In an enterprise agentic loop, what is the most important reason to separate goal, plan, action, observation, and reflection?
Preferred answer: To make each agent step observable, testable, and controllable.
```

Code touchpoints:

| Component | Why it matters |
|---|---|
| `AIProjectClient` | Project-scoped Foundry connection. |
| `AzureCliCredential` | Uses signed-in identity and RBAC. |
| `projectClient.AsAIAgent(...)` | Creates Foundry-backed Agent Framework agent. |
| `AIFunctionFactory.Create(...)` | Turns C# functions into tools. |
| Agent middleware | Logs and governs the run boundary. |
| Function middleware | Validates and governs tool execution. |
| `UseToolApproval(...)` | Adds trust gate before tool execution. |
| `LoopAgent` | Adds bounded reflection and retry. |
| `AgentSession.StateBag` | Stores local checkpoint metadata. |

Trainer talk track:

1. This is not yet a full workflow. This is the smallest observable agentic loop.
2. The model is allowed to reason, but the system controls tools, approval, and retry.
3. Every agentic system needs to expose what it thought, what it did, what it saw, and why it stopped.
4. The checkpoint is not a database yet. It is the local state contract we can persist later.

Acceptance:

- Live Foundry call succeeds.
- Model selects and invokes tools.
- Tool approval appears.
- Final answer includes `Goal`, `Plan`, `Action`, `Observation`, and `Reflection`.
- Session checkpoint metadata is visible.

References:

- https://learn.microsoft.com/en-us/agent-framework/agents/middleware/
- https://learn.microsoft.com/en-us/agent-framework/agents/conversations/

### Lab 02 - Flow Engineering

Project:

```text
src\Lab02FlowEngineering
```

Run:

```powershell
dotnet run --project .\src\Lab02FlowEngineering\Lab02FlowEngineering.csproj
```

Core concept:

```text
Flow engineering is the design of execution paths around agents, tools, decisions, approvals, retries, state, and termination.
```

Trainer-only poll:

```text
When should a typed workflow be preferred over a single free-form agent prompt?
Preferred answer: When the process has explicit steps, branching, approval, retry, or termination rules.
```

Board flow:

```text
Intake
  -> classify request
  -> branch by risk
  -> request approval when needed
  -> execute approved path
  -> retry if recoverable
  -> emit typed final result
```

Code touchpoints:

| Component | Why it matters |
|---|---|
| `WorkflowBuilder` | Defines workflow graph. |
| Typed `Executor<TInput,TOutput>` | Gives each step clear contract. |
| `AddSwitch(...)` | Branching is explicit. |
| `RequestPort` / `RequestInfoEvent` | Implements real HITL approval. |
| `ExternalResponse` | Feeds approval back into workflow. |
| Workflow state | Captures facts across steps. |
| `WorkflowOutputEvent` | Emits typed final result. |

Trainer talk track:

1. Lab 01 showed the loop. Lab 02 turns the loop into a process.
2. If a business process has approvals, retries, or branch rules, it should not live only in a prompt.
3. The model can still help, but the workflow owns the process contract.
4. Human approval must be real. A mocked approval teaches the wrong lesson.

Acceptance:

- Workflow starts and prints events.
- Approval prompt pauses for input.
- Approved and rejected paths can be explained.
- Retry path is visible.
- Final typed result is emitted.

References:

- https://learn.microsoft.com/en-us/agent-framework/workflows/
- https://learn.microsoft.com/en-us/agent-framework/workflows/state

### Lab 03 - Skill-Driven Development

Project:

```text
src\Lab03SkillDrivenDevelopment
```

Run local path:

```powershell
dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
```

Optional API Center/MCP path:

```powershell
$env:ENABLE_APIC_MCP_SKILLS="true"
$env:APIC_RUNTIME_URL="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
$env:APIC_MCP_ENDPOINT="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
```

Core concept:

```text
Skill-driven development packages reusable capability so agents can discover, activate, execute, test, and reuse it.
```

Trainer-only poll:

```text
What is the best mental model for agent skills in an enterprise platform?
Preferred answer: Skills are reusable capabilities that can be discovered, activated, executed, versioned, and tested.
```

Skill types:

| Skill type | Training purpose |
|---|---|
| File-based skills | Portable skill instructions and reference files. |
| Inline skills | Small in-code skill definitions. |
| Class-based skills | Reusable typed C# skill classes. |
| MCP/API Center skills | Cataloged or remotely discoverable skills. |

Azure API Center clarification:

```text
Azure API Center is catalog and governance. It is not the runtime executor.
Runtime loading through MCP requires an MCP server or endpoint that exposes the skill package.
```

Acceptance:

- File skills load.
- Inline and class-based skills are visible.
- Optional MCP/API Center path is clearly marked optional.
- Students can explain discovery, activation, execution, and reuse.

References:

- https://learn.microsoft.com/en-us/agent-framework/agents/skills
- https://learn.microsoft.com/en-us/azure/api-center/register-discover-skills

### Lab 04 - Conversations, State, and Memory

Project:

```text
src\Lab04ConversationsMemory
```

Run local session path:

```powershell
dotnet run --project .\src\Lab04ConversationsMemory\Lab04ConversationsMemory.csproj
```

Optional Cosmos checkpoint path:

```powershell
$env:ENABLE_COSMOS_PERSISTENCE="true"
$env:COSMOS_SESSION_CONTAINER="agent-session-checkpoints"
dotnet run --project .\src\Lab04ConversationsMemory\Lab04ConversationsMemory.csproj
```

Trainer setup if needed:

```powershell
$env:COSMOS_CREATE_IF_NOT_EXISTS="true"
```

Core concept:

```text
Memory is not magic. It is a set of explicit state boundaries.
```

Trainer-only poll:

```text
What should Cosmos DB persistence add to the Lab 04 Agent Framework session model?
Preferred answer: It stores a durable checkpoint of the serialized session so state can survive process restarts.
```

Code touchpoints:

| Component | Why it matters |
|---|---|
| `AgentSession` | Conversation continuity boundary. |
| `AgentSession.StateBag` | Serializable task/session metadata. |
| `AIContextProvider` | Injects session-scoped context. |
| `ProviderSessionState<T>` | Typed provider state. |
| `InMemoryChatHistoryProvider` | Framework-managed local chat history. |
| `MessageCountingChatReducer` | Controls history size. |
| `SerializeSessionAsync(...)` / `DeserializeSessionAsync(...)` | Restart-safe checkpoint. |
| `CosmosClient` | Optional durable checkpoint store. |
| `CompactionProvider` | Controls context growth. |

Trainer talk track:

1. The local chat client avoids model cost. The framework concepts are the important part.
2. Do not store secrets in session state.
3. Cosmos DB is not replacing the Agent Framework session model. It persists the serialized checkpoint.
4. Compaction controls context size. It does not replace retrieval or memory design.

Acceptance:

- Session state is visible.
- Context provider remembers facts.
- Chat history is stored.
- Session serialization/deserialization works.
- Cosmos checkpoint upsert/read works when enabled.
- Compaction behavior is visible.

References:

- https://learn.microsoft.com/en-us/agent-framework/agents/conversations/
- https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3

### Lab 05 - Harness Engineering

Project:

```text
src\Lab05HarnessEngineering
```

Run:

```powershell
dotnet run --project .\src\Lab05HarnessEngineering\Lab05HarnessEngineering.csproj
```

Core concept:

```text
The model produces reasoning and tool requests. The harness manages operational scaffolding around long-running agent work.
```

Trainer-only poll:

```text
What does an agent harness primarily provide around a model?
Preferred answer: Runtime controls such as tools, approvals, memory/file access, loop evaluation, compaction, evidence, and telemetry.
```

Non-negotiable code evidence:

- `PackageReference Include="Microsoft.Agents.AI.Harness"`
- `AsHarnessAgent(new HarnessAgentOptions { ... })`
- `HarnessAgentOptions`

Trainer talk track:

1. Harness is not another name for a prompt.
2. Harness is what makes longer-running agent work inspectable.
3. Output is not only final text. Evidence is part of the product.
4. This is where evaluation and operations start to connect.

Evaluation framing:

| Layer | Use |
|---|---|
| `Microsoft.Extensions.AI.Evaluation` | Lightweight C# evaluation close to code. |
| Azure AI Evaluation SDK / Foundry evaluation | Enterprise evaluation and platform-level assessment. |

Acceptance:

- Official harness path is used.
- Output includes evidence/evaluation/completion.
- Final response ends with `DONE`.
- Evidence artifact is written.

References:

- https://learn.microsoft.com/en-us/agent-framework/agents/harness

### Lab 06 - Retrieval-Grounded RAG for Agentic Workflow

Project:

```text
src\Lab06HybridRagWorkflow
```

Trainer seed/setup:

```powershell
$env:COSMOS_CREATE_IF_NOT_EXISTS="true"
$env:COSMOS_SEED_SAMPLE_DATA="true"
dotnet run --project .\src\Lab06HybridRagWorkflow\Lab06HybridRagWorkflow.csproj -- --seed
```

Run:

```powershell
dotnet run --project .\src\Lab06HybridRagWorkflow\Lab06HybridRagWorkflow.csproj
```

Core concept:

```text
RAG is not one Azure service. RAG is a grounding design pattern.
```

Trainer-only poll:

```text
Why does this lab place retrieval inside an agentic workflow instead of calling retrieval once and answering immediately?
Preferred answer: To make retrieval, answer generation, verification, retry, and citations visible as separate steps.
```

Retrieval options:

| Option | Position |
|---|---|
| Azure AI Search | Covered earlier; strong for dedicated document and hybrid search; removed from Day 3 runtime to reduce weekend cost. |
| Cosmos DB | Hands-on backend for structured/semi-structured operational grounding. |
| HorizonDB | Concept-only for PostgreSQL-native vector/full-text/hybrid retrieval because of cost and regional constraints. |

Code touchpoints:

| Component | Why it matters |
|---|---|
| `WorkflowBuilder` | Orchestrates retrieval, answer, verification, retry, finalization. |
| `AIProjectClient.AsAIAgent(...)` | Foundry-backed answer generation. |
| `AIFunctionFactory.Create(...)` | Exposes `retrieve_training_context` as a tool. |
| `CosmosClient` | Connects to live Cosmos DB grounding store. |
| `Container.GetItemQueryIterator<T>(...)` | Retrieves training knowledge records. |
| Citation verifier | Checks markers like `[doc:<id>]`. |
| Typed final output | Makes the result inspectable. |

Trainer test prompts:

| Test | Prompt | Expected grounding |
|---|---|---|
| Smoke test | `Why are we using Cosmos DB for Lab 06 instead of Azure AI Search?` | `pn-d3-cosmos-grounding`, `pn-alpha-cost-control`, `pn-d3-retrieval-options` |
| Retrieval comparison | `Compare Azure AI Search, Cosmos DB, and HorizonDB for retrieval-grounded agentic apps in this course.` | `pn-d3-retrieval-options`, `pn-alpha-cost-control` |
| Tool boundary | `Why should retrieval be exposed as a tool instead of hidden inside a prompt?` | `pn-d3-cosmos-grounding` |
| Skills link | `What is the relationship between agent skills, API Center, and MCP runtime loading?` | `pn-d3-agent-skills` |
| State link | `Explain how AgentSession, StateBag, serialization, compaction, and Cosmos DB persistence fit together.` | `pn-d3-state-memory` |
| Negative test | `What exact GPU model and hourly price are used by this batch for HorizonDB?` | Should say evidence is not available. |

Acceptance:

- Cosmos DB records are seeded.
- Retrieval returns records.
- Answer cites records.
- Verification accepts or routes to retry.
- Final artifact contains retrieved record IDs and status.

References:

- https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3
- https://learn.microsoft.com/en-us/azure/search/search-what-is-azure-search
- https://learn.microsoft.com/en-us/azure/horizondb/ai/ai-search-overview
- https://learn.microsoft.com/en-us/azure/horizondb/ai/vector-search-pgvector

### Lab 07 - Workflow Agent

Project:

```text
src\Lab07WorkflowAgent
```

Run:

```powershell
dotnet run --project .\src\Lab07WorkflowAgent\Lab07WorkflowAgent.csproj
```

Core concept:

```text
Workflow agents are the bridge between single-agent control flow and Day 4 multi-agent systems.
```

Trainer-only poll:

```text
What is the main value of using specialist agents inside a structured workflow?
Preferred answer: Each specialist can own a clear role while the workflow preserves process order and output structure.
```

Code touchpoints:

| Component | Why it matters |
|---|---|
| `AIProjectClient.AsAIAgent(...)` | Creates Foundry-backed specialist agents. |
| `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)` | Uses official sequential agent workflow. |
| `WithChainOnlyAgentResponses(true)` | Controls what moves through the chain. |
| `WithIntermediateOutputFrom(...)` | Makes specialist outputs visible. |
| `WithOutputFrom(...)` | Declares terminal output. |
| `InProcessExecution.RunStreamingAsync(...)` | Shows workflow events. |
| Structured output | Normalizes final recommendation. |

Acceptance:

- Live Foundry calls succeed.
- Specialist outputs are visible.
- Streaming workflow events are visible.
- Final structured result is visible.

References:

- https://learn.microsoft.com/en-us/agent-framework/workflows/agents-in-workflows

### Day 3 Close

Review questions:

1. What is the difference between an agentic loop and a workflow?
2. Why is HITL approval part of flow engineering?
3. How do skills differ from ordinary helper functions?
4. What does Cosmos DB add to Lab 04 state and memory?
5. What does a harness add around the model?
6. Why is RAG better as an explicit workflow?
7. Why do workflow agents prepare us for multi-agent systems?

Close:

```text
Today we engineered the operating path around one agent:
control flow, skills, state, harness, grounding, and workflow agents.
Tomorrow we distribute this across multiple agents and runtime boundaries.
```

## 8. Day 4 Delivery Structure

| Time | Section | Mode |
|---:|---|---|
| 30 min | Day 3 recap and Day 4 framing | Discussion |
| 60 min | Lab 01: Multi-agent architecture | Concept + lab |
| 60 min | Lab 02: Magentic-style orchestration | Concept + lab |
| 70 min | Lab 03: A2A exposure, discovery, and consumption | Concept + lab |
| 45 min | Lab 04: AG-UI / A2UI boundary | Concept + lab |
| 50 min | Lab 05: MCP vs UTCP tool boundary | Concept + lab |
| 65 min | Lab 06: AgentGateway baseline | Concept + lab |
| 65 min | Lab 07: Gateway observability and control | Concept + lab |
| 25 min | Optional multi-agent evaluation / Day 5 bridge | Discussion |

## 9. Day 4 Detailed Trainer Guide

### Day 4 Opening

Opening:

```text
Yesterday we engineered the path around one agent.
Today we distribute work across agents and runtime boundaries.
The goal is not more agents. The goal is clearer responsibility and stronger control.
```

Board flow:

```text
workflow agent
  -> multiple specialists
  -> orchestration topology
  -> protocol boundary
  -> gateway route
  -> observable runtime
```

### Lab 01 - Multi-Agent Architecture

Project:

```text
src\Lab01MultiAgentArchitecture
```

Run:

```powershell
dotnet run --project .\src\Lab01MultiAgentArchitecture\Lab01MultiAgentArchitecture.csproj
```

Core concept:

```text
Multi-agent architecture is a coordination decision, not a headcount decision.
```

Trainer-only poll:

```text
Which coordination style is best when several agents can work independently and their results can be merged afterward?
Preferred answer: Concurrent orchestration.
```

Topology map:

| Topology | Use when | Risk if misused |
|---|---|---|
| Sequential | Order matters and each step depends on previous output. | Too slow for independent work. |
| Concurrent | Specialists can work in parallel. | Harder merge/conflict handling. |
| Handoff | Ownership changes based on responsibility or condition. | Poor routing causes lost context. |
| Group chat | Bounded discussion or critique is valuable. | Can become noisy and expensive. |

Acceptance:

- Native orchestration classes are used.
- Students can compare all four topologies.
- Workflow events show agent responses and outputs.

References:

- https://learn.microsoft.com/en-us/agent-framework/workflows/
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

Core concept:

```text
Magentic-style orchestration gives one coordinator the job of planning, delegating, monitoring, recovering, and synthesizing.
```

Trainer-only poll:

```text
What is the key role of the coordinator in Magentic-style orchestration?
Preferred answer: To plan, assign work to specialists, monitor progress, and synthesize results.
```

Code touchpoints:

- `AgentWorkflowBuilder.CreateMagenticBuilderWith(...)`
- participants/workers
- max rounds
- max resets
- max stalls
- streaming Magentic events

Acceptance:

- Native Magentic builder path is used.
- Coordinator and worker roles are visible.
- Rounds, stalls, and resets are explainable.

References:

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

Optional gateway route:

```powershell
$env:A2A_BASE_URL="https://agentgateway-an2607101.azurecontainerapps.io"
$env:A2A_AGENT_CARD_PATH="/a2a/training-ops/v1/card"
dotnet run --project .\src\Lab03A2AAgentConsumer\Lab03A2AAgentConsumer.csproj
```

Core concept:

```text
A2A lets one agent or client discover another agent's capabilities and send it a message through a defined runtime contract.
```

Trainer-only poll:

```text
What problem does an Agent Card solve in an A2A scenario?
Preferred answer: It describes an agent's identity, capabilities, endpoint, and interaction contract so another agent/client can discover and call it.
```

Runtime vs catalog:

| Item | Role |
|---|---|
| A2A provider | Runtime agent endpoint. |
| Agent Card | Runtime discovery and capability contract. |
| A2A consumer | Client/agent that resolves the card and sends a message. |
| Azure API Center | Catalog/governance metadata for enterprise discovery. |
| AgentGateway | Optional runtime route/control point. |

Acceptance:

- Provider starts on `http://localhost:5063`.
- Agent Card endpoint responds.
- Consumer resolves card.
- Consumer sends message using A2A SDK.
- API Center metadata is reviewed.

References:

- https://a2a-protocol.org/latest/specification/
- https://www.nuget.org/packages/A2A/
- https://www.nuget.org/packages/Microsoft.Agents.AI.Hosting.A2A.AspNetCore/
- https://learn.microsoft.com/en-us/azure/api-center/register-discover-skills

### Lab 04 - AG-UI / A2UI Agent-User Interaction Boundary

Project:

```text
src\Lab04AgentUserInteractionBoundary
```

Run:

```powershell
dotnet run --project .\src\Lab04AgentUserInteractionBoundary\Lab04AgentUserInteractionBoundary.csproj
```

Core concept:

```text
Agent-user interaction is not only final text. It includes state, progress, tool calls, approvals, interrupts, and resumes.
```

Trainer-only poll:

```text
What is the main reason to discuss AG-UI and A2UI separately from A2A?
Preferred answer: A2A is for agent-to-agent interoperability, while AG-UI/A2UI focus on agent-to-user interaction patterns.
```

Comparison:

| Boundary | Focus |
|---|---|
| A2A | Agent-to-agent discovery and messaging. |
| AG-UI | Event-driven agent-to-UI runtime interaction. |
| A2UI | Declarative/adaptive agent-to-UI payload concept. |

Acceptance:

- Students can explain interrupt/resume.
- Students can compare AG-UI and A2UI.
- Students can identify where approval belongs in user interaction.

References:

- https://docs.ag-ui.com/introduction
- https://a2ui.org/

### Lab 05 - MCP vs UTCP Tool Boundary

Project:

```text
src\Lab05ProtocolToolBoundary
```

Run:

```powershell
dotnet run --project .\src\Lab05ProtocolToolBoundary\Lab05ProtocolToolBoundary.csproj
```

Core concept:

```text
MCP and UTCP both relate to tools, but they represent different integration styles.
```

Trainer-only poll:

```text
When is MCP usually the better fit compared with direct API-style tool calling?
Preferred answer: When tools are exposed through a tool server with a standard discovery and invocation boundary.
```

Comparison:

| Area | MCP | UTCP |
|---|---|---|
| Primary style | Tool server protocol | Direct API/tool calling style |
| Best when | Tools/resources/prompts live behind a server boundary | Existing APIs should be invoked directly |
| Discovery | Server exposes tool metadata | API/tool description drives discovery |
| Training depth | Practical C# concept | Conceptual comparison |

Acceptance:

- MCP tool metadata is visible.
- UTCP-style request shape is visible.
- Students can explain direct API vs tool-server boundary.

References:

- https://modelcontextprotocol.io/docs/getting-started/intro
- https://csharp.sdk.modelcontextprotocol.io/
- https://github.com/universal-tool-calling-protocol

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

Live run:

```powershell
$env:PN_AGENTGATEWAY_ENDPOINT="https://agentgateway-an2607101.azurecontainerapps.io"
$env:PN_AGENTGATEWAY_LIVE="true"
dotnet run --project .\src\Lab06AgentGatewayClient\Lab06AgentGatewayClient.csproj
```

Core concept:

```text
AgentGateway is where runtime traffic becomes routable, observable, policy-controlled, and attributable.
```

Trainer-only poll:

```text
Why route model, agent, or tool traffic through an AgentGateway in an enterprise AI platform?
Preferred answer: To centralize routing, identity, policy, observability, rate limits, and cost/token controls.
```

Landing-zone position:

| Component | Role |
|---|---|
| Azure AI Foundry | Model and agent platform. |
| Azure API Center | Catalog/governance metadata. |
| AgentGateway | Runtime route/policy/control. |
| Azure Monitor/App Insights | Logs and observability. |

Acceptance:

- Dry-run prints model, MCP, and A2A request shapes.
- Gateway YAML/config is reviewed.
- Live call works if gateway is deployed.

References:

- https://agentgateway.dev/docs/standalone/latest/
- https://agentgateway.dev/docs/standalone/latest/llm/providers/azure/
- https://agentgateway.dev/docs/standalone/latest/llm/observability/

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

Live run:

```powershell
$env:PN_AGENTGATEWAY_ENDPOINT="https://agentgateway-an2607101.azurecontainerapps.io"
$env:PN_AGENTGATEWAY_LIVE="true"
$env:PN_RATE_LIMIT_BURST_COUNT="8"
dotnet run --project .\src\Lab07GatewayObservabilityControl\Lab07GatewayObservabilityControl.csproj
```

Core concept:

```text
In AI-native systems, the answer is not the only artifact. The request path, policy decisions, trace IDs, rate limits, and cost attribution are also artifacts.
```

Trainer-only poll:

```text
Which signal is most useful for tracing one user request across app, gateway, model, and logs?
Preferred answer: Correlation ID / request ID / traceparent propagated across the call path.
```

Signals:

| Signal | Why it matters |
|---|---|
| `x-request-id` | Identifies one gateway request. |
| `x-correlation-id` | Connects related service calls. |
| `traceparent` | Connects distributed traces. |
| token/cost headers | Supports FinOps and attribution where configured. |
| rate-limit response | Shows runtime control. |
| Azure Container Apps logs | Shows gateway runtime evidence. |
| KQL query | Shows operations investigation path. |

Acceptance:

- Baseline request is visible.
- Burst probe demonstrates rate limiting if strict config is deployed.
- KQL can find request/correlation evidence.
- Students understand why gateway logs matter for Day 5 operations/governance.

References:

- https://agentgateway.dev/docs/standalone/latest/configuration/resiliency/rate-limits/
- https://learn.microsoft.com/en-us/azure/container-apps/log-monitoring
- https://learn.microsoft.com/en-us/azure/azure-monitor/logs/log-query-overview

### Optional Extension - Multi-Agent Evaluation

Optional project:

```text
src\Lab07MultiAgentEvaluation
```

Use this only if time permits or as a short walkthrough. It connects Day 4 multi-agent behavior to Day 5 evaluation and operations.

Trainer-only poll:

```text
What makes multi-agent evaluation different from evaluating a single model answer?
Preferred answer: You evaluate the final answer plus coordination quality, role adherence, evidence use, handoffs, and failure handling.
```

Evaluation dimensions:

| Dimension | Question |
|---|---|
| Task success | Did the system solve the user goal? |
| Role adherence | Did each agent stay within its role? |
| Handoff quality | Was responsibility transferred correctly? |
| Evidence use | Did agents use evidence instead of unsupported claims? |
| Coordination cost | Was the number of agent turns justified? |
| Failure handling | Did the system stop, retry, or escalate correctly? |

### Day 4 Close And Day 5 Bridge

Close with:

```text
Days 3 and 4 taught us how to build controlled agentic and multi-agent systems.
Day 5 asks how to operate, govern, secure, observe, evaluate, and optimize them.
```

Preview Day 5:

- GenAIOps
- LLMOps
- observability and App Insights
- governance and Agent 365
- security and Zero Trust for AI
- FinOps for AI
- hybrid AI and Foundry Local/DGX Spark/neocloud concepts

## 10. Troubleshooting

| Symptom | Likely cause | Trainer recovery |
|---|---|---|
| `az account show` fails | Not logged in or wrong tenant | Run `az login`; verify subscription/tenant. |
| Foundry 404 | Raw Azure OpenAI endpoint used instead of project endpoint | Use `https://...services.ai.azure.com/api/projects/...`. |
| Model deployment error | Wrong deployment name or RBAC missing | Confirm `gpt-5-mini` and role assignments. |
| Lab 01 no tool calls | Prompt changed or model did not follow instruction | Use default prompt first. |
| Lab 02 approval does not pause | Running in terminal that does not accept input | Use Windows PowerShell. |
| Lab 03 API Center skill path fails | Optional MCP/API Center endpoint not ready | Keep local skill path; explain optional live path. |
| Lab 04 Cosmos checkpoint fails | Check `ENABLE_COSMOS_PERSISTENCE`, `COSMOS_SESSION_CONTAINER`, partition key `/batchId`, and RBAC | Use local checkpoint path first; retry with trainer `COSMOS_KEY` if RBAC is not ready. |
| Lab 05 harness fails to compile | Wrong package restore or SDK | Restore packages; confirm `Microsoft.Agents.AI.Harness`. |
| Lab 06 returns zero records | Cosmos not seeded or wrong env vars | Seed with `COSMOS_SEED_SAMPLE_DATA=true`. |
| A2A consumer fails | Provider not running or card path wrong | Start provider; confirm `/a2a/training-ops/v1/card`. |
| Gateway live call fails | Gateway not deployed/auth missing | Use dry-run mode; review endpoint/auth. |
| Expected 429 does not appear | Strict Lab 07 route not deployed | Explain config dependency; show YAML and dry-run evidence. |

## 11. Weekend Acceptance

By the end of Day 3/4, students should understand:

- agentic reasoning loop with tools, middleware, approval, reflection, and checkpoint
- typed workflow with branching, HITL, retry, and termination
- skill-driven development across file, inline, class-based, and MCP/API Center paths
- session, state, context providers, serialization, Cosmos checkpoint, and compaction
- official Harness usage and evidence capture
- Cosmos-backed retrieval-grounded workflow
- workflow agents and structured outputs
- multi-agent topologies and Magentic-style coordination
- A2A exposure/discovery/consumption and API Center catalog distinction
- AG-UI/A2UI agent-user interaction boundary
- MCP vs UTCP tool boundary
- AgentGateway runtime routing, observability, rate limits, and cost attribution

## 12. Full Reference Library

| Topic | Reference |
|---|---|
| Microsoft Agents hub | https://learn.microsoft.com/en-us/agents/ |
| What is agentic AI | https://www.microsoft.com/en-us/software-development-companies/resources/articles/what-is-agentic-ai |
| Microsoft.Agents.AI namespace API reference | https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest |
| Agent Framework overview | https://learn.microsoft.com/en-us/agent-framework/overview/?pivots=programming-language-csharp |
| Agent Framework workflows | https://learn.microsoft.com/en-us/agent-framework/workflows/ |
| Agents in workflows | https://learn.microsoft.com/en-us/agent-framework/workflows/agents-in-workflows |
| Agent Framework middleware | https://learn.microsoft.com/en-us/agent-framework/agents/middleware/?pivots=programming-language-csharp |
| Agent Framework skills | https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp |
| Agent Framework conversations | https://learn.microsoft.com/en-us/agent-framework/agents/conversations/?pivots=programming-language-csharp |
| Agent Framework harness | https://learn.microsoft.com/en-us/agent-framework/agents/harness?pivots=programming-language-csharp |
| Azure API Center skills/assets | https://learn.microsoft.com/en-us/azure/api-center/register-discover-skills |
| Cosmos DB .NET SDK | https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3 |
| Azure AI Search overview | https://learn.microsoft.com/en-us/azure/search/search-what-is-azure-search |
| HorizonDB AI search overview | https://learn.microsoft.com/en-us/azure/horizondb/ai/ai-search-overview |
| HorizonDB embeddings | https://learn.microsoft.com/en-us/azure/horizondb/ai/generate-vector-embeddings |
| HorizonDB pgvector | https://learn.microsoft.com/en-us/azure/horizondb/ai/vector-search-pgvector |
| A2A specification | https://a2a-protocol.org/latest/specification/ |
| A2A .NET SDK | https://www.nuget.org/packages/A2A/ |
| Agent Framework A2A hosting package | https://www.nuget.org/packages/Microsoft.Agents.AI.Hosting.A2A.AspNetCore/ |
| AG-UI | https://docs.ag-ui.com/introduction |
| A2UI | https://a2ui.org/ |
| MCP | https://modelcontextprotocol.io/docs/getting-started/intro |
| MCP C# SDK | https://csharp.sdk.modelcontextprotocol.io/ |
| UTCP | https://github.com/universal-tool-calling-protocol |
| AgentGateway | https://agentgateway.dev/docs/standalone/latest/ |
| AgentGateway Azure provider | https://agentgateway.dev/docs/standalone/latest/llm/providers/azure/ |
| AgentGateway observability | https://agentgateway.dev/docs/standalone/latest/llm/observability/ |
| AgentGateway rate limits | https://agentgateway.dev/docs/standalone/latest/configuration/resiliency/rate-limits/ |
| Azure Container Apps logs | https://learn.microsoft.com/en-us/azure/container-apps/log-monitoring |
| Azure Monitor log queries | https://learn.microsoft.com/en-us/azure/azure-monitor/logs/log-query-overview |
