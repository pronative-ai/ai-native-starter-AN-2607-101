# Day 4 Labs: Microsoft Agent Framework, Protocols, and AgentGateway Component Mapping

This mapping keeps Day 4 coherent before and during code generation.

Day 4 must not use dummy agent abstractions as the primary implementation. Where Microsoft Agent Framework owns the agent/workflow runtime, labs must use the official package surface directly. Protocol and gateway topics can use their own protocol-specific or gateway-specific components.

## Verified Package Boundary

Verified locally for the current starter pack:

- `Microsoft.Agents.AI` 1.13.0
- `Microsoft.Agents.AI.Foundry` 1.12.0-preview.260629.1
- `Microsoft.Agents.AI.Workflows` 1.13.0
- `Azure.AI.Projects` 2.1.0-beta.3
- `Azure.Identity` 1.21.0
- `Microsoft.Extensions.AI` 10.6.0
- `AGUI.Abstractions` 0.0.3
- `ModelContextProtocol` 1.4.0

The installed C# package does not expose literal classes named `SequentialOrchestration`, `ConcurrentOrchestration`, `HandoffOrchestration`, `GroupChatOrchestration`, or `MagenticOrchestration`. The verified C# implementation surface is the native Agent Framework builder API:

- `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)`
- `AgentWorkflowBuilder.CreateConcurrentBuilderWith(...)`
- `AgentWorkflowBuilder.CreateHandoffBuilderWith(...)`
- `AgentWorkflowBuilder.CreateGroupChatBuilderWith(...)`
- `AgentWorkflowBuilder.CreateMagenticBuilderWith(...)`
- `RoundRobinGroupChatManager`
- `InProcessExecution.RunStreamingAsync(...)`

These builder APIs are the official native implementation target for Day 4 Labs 01 and 02. Do not replace them with hand-built workflow edges or custom agent loops unless an official API is unavailable and a fallback is explicitly approved.

## Day 4 Capability Progression

| Lab | Primary Learning Move | Main Components | Status |
|---|---|---|---|
| Lab 01 | Compare multi-agent coordination topologies | `CreateSequentialBuilderWith`, `CreateConcurrentBuilderWith`, `CreateHandoffBuilderWith`, `CreateGroupChatBuilderWith`, `RoundRobinGroupChatManager` | Generated and build-verified |
| Lab 02 | Build Magentic-style coordinator-worker orchestration | `CreateMagenticBuilderWith` with manager/coordinator and worker agents | Generated and build-verified |
| Lab 03 | Expose an agent interoperability boundary | `Microsoft.Agents.AI.Hosting.A2A.AspNetCore`, `builder.AddAIAgent(...)`, `app.MapA2AHttpJson(...)` | Generated and build-verified |
| Lab 04 | Compare AG-UI and A2UI interaction boundaries | `AGUI.Abstractions`, A2UI declarative protocol payloads, interrupt/resume approval bridge | Generated and build-verified |
| Lab 05 | Compare UTCP and MCP tool boundaries | `ModelContextProtocol`, UTCP manual and native HTTP request construction | Generated and build-verified |
| Lab 06 | Route agent/model/tool calls through gateway | AgentGateway listener, route, backend, policy, managed identity backend auth, tracing | Generated and build-verified |
| Lab 07 | Control and observe gateway traffic | AgentGateway rate limit, request/cost headers, logs, traces, Azure Monitor/App Insights KQL | Generated and build-verified |

## Lab 01 - Multi-Agent Architecture

Goal: Compare sequential, handoff, concurrent, and group-style coordination using native Agent Framework workflows.

Recommended project: `src/Lab01MultiAgentArchitecture`

