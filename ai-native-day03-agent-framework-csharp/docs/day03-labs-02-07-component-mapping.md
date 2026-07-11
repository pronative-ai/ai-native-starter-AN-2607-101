# Day 3 Labs 02-07: Microsoft Agent Framework Component Mapping

This mapping records implemented decisions for Lab 02 and review decisions for the remaining Day 3 code.

Lab 01 is already regenerated as an Agent Framework-first lab. The remaining labs should deepen the same stack without inventing custom agent APIs.

## Current Package Boundary

Currently available in the local package cache:

- `Microsoft.Agents.AI`
- `Microsoft.Agents.AI.Abstractions`
- `Microsoft.Agents.AI.Foundry`
- `Microsoft.Agents.AI.Harness` 1.13.0-preview.260703.1
- `Microsoft.Agents.AI.OpenAI`
- `Microsoft.Agents.AI.Workflows` 1.13.0

Expected additions for later labs:

- `Microsoft.SemanticKernel` only if Lab 03 should explicitly show Semantic Kernel `KernelPlugin` patterns.
- Dapr packages only if Lab 04 includes a runnable Dapr actor checkpoint sample. Otherwise Dapr remains a deployment/runtime note for Azure Container Apps.

## Verified API Notes Before Code Generation

| Item | Current Status | Code Generation Rule |
|---|---|---|
| Middleware scope | Microsoft Agent Framework documents three C# middleware layers: Agent Run middleware, Function Calling middleware, and `IChatClient` middleware | Lab 01 now demonstrates all three layers; Lab 05 should revisit them in harness/evaluation mode with richer evidence capture. |
| `AgentSession.StateBag` | Verified in local `Microsoft.Agents.AI.Abstractions` 1.13.0 XML | Safe to use after compile verification. Use `StateBag.SetValue(...)`, `GetValue(...)`, and `Serialize()`, not a non-existent `session.State`. |
| Agent cleanup | Local `Azure.AI.Projects` 2.1.0-beta.3 README shows `AgentAdministrationClient.DeleteAgentVersionAsync(...)`, not `DeleteAgentAsync(...)` | Do not build against `DeleteAgentAsync` unless the target SDK version proves it exists. Use verified agent-version cleanup API or mark cleanup as trainer manual. |
| `Workflow.as_agent()` | Not verified in the local C# package surface | Treat as unconfirmed. Do not build against it. Describe workflow-as-agent only as a conceptual boundary unless verified later. |
| Native skills | `AgentSkillsProvider`, `AgentSkillsProviderBuilder`, `AgentFileSkill`, `AgentInlineSkill`, and `AgentClassSkill<T>` are present in local `Microsoft.Agents.AI` 1.13.0 XML | Lab 03 uses native skills provider and pins the package version. |
| MCP skills | `UseMcpSkills(...)` is available from `Microsoft.Agents.AI.Mcp` 1.13.0-alpha.260703.1 | Lab 03 includes API Center/MCP published skills, disabled by default and enabled by environment flag. |

## Lab 02 - Flow Engineering

Goal: Build a typed workflow with steps, branching, retry, and termination.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Typed workflow graph | Make the flow explicit instead of relying on a prompt-only plan | `WorkflowBuilder` | Build a business process graph from typed executors |
| Processing steps | Each step owns one responsibility | `Executor<TInput,TOutput>` | Intake, classify, approve, execute, summarize |
| Branching | Route based on deterministic state or model output | Workflow edges and conditional edges | Low-risk path continues; high-risk path goes to approval |
| Retry | Re-enter a failed step with bounded attempts | Executor + workflow state | Store attempt count in workflow state; stop at max retry |
| Termination | End cleanly with a final output contract | `WorkflowOutputEvent` / final executor | Emit structured result and reason |
| Human approval | Pause the workflow for a real trainer/student decision | Real HITL executor/event | Approval must wait for console input or external approval callback; do not auto-approve or mock approval |
| Observability | Let students see the flow happen | `InProcessExecution.RunStreamingAsync(...)`, `WorkflowEvent` | Print executor events, output events, and final decision |
| State between steps | Share values safely across executors | `IWorkflowContext.QueueStateUpdateAsync(...)`, `ReadStateAsync(...)` | Store classification, approval status, retry count |

