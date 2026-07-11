# Lab 02, Lab 06, and Lab 07 — Comparison

## Shared Foundation: WorkflowBuilder (Programmatic Orchestration)

**Both Lab 02 and Lab 06 use `Microsoft.Agents.AI.Workflows`:**

- `WorkflowBuilder` to define the pipeline graph
- Typed `Executor<TInput, TOutput>` classes for each processing step
- Explicit `AddEdge` / `AddSwitch` edges for routing
- `InProcessExecution.RunStreamingAsync(...)` for execution
- `WorkflowOutputEvent` for the typed terminal result

This pattern is a **code-defined orchestration** — the pipeline, routing decisions, retry logic, and terminal conditions are all written in C#. The flow is deterministic at the orchestration level regardless of what each executor does internally.

## Lab 07: AgentWorkflowBuilder (Agent-Driven Orchestration)

**Lab 07 uses a fundamentally different pattern:**

- `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)` to define the pipeline
- `AIAgent` instances for each specialist role (Intake, IntentAnalysis, ExecutionPlanning, DeliveryRisk)
- `WithChainOnlyAgentResponses(true)` so each downstream agent receives only the previous agent's output
- Agents independently reason using an LLM and pass structured output to the next agent
- The orchestration is agent-driven rather than code-driven

---

## Three-Way Comparison

| Aspect | Lab 02 — FlowEngineering | Lab 06 — Hybrid RAG Workflow | Lab 07 — Workflow Agent |
|--------|--------------------------|------------------------------|-------------------------|
| **Orchestration framework** | `WorkflowBuilder` | `WorkflowBuilder` | `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)` |
| **Node type** | `Executor<TInput,TOutput>` (C# class) | `Executor<TInput,TOutput>` (C# class, one calls an LLM) | `AIAgent` (LLM-powered agent) |
| **Routing** | Hardcoded `AddSwitch` conditions | Hardcoded `AddSwitch` conditions | Agents decide via LLM reasoning (sequential handoff) |
| **LLM / Agent inside pipeline** | None — every executor is pure C# logic | One executor (`FoundryGroundedAnswerExecutor`) calls `AIProjectClient.AsAIAgent(...)` with an LLM + retrieval tool | Every node is a Foundry-backed `AIAgent` that independently reasons |
| **Human-in-the-loop** | Yes — `RequestPort` + console `ReadApprovalFromConsole()` | No | No |
| **External dependencies** | None (runs fully local) | Azure AI Search, Foundry project endpoint | Foundry project endpoint |
| **Retry strategy** | Bounded: deliberate first-attempt failure for operational requests, retry once | Bounded: retry once with expanded query when grounding verification fails | N/A — sequential agent chain, no retry loop |
| **Routing decisions** | Keyword-based classification (`provision`/`deploy` → Operational) | Deterministic: retry on `NeedsRetry && Attempt < 2`, else finalize | Agent decides what to pass to the next agent |
| **State** | `QueueStateUpdateAsync` / `ReadStateAsync` in `Day03Lab02Flow` scope | Same pattern in `Day03Lab06HybridRag` scope | Not used — agents pass structured output via chain |
| **Terminal output** | `FinalFlowResult` | `FinalHybridRagResult` | `GetResponseAsync<WorkflowAgentStructuredResult>` via `ChatClientStructuredOutputExtensions` |

---

## Key Takeaway

All three labs use a **workflow pattern**, but they fall on a spectrum:

| Lab | Orchestration type | How LLM is used |
|-----|-------------------|-----------------|
| **Lab 02** | Code-defined (`WorkflowBuilder`) | No LLM anywhere — pure C# logic |
| **Lab 06** | Code-defined (`WorkflowBuilder`) | One executor step internally calls an LLM as a tool (answer generation) — but routing is still code |
| **Lab 07** | Agent-driven (`AgentWorkflowBuilder`) | Every node is a self-reasoning LLM agent that controls what to pass forward |

- **Lab 02** is a pure business-process workflow: deterministic classification, HITL approval, and execution.
- **Lab 06** applies the same `WorkflowBuilder` pattern to a RAG scenario. The routing is still hardcoded, but one step uses an LLM-powered Foundry agent.
- **Lab 07** shifts to agent-driven orchestration. The framework (`AgentWorkflowBuilder`) manages agent handoff, and each agent independently reasons using an LLM.