Recommended enterprise scenario: ProNative AI training change request review.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Sequential coordination | Agent output flows to the next agent | `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)` | Intake analyst -> delivery planner -> risk reviewer -> final decision |
| Concurrent coordination | Multiple specialists work on independent branches | `AgentWorkflowBuilder.CreateConcurrentBuilderWith(...)` | Learner value, Azure cost, and governance specialists evaluate the same request |
| Handoff | One agent chooses the next specialist | `AgentWorkflowBuilder.CreateHandoffBuilderWith(...)`, `WithHandoff(...)` | Triage router hands off to delivery, cost, or governance specialist |
| Group-style coordination | Agents collaborate in a shared conversation | `AgentWorkflowBuilder.CreateGroupChatBuilderWith(...)`, `RoundRobinGroupChatManager` | Specialists participate in a bounded round-robin group discussion |
| Observability | Show students how messages move | `InProcessExecution.RunStreamingAsync(...)`, workflow events | Print workflow started, agent response, workflow output, and failure events |

Component contract:

- Official capability: Multi-agent orchestration using Agent Framework workflow builders.
- Package: `Microsoft.Agents.AI.Workflows`.
- Required classes/methods: `AIProjectClient.AsAIAgent(...)`, `AgentWorkflowBuilder.CreateSequentialBuilderWith(...)`, `AgentWorkflowBuilder.CreateConcurrentBuilderWith(...)`, `AgentWorkflowBuilder.CreateHandoffBuilderWith(...)`, `AgentWorkflowBuilder.CreateGroupChatBuilderWith(...)`, `RoundRobinGroupChatManager`, `InProcessExecution.RunStreamingAsync(...)`.
- Required code evidence: package references in `Lab01MultiAgentArchitecture.csproj`; four separate workflow builds in `Program.cs`; workflow events printed through `WatchStreamAsync()`.
- Forbidden substitutes: no `ITrainingAgent`, no `RoleAgent`, no custom multi-agent loop, no hand-built workflow graph for the four coordination types.
- Build acceptance: `dotnet build src/Lab01MultiAgentArchitecture/Lab01MultiAgentArchitecture.csproj`.

## Lab 02 - Magentic-Style Orchestration

Goal: Show planner/coordinator-worker pattern inspired by Magentic-One.

Recommended project: `src/Lab02CoordinatorWorkerAgents`

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Magentic orchestration | Manager decomposes and assigns work | `AgentWorkflowBuilder.CreateMagenticBuilderWith(...)` | Native Magentic workflow owns planning and worker coordination |
| Manager/coordinator | Owns task plan, progress, and completion | Foundry-backed `AIAgent` used as manager agent | Creates and updates task plan |
| Worker agents | Specialized agents execute subtasks | Foundry-backed worker `AIAgent` participants | Research, implementation, validation, summarization |
| Re-planning | Recover when output is incomplete | Magentic workflow manager behavior | Manager requests missing evidence or redirects |
| Completion decision | Stop only when output meets acceptance checks | Magentic workflow completion behavior plus evaluator if supported | Final result includes acceptance status |

Component contract:

- Official capability: Magentic-style orchestration.
- Package: `Microsoft.Agents.AI.Workflows`.
- Required classes/methods: `AIProjectClient.AsAIAgent(...)`, `AgentWorkflowBuilder.CreateMagenticBuilderWith(...)`, `MagenticWorkflowBuilder.AddParticipants(...)`, `WithMaxRounds(...)`, `WithMaxResets(...)`, `WithMaxStalls(...)`, `RequirePlanSignoff(...)`, `Build()`, `InProcessExecution.RunStreamingAsync(...)`, `MagenticPlanCreatedEvent`, `MagenticReplannedEvent`, `MagenticProgressLedgerUpdatedEvent`.
- Required code evidence: manager agent passed to `CreateMagenticBuilderWith(...)`; workers registered through `AddParticipants(...)`; Magentic plan/progress events printed from `WatchStreamAsync()`.
- Forbidden substitutes: no custom coordinator loop as the main path.
- Build acceptance: the lab must compile against `Microsoft.Agents.AI.Workflows` before it is considered complete.

## Lab 03 - A2A Concept and Agent Card