Implemented project: `Lab02FlowEngineering`

Recommended enterprise scenario: ProNative AI training request triage.

Flow:

1. `IntakeExecutor`: accepts a training operations request.
2. `ClassifyExecutor`: classifies request as informational, operational, or sensitive.
3. `RiskDecisionExecutor`: decides whether approval is needed.
4. `ApprovalExecutor`: performs real human-in-the-loop approval using console input first; later can be replaced by approval service callback.
5. `ExecutionExecutor`: performs the approved action.
6. `SummaryExecutor`: emits final workflow result.

Implemented decision:

- Uses official `Microsoft.Agents.AI.Workflows` package pinned to `1.13.0`.
- Introduces `WorkflowBuilder`, typed executors, switch routing, workflow state, streaming events, and termination.
- Uses real HITL through `RequestPort`, `RequestInfoEvent`, and `ExternalResponse`; no mocked or default-approved branch.

## Lab 03 - Agent Skills

Goal: Implement Discovery, Activation, and Execution for reusable skills.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Skill discovery | Find available capabilities before invoking them | `AgentSkillsProvider` / `AgentSkillsProviderBuilder` | Use native skills provider instead of a custom registry |
| File-based skill packaging | Package skills as folders with `SKILL.md` and resources | `UseFileSkill(...)`, `AgentFileSkill` | Include two file-based skills in the starter pack |
| Inline skill packaging | Define runtime-created skills in code | `AgentInlineSkill` | Add a session follow-up skill |
| Class-based skill packaging | Define compiled reusable C# skill logic | `AgentClassSkill<T>` | Add a student environment check skill |
| MCP/API Center skill publishing | Discover skills from a remote MCP source | `UseMcpSkills(...)`, `AgentMcpSkillsSourceOptions`, `ModelContextProtocol.Client` | Use API Center runtime URL as the published skills boundary |
| Skill execution | Let the agent call the selected capability | `AgentSkillsProvider` exposed tools | Show load/read/run provider tools; direct script execution only for inline/class examples |
| Skill versioning | Avoid ambiguous or stale tool behavior | Agent Skills metadata + package pinning | Pin `Microsoft.Agents.AI` version and include skill version in metadata |
| Skill governance | Block unsafe or unapproved skills | `UseToolApproval(...)` + function middleware | Native skills provider tools require approval; use read-only auto-approval rules only where safe |
| Skill reuse | Same skill can be used by another lab/project | File-backed or class-backed skills | Keep skill package separate from agent code |

Implemented project: `Lab03SkillDrivenDevelopment`

Recommended enterprise scenario: Training operations skills.

Skills:

- file-based `training-delivery-readiness`
- file-based `student-support-triage`
- inline `training-session-followup-v1`
- class-based `student-environment-check-v1`
- API Center/MCP `pronative-training-operations-skill`
- API Center/MCP `pronative-student-environment-skill`

Implementation options:

| Option | What It Shows | Package Impact | Recommendation |
|---|---|---|---|
| Native Agent Skills Provider | Discovery/activation/execution using `AgentSkillsProvider` and native skill types | Uses `Microsoft.Agents.AI` 1.13.0 verified locally | Required primary implementation |
| Lightweight manifest fallback | File metadata plus `AIFunctionFactory` | Uses current packages | Only fallback if native provider API changes or restore fails |
| Semantic Kernel `KernelPlugin` | Literal plugin model with `[KernelFunction]` | Requires `Microsoft.SemanticKernel` | Useful optional comparison, not mandatory for Day 3 |

