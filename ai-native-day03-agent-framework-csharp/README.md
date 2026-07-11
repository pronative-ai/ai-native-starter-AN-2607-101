# AI-Native Day 3 - Microsoft Agent Framework C# Labs

This is the corrected Day 3 starter repository.

It is generated lab by lab. Only completed and reviewed labs should be added here.

## Current Labs

| Lab | Project | Status |
|---|---|---|
| Lab 01 | `Lab01AgenticReasoningLoop` | Generated and build-verified |
| Lab 02 | `Lab02FlowEngineering` | Generated, build-verified, and smoke-tested |
| Lab 03 | `Lab03SkillDrivenDevelopment` | Generated, build-verified, and smoke-tested |
| Lab 04 | `Lab04ConversationsMemory` | Generated, build-verified, and smoke-tested |
| Lab 05 | `Lab05HarnessEngineering` | Generated and build-verified |
| Lab 06 | `Lab06HybridRagWorkflow` | Generated and build-verified |
| Lab 07 | `Lab07WorkflowAgent` | Generated and build-verified |

## Non-Negotiable Implementation Rule

These labs must use Microsoft Agent Framework directly.

No custom agent abstractions, fake reasoning loops, or simulated tool execution should be used for lab implementation.

## Lab 01 Components

Lab 01 uses the Agent Framework runtime directly:

- Foundry-backed `AsAIAgent(...)`
- `AIAgent.AsBuilder().Use(...)` run middleware
- Function invocation middleware
- `IChatClient` middleware through Foundry `clientFactory`
- `AIFunctionFactory.Create(...)` tools
- `UseToolApproval(...)` approval boundary
- `LoopAgent` + `DelegateLoopEvaluator` reflection loop
- `AgentSession.StateBag` checkpoint boundary

Review `docs/component-alignment.md` before approving Lab 02, because it records which concepts remain in Lab 01 and which move to Flow Engineering, Skill-driven Development, and State/Memory.

For the proposed Microsoft Agent Framework component mapping for Labs 02-07, review `docs/day03-labs-02-07-component-mapping.md`.

Review `docs/lab02-flow-engineering.md` before running Lab 02. It explains how the workflow graph, HITL approval, retry route, workflow state, and final output map to the code.

Review `docs/lab03-skill-driven-development.md` before running Lab 03. It explains file-based, inline, class-based, and MCP/API Center Agent Skills, plus provider tool boundaries and approval guidance.

Review `docs/lab04-conversations-memory.md` before running Lab 04. It explains `AgentSession`, context providers, in-memory chat history storage, session serialization, and compaction.

Review `docs/lab05-harness-engineering.md` before running Lab 05. It explains the official `Microsoft.Agents.AI.Harness` implementation, required code evidence, live Azure OpenAI client path, and trainer review checklist.

Review `docs/lab06-hybrid-rag-workflow.md` before running Lab 06. It explains the Agent Framework workflow, Foundry-backed answer agent, Azure AI Search retrieval tool, grounding verification, retry path, and hybrid provider boundary.

Review `docs/lab07-workflow-agent.md` before running Lab 07. It explains the native `AgentWorkflowBuilder` sequential workflow, four Foundry-backed specialist agents, intermediate/terminal workflow events, and structured output normalization.

## Required Local Setup

```powershell
az login
dotnet --version
```

The expected .NET SDK is `10.0.100` or compatible.

## Required Foundry Settings

Default values are aligned to batch `AN-2607-101`:

```powershell
$env:AZURE_AI_PROJECT_ENDPOINT="https://proj-an2607101-default-resource.services.ai.azure.com/api/projects/proj-an2607101-default"
$env:AZURE_OPENAI_ENDPOINT="https://proj-an2607101-default-resource.openai.azure.com/"
$env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
$env:BATCH_ID="AN-2607-101"
$env:STUDENT_ID="ST-2606-1000"
$env:AZURE_SEARCH_ENDPOINT="https://srchan2607101.search.windows.net"
$env:AZURE_SEARCH_INDEX_NAME="idx-st26061000-rag"
```

## Run Lab 01

```powershell
dotnet run --project .\src\Lab01AgenticReasoningLoop\Lab01AgenticReasoningLoop.csproj
```

This lab performs a live Foundry-backed Agent Framework run. If authentication, RBAC, model deployment, or project endpoint is not ready, the lab should fail clearly rather than returning a simulated answer.

## Run Lab 02

```powershell
dotnet run --project .\src\Lab02FlowEngineering\Lab02FlowEngineering.csproj
```

This lab is local-first and uses Microsoft Agent Framework Workflows. It pauses for real console approval when the request is operational or sensitive.

## Run Lab 03

```powershell
dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
```

This lab is local-first and uses native Microsoft Agent Framework skills. It demonstrates file-based, inline, class-based, and API Center/MCP published skill sources without model cost, then shows how the same provider boundary maps to agent tool approval.

Optional API Center MCP connection:

```powershell
$env:ENABLE_APIC_MCP_SKILLS="true"
$env:APIC_RUNTIME_URL="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
$env:APIC_MCP_ENDPOINT="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
```

## Run Lab 04

```powershell
dotnet run --project .\src\Lab04ConversationsMemory\Lab04ConversationsMemory.csproj
```

This lab is local-first and uses Microsoft Agent Framework conversation primitives. It demonstrates `AgentSession`, `AIContextProvider`, `InMemoryChatHistoryProvider`, session serialization/restoration, and compaction without model cost.

## Run Lab 05

```powershell
dotnet run --project .\src\Lab05HarnessEngineering\Lab05HarnessEngineering.csproj
```

This lab uses the official Microsoft Agent Framework Harness directly. It demonstrates `AsHarnessAgent(new HarnessAgentOptions { ... })`, live Azure OpenAI `IChatClient`, tool approval, loop evaluators, compaction, file memory/access, todo/mode providers, OpenTelemetry, and evidence capture.

## Run Lab 06

```powershell
dotnet run --project .\src\Lab06HybridRagWorkflow\Lab06HybridRagWorkflow.csproj
```

This lab uses Microsoft Agent Framework Workflows with a live Foundry-backed agent and Azure AI Search retrieval. It demonstrates `WorkflowBuilder`, typed executors, `AIFunctionFactory.Create(...)` retrieval tooling, `SearchClient.SearchAsync<SearchDocument>(...)`, grounding verification, bounded retry, and typed workflow output.

## Run Lab 07

```powershell
dotnet run --project .\src\Lab07WorkflowAgent\Lab07WorkflowAgent.csproj
```

This lab uses native Microsoft Agent Framework workflow-agent orchestration. It demonstrates `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)`, `WithChainOnlyAgentResponses(true)`, `WithIntermediateOutputFrom(...)`, `WithOutputFrom(...)`, Foundry-backed specialist agents, workflow events, and official structured output with `GetResponseAsync<WorkflowAgentStructuredResult>(...)`.
