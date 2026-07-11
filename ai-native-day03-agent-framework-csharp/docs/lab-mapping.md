# Day 3 Corrected Lab Mapping

## Lab 01 - Agentic AI Reasoning Loop

Project:

`src/Lab01AgenticReasoningLoop`

Purpose:

Students observe a real Agent Framework run that follows the agentic loop:

1. goal
2. plan
3. action
4. observation
5. reflection

Implementation requirements:

- Use `AIProjectClient`
- Use `AzureCliCredential`
- Use `projectClient.AsAIAgent(...)`
- Use `AIAgent.AsBuilder().Use(...)` for Agent Framework run middleware
- Use function invocation middleware for tool governance
- Use `IChatClient` middleware through the Foundry `clientFactory` hook
- Use `AIFunctionFactory.Create(...)` for real action/tool invocation
- Use `ToolApprovalAgent` / `UseToolApproval(...)` for trust and approval boundaries
- Use `LoopAgent` with `DelegateLoopEvaluator` for bounded reflection
- Use `AgentSession.StateBag` as the serializable local checkpoint boundary
- Let the model choose and invoke the tool
- Do not return a simulated answer if live Foundry is unavailable

Tools used:

`build_milestone_plan`

This tool returns a lightweight milestone graph for the requested goal. It is intentionally not the full WorkflowBuilder implementation; Lab 02 upgrades this into formal Agent Framework workflows.

`get_batch_readiness_signal`

This tool returns structured ProNative batch readiness information. The agent must call it during the action step and use the returned result as the observation.

`record_reasoning_checkpoint`

This tool records a local reasoning checkpoint contract. Lab 04 will upgrade this state boundary to persistent memory/checkpoint storage.

Middleware demonstrated:

- Agent run middleware: logs run entry/exit and writes run metadata to `AgentSession.StateBag`.
- Function-calling middleware: validates tool invocation before execution and blocks destructive tool names.
- `IChatClient` middleware: wraps the underlying Foundry-backed chat client through `clientFactory` to capture request/response timing without replacing the Foundry runtime.

See `component-alignment.md` for the exact component choices and options for Lab 02 onward.

## Lab 02 - Flow Engineering

Project:

`src/Lab02FlowEngineering`

Purpose:

Students build and inspect a deterministic Agent Framework workflow:

1. intake
2. classify
3. route
4. approve when required
5. execute
6. retry once when policy evidence is missing
7. terminate with a typed result

Implementation requirements:

- Use `Microsoft.Agents.AI.Workflows` pinned to `1.13.0`.
- Use `WorkflowBuilder` for the workflow graph.
- Use typed `Executor<TInput,TOutput>` classes for individual flow steps.
- Use `AddSwitch(...)` for conditional branching.
- Use `RequestPort`, `RequestInfoEvent`, and `ExternalResponse` for real human-in-the-loop approval.
- Use `QueueStateUpdateAsync(...)` and `ReadStateAsync(...)` for workflow state.
- Use a bounded retry route through `RetryPlannerExecutor`.
- Use `WorkflowOutputEvent` with a typed `FinalFlowResult` as the termination contract.

See `lab02-flow-engineering.md` for trainer-facing walkthrough notes.

## Lab 03 - Agent Skills

Project:

`src/Lab03SkillDrivenDevelopment`

Purpose:

Students implement the Agent Skills lifecycle:

1. discovery
2. activation
3. execution
4. reuse

Implementation requirements:

- Use `Microsoft.Agents.AI` pinned to `1.13.0`.
- Use `Microsoft.Agents.AI.Mcp` pinned to `1.13.0-alpha.260703.1` for MCP skills.
- Use `AgentSkillsProviderBuilder` as the provider boundary.
- Use `UseFileSkill(...)` for file-based skills.
- Use `AgentInlineSkill` for code-defined inline skills.
- Use `AgentClassSkill<T>` for class-based compiled skills.
- Use `UseMcpSkills(...)` for API Center/MCP published skills when enabled.
- Show provider tool names: `load_skill`, `read_skill_resource`, and `run_skill_script`.
- Keep read-only skill access separate from script execution approval.

Skills used:

- File-based: `training-delivery-readiness`
- File-based: `student-support-triage`
- Inline: `training-session-followup-v1`
- Class-based: `student-environment-check-v1`
- API Center/MCP published: `pronative-training-operations-skill`
- API Center/MCP published: `pronative-student-environment-skill`

API Center runtime URL:

`https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms`

See `lab03-skill-driven-development.md` for trainer-facing walkthrough notes.

## Lab 04 - Conversations, State, and Memory

Project:

`src/Lab04ConversationsMemory`

Purpose:

Students learn how Agent Framework keeps conversation/task continuity:

1. session
2. context providers
3. storage
4. serialization
5. compaction

Implementation requirements:

- Use `Microsoft.Agents.AI` pinned to `1.13.0`.
- Use `AgentSession` as the conversation continuity boundary.
- Use `AgentSession.StateBag` for serializable metadata.
- Use `AIContextProvider` and `ProviderSessionState<T>` for session-scoped memory.
- Use `InMemoryChatHistoryProvider` for local history stored in the session.
- Use `MessageCountingChatReducer` for history size control.
- Use `SerializeSessionAsync(...)` and `DeserializeSessionAsync(...)` for checkpointing.
- Use `CompactionProvider`, `SlidingWindowCompactionStrategy`, `TruncationCompactionStrategy`, and `PipelineCompactionStrategy`.
- Keep service-managed Foundry conversation state as a production comparison, not the default implementation for this lab.