Implemented decision:

- Uses native `AgentSkillsProviderBuilder`, `UseFileSkill(...)`, `AgentClassSkill<T>`, `AgentInlineSkill`, and `UseMcpSkills(...)`.
- Pins `Microsoft.Agents.AI` to `1.13.0`.
- Pins `Microsoft.Agents.AI.Mcp` to `1.13.0-alpha.260703.1`.
- Demonstrates file-based skill packaging with real `SKILL.md` folders.
- Demonstrates activation through `GetContentAsync(...)` for code-defined/class skills and resource files for file-based skills.
- Demonstrates direct execution through native inline/class skill scripts.
- Includes API Center runtime URL `https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms` for MCP published skills.
- Shows the provider tool boundary: `load_skill`, `read_skill_resource`, and `run_skill_script`.
- Keeps script execution positioned as an approval-controlled action.
- Does not add Semantic Kernel in this lab; SK remains optional comparison material only.

## Lab 04 - State and Memory

Goal: Store conversation/task state using Agent Framework conversation primitives first, then map the same boundary to Cosmos DB or Dapr later.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Session state | Persist run-scoped metadata | `AgentSession.StateBag` | Store batch ID, student ID, task ID, current status |
| Context provider memory | Inject and extract session facts across turns | `AIContextProvider`, `ProviderSessionState<T>` | Store preferred language, current issue, and task in provider state |
| Local history storage | Keep conversation history in the session | `InMemoryChatHistoryProvider` | Store local chat history under a named state key |
| History reduction | Keep local history bounded | `MessageCountingChatReducer` | Reduce history before retrieval so students see cost/context control |
| Serializable checkpoint | Save and reload agent session boundary | `SerializeSessionAsync(...)`, `DeserializeSessionAsync(...)` | Demonstrate local JSON checkpoint first |
| Context compaction | Reduce long-running conversation growth | `CompactionProvider`, `PipelineCompactionStrategy`, `SlidingWindowCompactionStrategy`, `TruncationCompactionStrategy` | Run explicit compaction over a long message list |
| Durable memory | Enterprise persistence beyond process lifetime | Cosmos DB container | Save serialized session/checkpoint document keyed by `BatchId`, `StudentId`, `TaskId` |
| Actor checkpoint | Long-running task lifecycle | Dapr actor state on Azure Container Apps | Optional deployment path; not required for local-only Lab 04 |

Implemented project: `Lab04ConversationsMemory`

Recommended enterprise scenario: Long-running student environment remediation task.

Implemented sequence:

1. Create an `AgentSession` and write training metadata to `StateBag`.
2. Use `TrainingPreferenceContextProvider` to store and inject session-scoped memory.
3. Use `InMemoryChatHistoryProvider` with `MessageCountingChatReducer`.
4. Serialize the session to local JSON and restore it.
5. Continue the conversation from restored session state.
6. Run explicit compaction with sliding-window and truncation strategies.

Implemented decision:

- Keep Lab 04 runnable locally first and avoid model cost.
- Use a minimal deterministic `IChatClient` only as a no-cost model stand-in; do not implement custom memory/session APIs.
- Use official Agent Framework session, context provider, history provider, reducer, serialization, and compaction APIs.
- Keep Cosmos DB and Dapr as enterprise deployment upgrade paths after students understand the session boundary.

## Lab 05 - Harness Engineering and Evaluation

