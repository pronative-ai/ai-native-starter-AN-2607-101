# Lab 01 Component Alignment

## Lab Purpose

Lab 01 introduces the agentic loop using real Microsoft Agent Framework runtime components:

`goal -> plan -> action -> observation -> reflection -> checkpoint`

The lab is intentionally Agent Framework-first. Semantic Kernel plugins, formal WorkflowBuilder graphs, and Dapr actor checkpoints are reserved for the labs where they become the primary learning objective.

## Implemented Alignment

| Capability | Training Meaning | Implemented In Lab 01 | Component Used |
|---|---|---|---|
| Goal / planning | Convert a user goal into milestone steps | `build_milestone_plan` tool returns a typed milestone graph | `AIFunctionFactory.Create(...)` |
| Action | Let the model choose and invoke a real tool | Agent calls readiness and checkpoint tools through the function-calling loop | `AIFunctionFactory.Create(...)` |
| Observation | Model grounds the response in tool output | Agent must summarize the returned readiness payload | Foundry-backed `AsAIAgent(...)` |
| Reflection | Check whether the response satisfies the required loop structure | `LoopAgent` re-invokes once when required sections are missing | `LoopAgent` + `DelegateLoopEvaluator` |
| Lifecycle | Keep run-scoped state in a serializable session boundary | Batch/student/checkpoint metadata is written to `AgentSession.StateBag` | `AgentSession.StateBag` |
| Trust | Put approval and policy between agent intent and tool execution | Tools are auto-approved by explicit rule; destructive names are blocked | `UseToolApproval(...)` + function middleware |
| Cross-cutting controls | Logging, validation, timing, future trace correlation | Agent run middleware logs and stores duration in session state | `AIAgent.AsBuilder().Use(...)` |
| Middleware scope | Teach all middleware layers, not only one | Lab 01 covers run middleware, function-calling middleware, and `IChatClient` middleware through the Foundry `clientFactory` hook | Agent Run middleware, Function Calling middleware, `IChatClient` middleware |

## Alignment Options For Review

| Option | What It Means | Recommendation |
|---|---|---|
| Keep Lab 01 Agent Framework-first | Use Foundry agent, middleware, function tools, approval, loop, and session state | Recommended and implemented |
| Restore full middleware scope in implementation | Add `IChatClient` middleware without replacing Foundry-backed `AsAIAgent(...)` | Implemented in Lab 01 through `clientFactory` |
| Add formal WorkflowBuilder in Lab 01 | Bring in `Microsoft.Agents.AI.Workflows` and model the plan as executors and edges | Better for Lab 02, because Day 3 Lab 02 is Flow Engineering |
| Add Semantic Kernel `KernelPlugin` in Lab 01 | Bring in Semantic Kernel and express actions as plugins | Better for Lab 03, because Day 3 Lab 03 is Skill-driven Development |
| Add Dapr actor checkpoints in Lab 01 | Add hosted app + Dapr actor state | Better for Lab 04 or deployment notes, because Lab 04 is State and Memory |
| Add AutoGen / MagenticOne now | Use multi-agent orchestration concepts in the first lab | Better for Day 4, where multi-agent and Magentic-style orchestration are the focus |

## References

- Agent Framework middleware: https://learn.microsoft.com/en-us/agent-framework/agents/middleware/?pivots=programming-language-csharp
- Agent Framework workflows: https://learn.microsoft.com/en-us/agent-framework/workflows/
- Workflow Builder and execution: https://learn.microsoft.com/en-us/agent-framework/workflows/workflows
- Semantic Kernel plugins: https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/
- Azure Container Apps Dapr integration: https://learn.microsoft.com/en-us/azure/container-apps/dapr-overview