Goal: Expose or simulate an agent interoperability boundary.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Agent discovery | Other agents know this agent exists | Agent Card via `app.MapA2AHttpJson(...)` | `GET /a2a/training-ops/v1/card` |
| Capability declaration | Describe what the agent can do | `agentCard: new() { ... }` | Declare ProNative training operations capability |
| Message/task boundary | Remote agent interaction is a task/message contract | A2A message stream endpoint | `POST /a2a/training-ops/v1/message:stream` |
| Security declaration | Enterprise agents need auth metadata | Agent Card security schemes | Include Entra/JWT placeholder for Azure path |
| Framework independence | A2A does not expose internals | Agent Framework agent behind `MapA2AHttpJson(...)` facade | A2A maps external protocol messages to the hosted Agent Framework agent |

Component contract:

- Official capability: Agent Framework A2A ASP.NET Core hosting.
- Package: `Microsoft.Agents.AI.Hosting.A2A.AspNetCore` 1.13.0-preview.260703.1.
- Required classes/methods: `AIProjectClient.GetProjectOpenAIClient()`, `GetProjectResponsesClient()`, `AsIChatClient(...)`, `builder.AddAIAgent(...)`, `app.MapA2AHttpJson(...)`.
- Required code evidence: `MapA2AHttpJson(...)` route at `/a2a/training-ops`; HTTP file with Agent Card and message-stream requests.
- Forbidden substitutes: no static card-only printer and no custom A2A-like endpoint as the main lab.

## Lab 04 - AG-UI / A2UI Agent-User Interaction Boundary

Goal: Understand agent-to-user interaction patterns without heavy implementation.

Recommended project: `src/Lab04AgentUserInteractionBoundary`

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Agent-user event stream | Agent runtime sends events to frontend | `AGUI.Abstractions` event model | Text, tool, state, interrupt, resume, and completion events |
| Generative UI payload | Agent proposes UI safely | A2UI declarative component payload | `createSurface`, `updateComponents`, `updateDataModel`, and `action` JSON |
| Human-in-the-loop | User can approve/edit/interrupt | `RunFinishedInterruptOutcome`, `AGUIInterrupt`, `AGUIResume` | Frontend approval maps to backend tool approval response |
| UI trust boundary | UI renderer controls what can render | A2UI catalog/components | Only approved component types render; no arbitrary code is generated |
| Backend runtime | Agent still owns tool execution | `AGUITool`, `AGUIToolCallInfo`, `AGUIToolApprovalResumePayload` | Agent emits UI intent, not executable UI code |

Component contract:

- Official capability: AG-UI event stream and interrupt/resume flow.
- Package: `AGUI.Abstractions` 0.0.3.
- Required classes/methods: `RunAgentInput`, `AGUIUserMessage`, `AGUITool`, `RunStartedEvent`, `TextMessageStartEvent`, `TextMessageContentEvent`, `TextMessageEndEvent`, `ToolCallStartEvent`, `ToolCallArgsEvent`, `ToolCallEndEvent`, `StateSnapshotEvent`, `RunFinishedEvent`, `RunFinishedInterruptOutcome`, `AGUIInterrupt`, `AGUIResume`, `AGUIToolApprovalResumePayload`, `ToolCallResultEvent`, `RunFinishedSuccessOutcome`, `AGUIJsonSerializerContext`.
- Required code evidence: `PackageReference Include="AGUI.Abstractions" Version="0.0.3"`; AG-UI source-generated serialization through `AGUIJsonSerializerContext`; A2UI `createSurface`, `updateComponents`, `updateDataModel`, and `action` payloads.
- Forbidden substitutes: no custom AG-UI event records as the primary path; no homemade interrupt/resume model; no executable UI generation.
- Build acceptance: `dotnet build src/Lab04AgentUserInteractionBoundary/Lab04AgentUserInteractionBoundary.csproj`.

## Lab 05 - UTCP vs MCP

Goal: Understand when tool calling is direct API style versus tool-server style.

Recommended project: `src/Lab05ProtocolToolBoundary`

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| MCP boundary | Agent connects to a tool server that exposes tools/resources | `ModelContextProtocol` C# SDK | Generate MCP protocol tool metadata from a C# method |
| UTCP boundary | Agent calls existing APIs directly with tool metadata | UTCP manual/provider definition | Generate a native HTTP request from a UTCP manual |
| Tool discovery | Agent/runtime sees available tools | `McpServerTool.ProtocolTool` vs UTCP `tools[]` manual | Compare discovery mechanism |
| Tool execution | Invoke the chosen tool | MCP `tools/call` JSON-RPC vs direct HTTP API | Same training operation, two invocation shapes |
| Governance | Apply policy before tool use | Agent Framework function middleware plus gateway policy | Validate tool name, route, identity, cost tag |