Goal: Run repeatable prompts, compare outputs, capture traces/evidence, and introduce evaluation.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Harness runtime | Show the complete agent scaffold around a model | `AsHarnessAgent(new HarnessAgentOptions { ... })` | Official harness package is the primary implementation |
| Tool calling loop | Agent can repeatedly call tools to finish a task | `HarnessAgentOptions.ChatOptions.Tools` | Add small approved tool set |
| Mode and todo tracking | Plan/execute behavior and multi-step progress | Harness default `AgentModeProvider` and `TodoProvider` | Keep disable flags false |
| Context management | Keep long-running tasks inside context limits | Harness compaction via `MaxContextWindowTokens` and `MaxOutputTokens` | Configure token budget and compaction through `HarnessAgentOptions` |
| Safety and approval | Prevent tool execution without policy | `HarnessAgentOptions.ToolApprovalAgentOptions` | Auto-approve only safe lab tools and read-only file access |
| Observability | Capture trace evidence | `HarnessAgentOptions.OpenTelemetrySourceName` | Configure OpenTelemetry through the harness |
| Middleware completeness | Teach full Agent Framework runtime scope | Harness-owned function invocation, tool approval, context providers, and OpenTelemetry | Do not manually compose the same behavior |
| Repeatability | Same prompt set can be rerun | Prompt harness runner | Store inputs, outputs, timestamps, model/deployment |
| Local C# evaluation | Developer-side scoring | `Microsoft.Extensions.AI.Evaluation` | Score relevance, groundedness, coherence where practical |
| Enterprise evaluation | Foundry/enterprise-grade eval path | Azure AI Evaluation SDK / Foundry evaluation | Position as production evaluation workflow |

Recommended project: `Lab05HarnessEngineering`

Recommended enterprise scenario: Evaluate a training-support agent before using it in live projects.

Implemented project: `Lab05HarnessEngineering`

Implementation decision:

- Uses `Microsoft.Agents.AI.Harness` pinned to `1.13.0-preview.260703.1`.
- Uses `AzureOpenAIClient` with `AzureCliCredential`.
- Uses `GetChatClient(config.ModelDeployment).AsIChatClient()` as the live model client.
- Uses `chatClient.AsHarnessAgent(new HarnessAgentOptions { ... })`.
- Uses `AIFunctionFactory.Create(...)` for `assess_harness_readiness` and `capture_harness_evidence`.
- Uses `HarnessAgentOptions.ChatOptions.Tools` for tool exposure.
- Uses `HarnessAgentOptions.ToolApprovalAgentOptions` for tool approval.
- Uses `HarnessAgentOptions.LoopEvaluators` with `CompletionMarkerLoopEvaluator("DONE")`.
- Uses `HarnessAgentOptions.MaxContextWindowTokens`, `MaxOutputTokens`, and `DisableCompaction = false`.
- Keeps todo, mode, file memory, file access, and OpenTelemetry enabled through harness options.
- Writes a repeatable evaluation artifact to `output/day03-lab05-harness-evidence.json` in the harness file store.
- Does not use scripted `IChatClient`, manual `ChatClientAgent`, or a hand-built harness substitute.

See `lab05-harness-engineering.md` for trainer-facing walkthrough notes.

## Lab 06 - Hybrid RAG For Agentic Workflow

Goal: Use retrieval inside an agentic workflow and make grounding, verification, retry, and provenance visible.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Retrieval as a tool | Agent asks for evidence before answering | `AIFunctionFactory.Create(...)` retrieval function | Query Azure AI Search through a tool boundary |
| Live retrieval | Grounding evidence comes from the training search index | `SearchClient.SearchAsync<SearchDocument>(...)` | Use the Day 1 student index, not local-only documents |
| Answer agent | Foundry model generates the grounded response | `AIProjectClient`, `projectClient.AsAIAgent(...)` | The agent must call `retrieve_training_context` and cite returned document IDs |
| Workflow control | Retrieval planning, answer, verification, retry, and final output are explicit | `WorkflowBuilder`, typed `Executor<TInput,TOutput>` | `PlanRetrieval` -> `AnswerWithFoundryAgent` -> `VerifyGrounding` -> retry/finalize |
| State and provenance | Track what was retrieved and used | `IWorkflowContext.QueueStateUpdateAsync(...)`, `AgentSession.StateBag` | Save request, plan, answer, document count, verification, retry plan |
| Guarded response | Avoid unsupported answers | Verification executor plus bounded retry | If evidence/citations are weak, retry once and then return `not_grounded` |
| Hybrid boundary | Make retrieval/runtime provider choices explicit | Retrieval mode and provider config | Required path is Azure AI Search + Foundry; vector/local/neocloud paths are extensions, not simulated |