See `lab04-conversations-memory.md` for trainer-facing walkthrough notes.

## Lab 05 - Harness Engineering

Project:

`src/Lab05HarnessEngineering`

Purpose:

Students learn that a harness is the runtime around the model:

1. tool invocation
2. history persistence
3. mode and todo tracking
4. file memory and file access
5. approval
6. observability
7. looping
8. repeatable evidence capture

Implementation requirements:

- Use `Microsoft.Agents.AI.Harness` pinned to `1.13.0-preview.260703.1`.
- Use `AzureOpenAIClient` with `AzureCliCredential`.
- Use `GetChatClient(config.ModelDeployment).AsIChatClient()` for the real model client.
- Use `AsHarnessAgent(new HarnessAgentOptions { ... })`.
- Use `AIFunctionFactory.Create(...)` for `assess_harness_readiness` and `capture_harness_evidence`.
- Use `HarnessAgentOptions.ChatOptions.Tools` for tool exposure.
- Use `HarnessAgentOptions.ToolApprovalAgentOptions` for safe tool approval.
- Use `HarnessAgentOptions.LoopEvaluators` with `CompletionMarkerLoopEvaluator("DONE")`.
- Use `HarnessAgentOptions.MaxContextWindowTokens` and `MaxOutputTokens` for harness compaction.
- Keep `DisableTodoProvider = false` and `DisableAgentModeProvider = false`.
- Keep `DisableFileMemory = false`, `DisableFileAccess = false`, and configure `FileMemoryStore` / `FileAccessStore`.
- Keep `DisableOpenTelemetry = false` and configure `OpenTelemetrySourceName`.
- Write an evaluation artifact to the harness file store.
- Do not use a scripted `IChatClient`, manual `ChatClientAgent`, or a hand-built harness substitute.

See `lab05-harness-engineering.md` for trainer-facing walkthrough notes.

## Lab 06 - Hybrid RAG for Agentic Workflow

Project:

`src/Lab06HybridRagWorkflow`

Purpose:

Students learn how retrieval, answer generation, verification, retry, and final output can be made explicit in an agentic workflow.

Implementation requirements:

- Use `Microsoft.Agents.AI.Workflows` pinned to `1.13.0`.
- Use `WorkflowBuilder` for the RAG orchestration graph.
- Use typed `Executor<TInput,TOutput>` classes for retrieval planning, grounded answering, verification, retry planning, and finalization.
- Use `InProcessExecution.RunStreamingAsync(...)` and `WorkflowOutputEvent` for event visibility and typed termination.
- Use `IWorkflowContext.QueueStateUpdateAsync(...)` for request, plan, answer, verification, and retry state.
- Use `AIProjectClient` and `projectClient.AsAIAgent(...)` for the Foundry-backed answer agent.
- Use `AIFunctionFactory.Create(...)` to expose retrieval as a tool.
- Use `SearchClient.SearchAsync<SearchDocument>(...)` to retrieve live grounding evidence from Azure AI Search.
- Require answer citations in `[doc:<id>]` format and verify citation/evidence alignment.
- Do not hardcode answers, simulate retrieval, or use local-only retrieval as the main path.

Azure AI Search defaults:

- Search endpoint: `https://srchan2607101.search.windows.net`
- Student index pattern: `idx-st26061000-rag`, `idx-st26061001-rag`, etc.
- Preferred auth: `AzureCliCredential` / RBAC
- Trainer fallback auth: `AZURE_SEARCH_ADMIN_KEY`

See `lab06-hybrid-rag-workflow.md` for trainer-facing walkthrough notes.

## Lab 07 - Workflow Agent

Project:

`src/Lab07WorkflowAgent`

Purpose:

Students learn how to implement a structured business process as a workflow composed of real Agent Framework agents.

Implementation requirements:

- Use `Microsoft.Agents.AI.Workflows` pinned to `1.13.0`.
- Use `AIProjectClient` and `projectClient.AsAIAgent(...)` for each specialist Foundry-backed agent.
- Use `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)` as the primary workflow-agent implementation.
- Use `SequentialWorkflowBuilder.WithChainOnlyAgentResponses(true)` for clean handoff between specialist agents.
- Use `WithIntermediateOutputFrom(...)` for the first three specialist agents.
- Use `WithOutputFrom(...)` for the finalizer agent.
- Use `InProcessExecution.RunStreamingAsync(...)` for workflow execution.
- Observe `AgentResponseUpdateEvent`, `AgentResponseEvent`, and `WorkflowOutputEvent`.
- Use `ChatClientStructuredOutputExtensions.GetResponseAsync<T>(...)` and `ChatResponse<T>.TryGetResult(...)` for the final typed result.
- Do not build a manual `agent.RunAsync(...)` chain as the main workflow.
- Do not use `Workflow.as_agent()` because it is not verified in the installed C# package surface.

Agent sequence:

1. Intent Analyst Agent
2. Planning Agent
3. Risk Reviewer Agent
4. Finalizer Agent

See `lab07-workflow-agent.md` for trainer-facing walkthrough notes.