Component contract:

- Official capability: MCP server tool boundary and UTCP direct native endpoint boundary.
- MCP package: `ModelContextProtocol` 1.4.0.
- MCP required classes/methods: `[McpServerToolType]`, `[McpServerTool]`, `McpServerTool.Create(...)`, `McpServerToolCreateOptions`, `McpServerTool.ProtocolTool`, `Tool.Name`, `Tool.Description`, `Tool.InputSchema`.
- MCP deployment APIs shown for real server hosting: `AddMcpServer()`, `WithStdioServerTransport()`, `WithToolsFromAssembly()`.
- MCP client APIs referenced for real clients: `McpClient.CreateAsync(...)`, `ListToolsAsync()`, `CallToolAsync(...)`.
- UTCP required protocol evidence: `manual_version`, `utcp_version`, `tools`, `inputs`, `tool_call_template`, `call_template_type`, `url`, `http_method`, `auth`.
- Required code evidence: `PackageReference Include="ModelContextProtocol" Version="1.4.0"`; `McpServerTool.Create(...)`; `[McpServerToolType]`; `[McpServerTool]`; UTCP manual-generated `HttpRequestMessage`.
- Forbidden substitutes: no custom MCP-like protocol; no invented UTCP C# SDK; no UTCP wrapper server.
- Build acceptance: `dotnet build src/Lab05ProtocolToolBoundary/Lab05ProtocolToolBoundary.csproj`.

## Lab 06 - AgentGateway Baseline

Goal: Route agent/model/tool calls through gateway with logs and policy.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Gateway deployment | Put a control point between apps and agent/model/tool backends | AgentGateway in Azure Container Apps | Containerized gateway in shared resource group |
| Listener | Public/private entry point for traffic | AgentGateway listener | HTTP listener for model/tool routes |
| Route | Match traffic and send to backend | AgentGateway routes | `/openai/*`, `/mcp/*`, `/a2a/*` or training-specific paths |
| Backend | Target service | Azure OpenAI/Foundry endpoint, MCP server, A2A service | Backends configured with auth |
| Policy | Enforce enterprise controls | AgentGateway policy | Auth, route restrictions, headers, cost tags |
| Identity | Avoid raw shared secrets in apps | Managed identity / secret reference | Trainer config first, managed identity path for deployed apps |
| Azure integration | Run gateway in Microsoft cloud stack | Azure Container Apps, Log Analytics, App Insights | Logs and metrics sent to Azure Monitor |

Component contract:

- Official capability: AgentGateway standalone gateway baseline for model, MCP tool, and A2A agent traffic.
- Package: none for .NET. The official surface is the AgentGateway standalone container, configuration schema, and HTTP data plane.
- Required classes/methods: not applicable. Required config fields are `config.tracing`, `config.logging`, `binds`, `listeners`, `routes`, `matches.path.pathPrefix`, `backends`, `requestHeaderModifier`, `localRateLimit`, and `backendAuth.azure.explicitConfig.managedIdentity`.
- Required code evidence: `src/Lab06AgentGatewayClient/config/agentgateway-an2607101-lab06.yaml`; C# client prepares OpenAI-compatible `/azure/v1/chat/completions`, MCP JSON-RPC `/mcp`, and A2A `/a2a/training-ops/v1/message:stream` requests.
- Forbidden substitutes: no fake gateway response; no custom gateway abstraction; no direct Foundry bypass as the main lab path.
- Build acceptance: `dotnet build src/Lab06AgentGatewayClient/Lab06AgentGatewayClient.csproj`.

Implementation decision:

- This lab requires AgentGateway deployed in Azure Container Apps.
- Keep route config small and observable.
- Dry-run mode is default. Live gateway calls require `PN_AGENTGATEWAY_LIVE=true` or `--live`.
- Gateway frontend auth can be passed through `PN_AGENTGATEWAY_BEARER_TOKEN` or `PN_AGENTGATEWAY_API_KEY` if the deployed route requires it.

## Lab 07 - Gateway Observability and Control

Goal: Show rate limit, cost/token attribution, and trace correlation.

| Capability | Training Meaning | Primary Component | Implementation Alignment |
|---|---|---|---|
| Rate limiting | Protect shared model capacity | AgentGateway rate limiting policy | Limit per student/lab route |
| Token/cost attribution | Track consumption by batch/student | Gateway headers/tags plus model usage logs | `BatchId`, `StudentId`, `Route`, `ModelDeployment` |
| Trace correlation | Connect app request to gateway and agent run | Correlation ID plus App Insights | Generate `x-correlation-id` in client |
| Policy decision visibility | Trainer sees allowed/blocked traffic | AgentGateway logs | Show allow, throttle, reject examples |
| Azure observability | Use default/standard monitoring first | Azure Monitor, Log Analytics, Application Insights | No ProNative custom workbook yet unless live project days |
| Failure handling | Make gateway control visible | Retry/timeout/rate-limit response handling | Client prints response status and action |

Component contract:

- Official capability: AgentGateway traffic control and observability.
- Package: none for .NET. The official surfaces are AgentGateway standalone configuration, HTTP traffic, Container Apps logs, and Azure Monitor/App Insights KQL.
- Required classes/methods: not applicable. Required config/query evidence: `localRateLimit`, `requestHeaderModifier`, `backendAuth.azure.explicitConfig.managedIdentity`, `config.tracing`, `config.logging`, `ContainerAppHTTPLogs`, `ContainerAppConsoleLogs_CL`.
- Required code evidence: `x-request-id`, `x-correlation-id`, `traceparent`, `x-batch-id`, `x-student-id`, `x-lab-id`, `x-cost-center`, `x-route-purpose`, live status handling, and saved run evidence.
- Forbidden substitutes: no fake gateway response; no custom rate limiter; no simulated App Insights data.
- Build acceptance: `dotnet build src/Lab07GatewayObservabilityControl/Lab07GatewayObservabilityControl.csproj`.

Implementation decision:

- Use standard Azure Monitor/App Insights visibility during training.
- Custom ProNative workbooks remain for Day 6-8 live project delivery.
- Dry-run mode is default. Live gateway calls require `PN_AGENTGATEWAY_LIVE=true` or `--live`.
- Lab 07 includes a stricter temporary route configuration to make `localRateLimit` behavior visible in a short classroom run.

## Day 4 Open Review Decisions Before Further Code Generation

1. Confirm Lab 04 remains a compact concept/code lab and not a full frontend build.
2. Confirm Lab 05 remains architecture-choice focused, because Day 2 already has hands-on MCP.
3. Confirm the stricter Lab 07 AgentGateway route is deployed before expecting HTTP 429 during the burst.
4. Confirm logs for Labs 06-07 use standard Azure Monitor/App Insights, not custom ProNative workbooks.

## References

- Agent Framework workflows: https://learn.microsoft.com/en-us/agent-framework/workflows/
- Workflow Builder and execution: https://learn.microsoft.com/en-us/agent-framework/workflows/workflows
- Workflow edges: https://learn.microsoft.com/en-us/agent-framework/workflows/edges
- Agents in workflows: https://learn.microsoft.com/en-us/agent-framework/workflows/agents-in-workflows
- Agent middleware: https://learn.microsoft.com/en-us/agent-framework/agents/middleware/?pivots=programming-language-csharp
- A2A Protocol: https://a2a-protocol.org/latest/
- A2A specification: https://a2a-protocol.org/latest/specification/
- MCP introduction: https://modelcontextprotocol.io/docs/getting-started/intro
- UTCP organization: https://github.com/universal-tool-calling-protocol
- AG-UI overview: https://docs.ag-ui.com/introduction
- A2UI overview: https://a2ui.org/
- AgentGateway standalone docs: https://agentgateway.dev/docs/standalone/latest/