Implemented project: `Lab06HybridRagWorkflow`

Recommended enterprise scenario: ProNative policy/support assistant grounded on training operations documents.

Implemented decision:

- Uses `Microsoft.Agents.AI.Workflows` pinned to `1.13.0`.
- Uses `WorkflowBuilder`, typed executors, switch routing, workflow state, streaming execution, and typed workflow output.
- Uses `Microsoft.Agents.AI.Foundry` with `AIProjectClient.AsAIAgent(...)`.
- Uses `AIFunctionFactory.Create(...)` to expose `retrieve_training_context` as the retrieval tool.
- Uses `Azure.Search.Documents` with `SearchClient.SearchAsync<SearchDocument>(...)`.
- Defaults to `https://srchan2607101.search.windows.net` and student index pattern `idx-st26061000-rag`.
- Uses `AzureCliCredential` / RBAC by default, with `AZURE_SEARCH_ADMIN_KEY` as trainer fallback.
- Does not make NVIDIA DGX Spark, Runpod, Foundry Local, or vector indexes mandatory for Day 3.
- Does not simulate retrieval or return fake grounded answers.

## Lab 07 - Workflow Agent

Goal: Use Microsoft Agent Framework workflow-agent orchestration in C# for a structured business process.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Agents inside workflows | Agents become workflow participants | `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)` | Use specialized Foundry-backed `AIAgent` instances in sequence |
| Sequential coordination | Output from one agent feeds another | `SequentialWorkflowBuilder.WithChainOnlyAgentResponses(true)` | Intent Analyst -> Planner -> Risk Reviewer -> Finalizer |
| Intermediate output | Students can inspect each specialist contribution | `WithIntermediateOutputFrom(...)` | Emit analyst, planner, and reviewer output as intermediate workflow outputs |
| Terminal output | Finalizer owns the workflow result | `WithOutputFrom(...)` | Finalizer response is the terminal workflow output |
| Streaming visibility | Watch each agent produce output | `InProcessExecution.RunStreamingAsync(...)`, `AgentResponseUpdateEvent`, `AgentResponseEvent`, `WorkflowOutputEvent` | Print executor ID, streaming content, and workflow outputs |
| Structured output | Return a typed business result, not only free text | `ChatClientStructuredOutputExtensions.GetResponseAsync<T>(...)`, `ChatResponse<T>.TryGetResult(...)` | Normalize the final workflow transcript into `WorkflowAgentStructuredResult` |
| Workflow-as-agent boundary | Workflow can become a reusable unit conceptually | Unconfirmed in C# package surface | Do not build against `Workflow.as_agent()` until verified |
| Cleanup | Do not leave temporary server-side agent versions behind | Local `AsAIAgent(...)` response-agent path | No server-side agent version cleanup is required for this lab path |

Implemented project: `Lab07WorkflowAgent`

Recommended enterprise scenario: AI-native training change request workflow.

Workflow:

1. `IntentAnalystAgent`: interprets the request.
2. `PlanAgent`: drafts an execution plan.
3. `RiskReviewerAgent`: reviews risk and missing information.
4. `FinalizerAgent`: produces final action summary.

Implemented decision:

- Uses `Microsoft.Agents.AI.Workflows` pinned to `1.13.0`.
- Uses `Microsoft.Agents.AI.Foundry` with `AIProjectClient.AsAIAgent(...)`.
- Uses `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)` as the primary implementation.
- Uses `WithChainOnlyAgentResponses(true)` for clean sequential handoff.
- Uses `WithIntermediateOutputFrom(...)` and `WithOutputFrom(...)` for workflow visibility and terminal output designation.
- Uses `InProcessExecution.RunStreamingAsync(...)` and workflow event types for streaming visibility.
- Uses official structured output through `GetResponseAsync<WorkflowAgentStructuredResult>(...)`.
- Does not use direct sequential `agent.RunAsync(...)` calls as the main orchestration.
- Does not build against `Workflow.as_agent()`.
- This lab becomes the bridge into Day 4 multi-agent architecture, concurrent coordination, handoff, group chat, and Magentic-style orchestration.

## Day 3 Capability Progression

| Lab | Primary Learning Move | Main Agent Framework Components |
|---|---|---|
| Lab 01 | Agentic loop and runtime controls | `AsAIAgent`, agent-run middleware, function middleware, `IChatClient` middleware, tools, approval, `LoopAgent`, `StateBag` |
| Lab 02 | Deterministic flow engineering | `WorkflowBuilder`, typed executors, switch routing, `RequestPort`, events, workflow state |
| Lab 03 | Agent Skills source types and provider boundary | `AgentSkillsProviderBuilder`, `UseFileSkill`, `AgentInlineSkill`, `AgentClassSkill<T>`, `UseMcpSkills`, resources, scripts, approval boundary |
| Lab 04 | Conversations, state, and memory | `AgentSession.StateBag`, `AIContextProvider`, `ProviderSessionState<T>`, `InMemoryChatHistoryProvider`, `MessageCountingChatReducer`, session serialization, compaction |
| Lab 05 | Harness and evaluation | `Microsoft.Agents.AI.Harness`, `AsHarnessAgent`, `HarnessAgentOptions`, live Azure OpenAI `IChatClient`, tool approval, loop evaluators, compaction, file memory/access, todo/mode providers, OpenTelemetry, evidence capture |
| Lab 06 | Hybrid RAG in workflow | `WorkflowBuilder`, typed executors, `AIProjectClient.AsAIAgent`, `AIFunctionFactory.Create`, `SearchClient.SearchAsync<SearchDocument>`, workflow state, bounded retry, typed output |
| Lab 07 | Workflow agent | `AgentWorkflowBuilder`, `SequentialWorkflowBuilder`, Foundry-backed `AIAgent`, intermediate/terminal workflow outputs, streaming workflow events, structured output |

## Open Review Decisions Before Remaining Code Generation

No remaining Day 3 lab generation decisions are open.

## References

- Agent Framework workflows: https://learn.microsoft.com/en-us/agent-framework/workflows/
- Workflow Builder and execution: https://learn.microsoft.com/en-us/agent-framework/workflows/workflows
- Workflow edges: https://learn.microsoft.com/en-us/agent-framework/workflows/edges
- Workflow state: https://learn.microsoft.com/en-us/agent-framework/workflows/state
- Agents in workflows: https://learn.microsoft.com/en-us/agent-framework/workflows/agents-in-workflows
- Agent middleware: https://learn.microsoft.com/en-us/agent-framework/agents/middleware/?pivots=programming-language-csharp
- Agent harnesses: https://learn.microsoft.com/en-us/agent-framework/agents/harness?pivots=programming-language-csharp
- Agent conversations overview: https://learn.microsoft.com/en-us/agent-framework/agents/conversations/?pivots=programming-language-csharp
- Agent sessions: https://learn.microsoft.com/en-us/agent-framework/agents/conversations/session?pivots=programming-language-csharp
- Context providers: https://learn.microsoft.com/en-us/agent-framework/agents/conversations/context-providers?pivots=programming-language-csharp
- Conversation storage: https://learn.microsoft.com/en-us/agent-framework/agents/conversations/storage?pivots=programming-language-csharp
- Context compaction: https://learn.microsoft.com/en-us/agent-framework/agents/conversations/compaction?pivots=programming-language-csharp
