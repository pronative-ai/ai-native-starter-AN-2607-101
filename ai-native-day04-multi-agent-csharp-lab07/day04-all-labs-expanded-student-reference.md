# Day 04 Expanded Student Reference - Multi-Agent, Protocol, UI, Gateway, And Observability Labs

Program: ProNative AI-Native Fullstack Engineering  
Day: 04  
Theme: Moving from one controlled agentic application to distributed agents, protocols, user interaction boundaries, tool boundaries, gateways, and operations

## How To Use This Reference

Use this file to understand the purpose and flow of each Day 04 lab.

Each lab is explained using:

1. Concepts and sub-topics
2. Use cases by sub-topic
3. Lab objectives and coverage
4. Flow of the lab from starting point to classes/functions/methods and what is achieved

The exact class names in your starter repository may differ slightly, but the patterns are the same.

---

# Lab 01 - Multi-Agent Architecture

## 1. Concept

Lab 01 introduces the idea that agentic systems do not always stay as one agent.

Day 03 focused on making one agent controlled, stateful, skill-aware, and grounded. Day 04 starts with this question:

```text
How do multiple agents collaborate?
```

Common multi-agent patterns include:

```text
sequential
concurrent
handoff
group chat
```

The important idea:

```text
Multi-agent architecture is about dividing responsibility across agents and controlling how they coordinate.
```

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Sequential coordination | Agents run one after another | Draft -> review -> finalize |
| Concurrent coordination | Agents work in parallel | Compare multiple strategies, gather multiple analyses |
| Handoff | One agent transfers work to another | Support triage to specialist support agent |
| Group chat | Multiple agents collaborate in shared thread | Brainstorming, review boards, design discussions |
| Coordinator | Agent or workflow that routes work | Assign tasks, combine outputs, enforce process |
| Specialist agent | Agent focused on one responsibility | Planner, researcher, reviewer, validator |
| Shared state | Data passed among agents | Request, evidence, draft, validation result |
| Termination | Deciding when collaboration is complete | Stop after approval, consensus, final answer |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- why multi-agent systems exist
- how responsibility can be split across agents
- how coordination patterns affect reliability
- why shared state and termination rules matter
- how Day 03 workflow concepts extend into Day 04 multi-agent systems

The lab may cover:

```text
- creating multiple specialist agents
- running agents sequentially
- running agents concurrently
- handing work from one agent to another
- combining outputs
- showing final response after coordination
```

## 4. Lab Flow Walkthrough

### Step 1 - Program Starts

Starting point:

```text
Program.cs
```

The app usually:

```text
1. Reads environment variables.
2. Creates model/agent clients.
3. Creates multiple agent definitions.
4. Creates coordination flow.
5. Runs a user request through the selected pattern.
```

What this achieves:

```text
The lab moves from one agent to multiple agents with explicit roles.
```

### Step 2 - Specialist Agents Are Created

Example shape:

```csharp
var plannerAgent = CreateAgent("Planner", "Break the request into steps.");
var reviewerAgent = CreateAgent("Reviewer", "Review output for correctness.");
var summarizerAgent = CreateAgent("Summarizer", "Create final student-friendly response.");
```

What this achieves:

```text
Each agent has a focused responsibility.
```

### Step 3 - Coordination Pattern Runs

Sequential example:

```csharp
var plan = await plannerAgent.RunAsync(userInput);
var review = await reviewerAgent.RunAsync(plan.Text);
var summary = await summarizerAgent.RunAsync(review.Text);
```

Concurrent example:

```csharp
var architectureTask = architectureAgent.RunAsync(userInput);
var riskTask = riskAgent.RunAsync(userInput);

await Task.WhenAll(architectureTask, riskTask);
```

What this achieves:

```text
The application controls how agents collaborate.
```

### Step 4 - Outputs Are Combined

Example:

```csharp
var finalInput = $"""
Plan:
{plan.Text}

Review:
{review.Text}
""";
```

What this achieves:

```text
The final answer is composed from multiple specialist outputs.
```

## 5. Recall Summary

```text
Lab 01 teaches that multi-agent architecture is responsibility design plus coordination design.
```

## 6. References For This Lab

- [Workflow orchestrations in Agent Framework](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/)
- [Agent-to-Agent journey](https://learn.microsoft.com/en-us/agent-framework/journey/agent-to-agent)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| Workflow orchestration | A multi-step agentic system needs explicit orchestration, not only a chain of prompts |
| Agent-to-agent journey | Multiple agents become useful when each one has a defined role and communication boundary |
| State movement | Multi-agent systems need request, evidence, draft, validation result, and final output to move safely |

Practical recall:

```text
Multi-agent design is not "more agents".
It is responsibility design plus coordination design.
```

---

# Lab 02 - Magentic-Style Coordinator-Worker Orchestration

## 1. Concept

Lab 02 focuses on a coordinator-worker pattern.

The coordinator does not do all work directly. It plans, assigns, evaluates, and decides when work is complete.

Workers perform focused tasks.

```text
coordinator
  -> plans work
  -> assigns worker tasks
  -> receives worker results
  -> evaluates progress
  -> asks for more work or finalizes
```

The important idea:

```text
Coordination is a control layer over specialist execution.
```

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Coordinator | Plans and manages work | Project planner, training guide, incident commander |
| Worker | Executes assigned task | Researcher, coder, reviewer, retriever |
| Task assignment | Coordinator gives focused work | "Check API contract", "Summarize logs" |
| Worker result | Output returned to coordinator | Evidence, analysis, recommendation |
| Progress evaluation | Decide if more work is needed | Retry, assign another worker, finalize |
| Stop condition | End orchestration safely | Goal satisfied, max rounds, human intervention |
| Shared scratchpad/state | Stores plan and results | Traceable multi-agent work |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- how coordinator-worker differs from simple sequential agents
- why the coordinator owns planning and termination
- how workers stay focused
- how iterative collaboration can be bounded
- why max rounds and stop conditions matter

The lab may cover:

```text
- coordinator prompt/instructions
- worker agent definitions
- task assignment loop
- collecting worker responses
- deciding whether to continue
- final synthesis
```

## 4. Lab Flow Walkthrough

### Step 1 - Coordinator And Workers Are Created

Example:

```csharp
var coordinator = CreateCoordinatorAgent();
var retrieverWorker = CreateWorkerAgent("Retriever");
var reviewerWorker = CreateWorkerAgent("Reviewer");
```

What this achieves:

```text
The system separates planning from execution.
```

### Step 2 - Coordinator Creates A Plan

Example:

```csharp
var plan = await coordinator.PlanAsync(userGoal);
```

What this achieves:

```text
The coordinator decides which workers are needed.
```

### Step 3 - Worker Tasks Are Assigned

Example:

```csharp
var retrievalResult = await retrieverWorker.RunAsync(plan.RetrievalTask);
var reviewResult = await reviewerWorker.RunAsync(plan.ReviewTask);
```

What this achieves:

```text
Each worker receives a specific job instead of the whole problem.
```

### Step 4 - Coordinator Evaluates Results

Example:

```csharp
var decision = await coordinator.EvaluateAsync(retrievalResult, reviewResult);

if (decision.NeedsMoreWork)
{
    // assign next worker task
}
```

What this achieves:

```text
The system can iterate while still staying bounded.
```

### Step 5 - Final Answer Is Synthesized

Example:

```csharp
var final = await coordinator.FinalizeAsync(allWorkerResults);
```

What this achieves:

```text
The final response reflects coordinated worker evidence.
```

## 5. Recall Summary

```text
Lab 02 teaches that a coordinator-worker system uses a planning agent to manage specialist worker agents.
```

## 6. References For This Lab

- [Magentic orchestration in Agent Framework](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/magentic?pivots=programming-language-csharp)
- [Workflow orchestrations in Agent Framework](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| Magentic orchestration | A coordinator plans, assigns work, monitors progress, and re-plans when needed |
| Worker agents | Specialist agents perform focused tasks instead of each one owning the full problem |
| Bounded collaboration | Multi-agent loops need stop conditions, max rounds, and progress evaluation |
| Final synthesis | Worker results must be combined into one useful response |

Practical recall:

```text
Sequential agents follow a fixed order.
Coordinator-worker orchestration adapts by planning, assigning, evaluating, and re-planning.
```

---

# Lab 03 - A2A Exposure, Discovery, And Consumption

## 1. Concept

Lab 03 introduces Agent-to-Agent communication, often referred to as A2A.

The key idea:

```text
One agent or client can discover another agent's capabilities and send it a task through a defined contract.
```

A2A is about runtime interoperability.

It is different from a catalog.

```text
Catalog:
Where capabilities are registered or described.

Runtime A2A endpoint:
Where messages are actually sent.
```

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Agent card | Describes agent capabilities | Discover what another agent can do |
| Discovery endpoint | Where card is retrieved | Client learns available operations |
| Provider | Agent/service exposing A2A contract | Training ops agent, support agent |
| Consumer | Client/agent calling provider | Another agent delegates task |
| Message payload | Structured request to remote agent | "Summarize lab status" |
| Runtime response | Result returned by provider | Answer, status, task output |
| API Center metadata | Governance/catalog record | Track available agent APIs |
| Gateway route | Optional runtime control path | Rate limit, observe, secure |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- how an agent exposes capabilities
- how another client discovers those capabilities
- how agent cards describe runtime behavior
- how runtime A2A differs from API catalog metadata
- how A2A can connect distributed agentic systems

The lab may cover:

```text
- starting an A2A provider
- exposing an agent card endpoint
- reading the agent card
- sending a task/message to the provider
- displaying the returned response
- discussing API Center/catalog metadata
```

## 4. Lab Flow Walkthrough

### Step 1 - Provider Starts

Example:

```csharp
var app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/a2a/training-ops/v1/card", GetAgentCard);
app.MapPost("/a2a/training-ops/v1/message", HandleAgentMessage);

app.Run();
```

What this achieves:

```text
The agent becomes reachable through runtime endpoints.
```

### Step 2 - Agent Card Is Exposed

Example:

```csharp
static AgentCard GetAgentCard()
{
    return new AgentCard
    {
        Name = "Training Ops Agent",
        Description = "Helps with training lab operations.",
        Capabilities = ["summarize-lab-status", "explain-blocker"]
    };
}
```

What this achieves:

```text
Consumers can discover what the agent can do.
```

### Step 3 - Consumer Reads Agent Card

Example:

```csharp
var card = await httpClient.GetFromJsonAsync<AgentCard>(cardUrl);
```

What this achieves:

```text
The consumer learns the provider's capability contract.
```

### Step 4 - Consumer Sends Message

Example:

```csharp
var response = await httpClient.PostAsJsonAsync(messageUrl, request);
```

What this achieves:

```text
The remote agent performs work through the runtime contract.
```

### Step 5 - Result Is Displayed

Example:

```csharp
Console.WriteLine(result.Text);
```

What this achieves:

```text
You see agent-to-agent interoperability in action.
```

## 5. Recall Summary

```text
Lab 03 teaches that A2A is runtime communication: discover an agent, send it a task, receive a result.
```

## 6. References For This Lab

- [Agent-to-Agent journey](https://learn.microsoft.com/en-us/agent-framework/journey/agent-to-agent)
- [A2A Integration with Agent Framework](https://learn.microsoft.com/en-us/agent-framework/integrations/a2a?tabs=dotnet-cli%2Cuser-secrets&pivots=programming-language-csharp)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| A2A purpose | A2A standardizes communication between agents, even when agents are built with different frameworks or technologies |
| Agent card | The card describes the agent's name, description, version, URL, and capabilities for discovery |
| Message communication | Consumers send structured messages to a runtime endpoint instead of calling local methods |
| Long-running tasks | A2A supports task-oriented agentic processes, not only short request/response calls |
| Conversation identity | A2A uses context/conversation identifiers so repeated calls can continue the same conversation |
| ASP.NET Core hosting | Agent Framework can expose an agent over A2A through ASP.NET Core integration |

Practical recall:

```text
Agent card = discovery.
A2A message endpoint = runtime interaction.
contextId = conversation continuity.
```

---

# Lab 04 - AG-UI / A2UI Agent-User Interaction Boundary

## 1. Concept

Lab 04 focuses on the boundary between agents and user interfaces.

An agent should not only produce free-form text. Sometimes it needs to drive or update a UI.

AG-UI/A2UI patterns help describe structured interaction between:

```text
agent
  -> UI event/update/action
  -> user sees or responds
  -> agent continues
```

The important idea:

```text
Agent-user interaction needs a structured boundary, not only chat text.
```

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| UI event | Structured event from agent to UI | Show progress, ask confirmation |
| UI action | User action returned to agent | Button click, form input, approval |
| Streaming update | Incremental UI changes | Progress bars, live reasoning trace |
| State synchronization | UI and agent share state | Current lab step, selected conversation |
| Human boundary | User decision affects flow | Approve, reject, choose option |
| Structured payload | JSON/event contract | Render cards, commands, status |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- why chat text is not enough for rich agent apps
- how agents can emit structured UI events
- how the user can respond through UI actions
- how UI state and agent state stay connected
- how this supports Teams-style training cockpit experiences

The lab may cover:

```text
- creating event payloads
- sending structured updates
- simulating UI actions
- showing progress/status updates
- handling user action response
```

## 4. Lab Flow Walkthrough

### Step 1 - Agent Starts A UI-Aware Task

Example:

```csharp
var run = await agent.StartRunAsync(userInput);
```

What this achieves:

```text
The agent starts work that may produce UI events.
```

### Step 2 - Agent Emits Event

Example:

```csharp
var evt = new UiEvent
{
    Type = "progress",
    Title = "Checking setup",
    Percent = 40
};
```

What this achieves:

```text
The UI can render progress without parsing free-form text.
```

### Step 3 - UI Renders Structured Payload

Example:

```csharp
RenderProgress(evt.Title, evt.Percent);
```

What this achieves:

```text
User experience becomes predictable and app-like.
```

### Step 4 - User Action Returns To Agent

Example:

```csharp
var userAction = new UiAction
{
    Type = "approve",
    Value = "continue"
};
```

What this achieves:

```text
The user's UI action becomes part of the agent workflow.
```

### Step 5 - Agent Continues

The agent uses the UI action:

```csharp
if (userAction.Type == "approve")
{
    await ContinueAsync();
}
```

What this achieves:

```text
The workflow respects the human interaction boundary.
```

## 5. Recall Summary

```text
Lab 04 teaches that agent-user interaction should use structured events and actions, not only free-form chat.
```

## 6. References For This Lab

- [AG-UI Integration with Agent Framework](https://learn.microsoft.com/en-us/agent-framework/integrations/ag-ui/?pivots=programming-language-csharp)

What to take from this reference:

| Reference area | What it helps you recall |
|---|---|
| AG-UI purpose | AG-UI is for building web or app interfaces that interact with agents through a standardized protocol |
| Real-time streaming | Agent responses and progress can stream to the UI instead of arriving only as one final text block |
| Session management | UI clients can maintain conversation context across multiple requests |
| State synchronization | UI and agent can share state such as selected step, progress, or form data |
| Human-in-the-loop | User actions like approve, reject, choose option, or submit form can become structured inputs |
| Custom UI rendering | Agents can produce structured events that clients render as cards, progress, buttons, or status updates |

Practical recall:

```text
Chat text is content.
AG-UI events are interaction contracts.
```

---

# Lab 05 - MCP Vs UTCP Tool Boundary

## 1. Concept

Lab 05 compares tool boundary patterns.

The core question:

```text
How should an agent access external tools?
```

Two patterns:

```text
MCP:
Tool access through a tool server protocol.

UTCP:
Tool access through direct API/tool contracts.
```

The important idea:

```text
The tool boundary determines how tools are discovered, governed, called, and observed.
```

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Tool server | Server exposes tools | Shared enterprise tool layer |
| Tool discovery | Agent learns available tools | Dynamic capability discovery |
| Direct API tool | Agent calls known endpoint/contract | Internal REST API, typed SDK |
| Tool schema | Defines inputs/outputs | Safer calls, validation |
| Tool governance | Control what can be called | Policy, approval, allowlist |
| Tool execution | Runtime tool invocation | Query ticket, update status |
| Tool observation | Log/trace tool calls | Audit, debugging, cost |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- why tool access needs boundaries
- how MCP-style tool servers differ from direct API tools
- when discovery is useful
- when direct typed APIs are simpler
- how governance applies to both

The lab may cover:

```text
- defining a direct API/tool call
- showing MCP-style tool discovery
- invoking a tool
- comparing request/response shape
- discussing governance and approval
```

## 4. Lab Flow Walkthrough

### Step 1 - Tool Boundary Is Defined

Example direct API:

```csharp
public sealed record ToolRequest(string Operation, string Input);
```

Example MCP-style concept:

```text
Server lists tools.
Client chooses a tool.
Client sends structured input.
Server returns structured output.
```

What this achieves:

```text
The app defines how tools are exposed to the agent.
```

### Step 2 - Tool Is Discovered Or Registered

Direct API:

```csharp
tools.Add("lookup-lab-status", LookupLabStatusAsync);
```

MCP-style:

```csharp
var availableTools = await toolClient.ListToolsAsync();
```

What this achieves:

```text
The agent/runtime knows which tool capabilities exist.
```

### Step 3 - Agent Requests Tool Call

Example:

```csharp
var result = await toolInvoker.InvokeAsync(
    "lookup-lab-status",
    new { LabId = "day04-lab05" });
```

What this achieves:

```text
The agent accesses external capability through a controlled boundary.
```

### Step 4 - Tool Output Is Returned

Example:

```csharp
Console.WriteLine(result.Content);
```

What this achieves:

```text
Tool output becomes observation/evidence for the agent.
```

## 5. Recall Summary

```text
Lab 05 teaches that tool access is a boundary. MCP emphasizes server-based discovery; direct API tools emphasize known typed contracts.
```

## 6. References For This Lab

- [Using MCP tools with Agents](https://learn.microsoft.com/en-us/agent-framework/agents/tools/local-mcp-tools?pivots=programming-language-csharp)
- [Microsoft Learn MCP Server developer reference](https://learn.microsoft.com/en-us/training/support/mcp-developer-reference?source=recommendations)
- [UTCP vs MCP](https://www.utcp.io/utcp-vs-mcp)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| MCP purpose | MCP is an open standard for connecting model workflows to external tools and contextual data |
| MCP server | A server exposes tools; the agent or client retrieves the available tool list before using them |
| Tool discovery | MCP clients should list tools at initialization because tools and schemas can change |
| C# Agent Framework integration | A .NET agent can connect to an MCP server, list tools, convert them to AI tools, and invoke them through function calling |
| Security considerations | MCP calls can share prompt content or other data with tool servers, so trusted servers, credentials, and audit logging matter |
| Microsoft Learn MCP server | The Learn MCP endpoint is a remote MCP server at `https://learn.microsoft.com/api/mcp`; it exposes docs search and docs fetch tools |
| UTCP comparison | UTCP is useful to discuss direct tool contracts and tool calling without requiring a model-specific tool server pattern |

Practical recall:

```text
MCP = discover and call tools through a tool server.
UTCP/direct tools = call known tools/APIs through explicit contracts.
Both need governance, validation, and observability.
```

Security recall:

```text
Tool access is not just a developer convenience.
It is a data-sharing and execution boundary.
```

---

# Lab 06 - AgentGateway Baseline

## 1. Concept

Lab 06 introduces AgentGateway as a runtime control point.

Without a gateway:

```text
agent/app -> model or agent backend
```

With a gateway:

```text
agent/app -> gateway -> model/agent backend
```

The important idea:

```text
AgentGateway is where runtime traffic becomes routable, observable, policy-controlled, and attributable.
```

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Gateway endpoint | Central runtime URL | Route all agent traffic |
| Routing | Choose backend/model/agent | Environment-specific routing |
| Policy | Apply runtime rules | Rate limit, auth, allowlist |
| Attribution | Track caller/student/app | Cost and usage accountability |
| Request metadata | Correlation, identity, labels | Trace request across systems |
| Live vs dry-run mode | Simulate or call real gateway | Training safety, cost control |
| Response handling | Parse gateway result | Show success/error clearly |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- why gateways are useful for agent runtime traffic
- how requests can be routed through a central control point
- how policy and attribution are applied
- how dry-run mode differs from live mode
- how gateway responses are handled

The lab may cover:

```text
- configuring gateway endpoint
- building a request payload
- adding metadata headers
- sending dry-run or live request
- printing gateway response
- explaining route/policy behavior
```

## 4. Lab Flow Walkthrough

### Step 1 - Configuration Is Loaded

Example:

```csharp
var endpoint = Environment.GetEnvironmentVariable("PN_AGENTGATEWAY_ENDPOINT");
var liveMode = Environment.GetEnvironmentVariable("PN_AGENTGATEWAY_LIVE") == "true";
```

What this achieves:

```text
The app knows whether to use simulated or live gateway behavior.
```

### Step 2 - Request Is Built

Example:

```csharp
var request = new GatewayRequest
{
    Prompt = userInput,
    Deployment = Environment.GetEnvironmentVariable("PN_MODEL_DEPLOYMENT"),
    Metadata = new Dictionary<string, string>
    {
        ["batchId"] = batchId,
        ["studentId"] = studentId,
        ["labId"] = "day04-lab06"
    }
};
```

What this achieves:

```text
The gateway receives both task data and attribution metadata.
```

### Step 3 - Request Goes Through Gateway

Example:

```csharp
var response = await httpClient.PostAsJsonAsync(endpoint, request);
```

What this achieves:

```text
The gateway becomes the runtime path.
```

### Step 4 - Response Is Parsed

Example:

```csharp
var result = await response.Content.ReadFromJsonAsync<GatewayResponse>();
```

What this achieves:

```text
The app can display output, route info, policy info, or errors.
```

## 5. Recall Summary

```text
Lab 06 teaches that runtime agent traffic can be routed through a gateway for policy, routing, attribution, and observability.
```

## 6. References For This Lab

- [agentgateway Kubernetes documentation](https://agentgateway.dev/docs/kubernetes/latest/)

What to take from this reference:

| Reference area | What it helps you recall |
|---|---|
| Gateway role | agentgateway is a control plane and proxy data plane for HTTP/gRPC and agentic workloads |
| Unified traffic path | The same gateway can front normal APIs, LLM inference, MCP tool servers, and A2A agent traffic |
| Routing and policies | Gateway behavior includes routing, load balancing, timeouts, retries, TLS, rate limits, authorization, and traffic policies |
| Agent-specific needs | Agent traffic can involve stateful sessions, long-lived connections, tool fan-out, server-initiated events, and protocol-aware routing |
| MCP and A2A support | MCP and A2A are treated as first-class agentic traffic patterns |
| Operations | The docs include policies, security, traffic management, observability, tracing, and debugging |

Practical recall:

```text
Direct call:
app -> backend

Gateway call:
app -> gateway -> backend
```

Why this matters:

```text
The gateway gives one place to apply routing, security, rate limits, tracing, and cost controls.
```

---

# Lab 07 - Gateway Observability And Control

## 1. Concept

Lab 07 extends AgentGateway into operations.

The key question:

```text
How do we observe and control agent traffic after it is running?
```

Important runtime signals:

```text
request ID
correlation ID
traceparent
rate limit response
logs
latency
status code
cost/usage attribution
```

The important idea:

```text
An enterprise agent system must be observable and controllable, not just functional.
```

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Request ID | Unique request identifier | Debug one request |
| Correlation ID | Links related operations | Trace user action across services |
| Traceparent | W3C tracing context | Distributed tracing |
| Logs | Runtime records | Debug failures, audit behavior |
| Rate limiting | Control request volume | Protect backend, manage cost |
| Status codes | Runtime outcome | Success, policy rejection, throttled |
| Cost attribution | Link usage to caller | Student/team/customer chargeback |
| KQL/log query | Query operational data | Find errors, latency, hot callers |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- how to identify a request across services
- why correlation IDs matter
- how rate limiting protects systems
- how gateway logs support debugging
- how observability supports operations and governance

The lab may cover:

```text
- sending request with correlation metadata
- triggering or simulating rate limit
- printing request/correlation IDs
- viewing logs
- explaining traceparent
- connecting runtime evidence to operations
```

## 4. Lab Flow Walkthrough

### Step 1 - Request Metadata Is Created

Example:

```csharp
var requestId = Guid.NewGuid().ToString("n");
var correlationId = $"batch-{batchId}-student-{studentId}";
```

What this achieves:

```text
Each request can be traced.
```

### Step 2 - Metadata Is Sent

Example:

```csharp
httpRequest.Headers.Add("x-request-id", requestId);
httpRequest.Headers.Add("x-correlation-id", correlationId);
```

What this achieves:

```text
Gateway and backend logs can connect this request to the caller.
```

### Step 3 - Gateway Applies Control

Example outcomes:

```text
200 OK
429 Too Many Requests
403 Forbidden
500 Backend Error
```

What this achieves:

```text
The gateway can allow, throttle, reject, or route requests.
```

### Step 4 - Logs Are Reviewed

Example log fields:

```text
timestamp
requestId
correlationId
studentId
route
statusCode
latencyMs
tokens
```

What this achieves:

```text
Runtime behavior becomes observable.
```

### Step 5 - Operational Meaning Is Interpreted

Examples:

```text
429 means rate limit control is working.
Correlation ID links all operations for a student/session.
Request ID identifies one specific call.
Latency shows performance behavior.
```

What this achieves:

```text
The lab connects agent behavior to production operations.
```

## 5. Recall Summary

```text
Lab 07 teaches that production agent systems need observability, correlation, rate limits, logs, and operational control.
```

## 6. References For This Lab

- [Agent Framework observability](https://learn.microsoft.com/en-us/agent-framework/agents/observability?pivots=programming-language-csharp)
- [agentgateway Kubernetes documentation](https://agentgateway.dev/docs/kubernetes/latest/)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| Agent observability | Agent runs should expose runtime signals so behavior can be debugged, measured, and audited |
| Distributed tracing | Requests across agent, tool, model, and gateway layers need correlation identifiers |
| Logs and traces | Logs explain what happened; traces explain how a request moved across components |
| Gateway observability | Gateway metrics, logs, tracing, and policy outcomes show what happened at the traffic boundary |
| Operational control | Rate limits, authorization, retries, and routing decisions become visible through telemetry |

Practical recall:

```text
If you cannot trace it, you cannot operate it.
If you cannot correlate it, you cannot debug it across services.
```

---

# Day 04 Overall Map

| Lab | Main idea | What to remember |
|---|---|---|
| Lab 01 | Multi-agent architecture | Split responsibility across multiple agents |
| Lab 02 | Coordinator-worker orchestration | Use a coordinator to plan, assign, evaluate, and stop |
| Lab 03 | A2A | Agents can expose and consume runtime capability contracts |
| Lab 04 | AG-UI/A2UI | Agent-user interaction should use structured UI boundaries |
| Lab 05 | MCP vs UTCP | Tool access needs clear discovery and execution boundaries |
| Lab 06 | AgentGateway | Gateway centralizes routing, policy, attribution, and control |
| Lab 07 | Observability/control | Agent systems need logs, correlation, rate limits, and traces |

# How Day 04 Concepts Progress Across Labs

## 1. From One Agent To Many Agents

Day 03 ended with a workflow agent.

Day 04 begins by asking:

```text
What happens when one workflow is not enough?
```

Progression:

```text
Day 03 Lab 07 workflow agent
  -> Day 04 Lab 01 multiple specialist agents
  -> Day 04 Lab 02 coordinator-worker orchestration
```

What evolves:

| Earlier concept | Evolves into | Why it matters |
|---|---|---|
| Workflow step | Specialist agent | Work can be owned by focused agents |
| Workflow control | Coordinator | Planning and assignment become explicit |
| Validation step | Reviewer/validator worker | Quality checks can be delegated |
| Shared workflow state | Multi-agent scratchpad/state | Results must move between agents |

## 2. From Internal Collaboration To Runtime Interoperability

Labs 01 and 02 can run inside one application.

Lab 03 introduces runtime interoperability:

```text
agent inside app
  -> agent exposed through A2A endpoint
  -> another client/agent discovers and calls it
```

What evolves:

| Earlier concept | Evolves into | Why it matters |
|---|---|---|
| Local specialist agent | Remote A2A provider | Agents can live in different services |
| Method call | Runtime message | Collaboration crosses process boundaries |
| Local capability description | Agent card | Consumers can discover capability contracts |
| Internal route | Runtime endpoint | Agents become network-addressable |

## 3. From Chat Text To Structured UI Interaction

Lab 04 adds the user interface boundary.

Progression:

```text
agent response text
  -> structured UI event
  -> user action
  -> agent continues
```

What evolves:

| Earlier concept | Evolves into | Why it matters |
|---|---|---|
| Plain response | UI event payload | Apps can render predictable experiences |
| HITL approval | UI action | Human decisions become structured inputs |
| Agent status text | Progress update | Users can see workflow progress |
| Chat-only experience | Teams/app cockpit | Agent becomes part of an application surface |

## 4. From Tools To Tool Boundaries

Day 03 introduced tools and skills.

Day 04 Lab 05 asks how tools are exposed across systems:

```text
local tool/function
  -> tool server discovery
  -> direct API tool contract
  -> governed tool execution
```

What evolves:

| Earlier concept | Evolves into | Why it matters |
|---|---|---|
| C# function tool | External tool contract | Tools can live outside the agent app |
| Skill script/function | MCP server tool | Capabilities can be discovered dynamically |
| Known API call | UTCP/direct tool | Strong contracts can be simpler and safer |
| Tool approval | Tool governance | Execution needs policy and observation |

## 5. From Runtime Calls To Gateway-Controlled Traffic

Labs 03 and 05 introduce distributed calls.

Lab 06 adds a gateway:

```text
direct agent/tool/model call
  -> gateway route
  -> policy-controlled runtime path
```

What evolves:

| Earlier concept | Evolves into | Why it matters |
|---|---|---|
| Direct model call | Gateway-routed call | Centralized policy and routing |
| Local metadata | Attribution headers | Usage can be tied to caller/session |
| Individual endpoint | Managed route | Traffic can be controlled consistently |
| Per-app logic | Gateway policy | Cross-cutting rules move to runtime layer |

## 6. From Working System To Operable System

Lab 07 closes Day 04 by making runtime behavior observable.

Progression:

```text
agent request
  -> request ID
  -> correlation ID
  -> trace/log record
  -> rate-limit or policy result
  -> operational insight
```

What evolves:

| Earlier concept | Evolves into | Why it matters |
|---|---|---|
| Tool/model output | Operational evidence | You can debug and audit behavior |
| Student/session ID | Attribution | Usage and cost can be assigned |
| Gateway response | Runtime control signal | Policies become visible |
| Logs | Observability queries | Operations can find failures and patterns |

## 7. End-To-End Day 04 Mental Model

By the end of Day 04, the system has evolved like this:

```text
single controlled agent
  -> multiple specialist agents
  -> coordinator-worker orchestration
  -> A2A runtime interoperability
  -> structured agent-user UI boundary
  -> governed tool boundary
  -> gateway-controlled runtime path
  -> observable and controllable operations layer
```

The final mental model:

```text
An enterprise AI-native system is not one chatbot.
It is a network of agents, tools, protocols, user interfaces, gateways, and observability controls.
```

# References

- [Agent Framework overview](https://learn.microsoft.com/en-us/agent-framework/)
- [Agent Framework skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp)
- [Agent Framework middleware](https://learn.microsoft.com/en-us/agent-framework/agents/middleware/)
- [Workflow orchestrations in Agent Framework](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/)
- [Magentic orchestration in Agent Framework](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/magentic?pivots=programming-language-csharp)
- [Agent-to-Agent journey](https://learn.microsoft.com/en-us/agent-framework/journey/agent-to-agent)
- [A2A Integration with Agent Framework](https://learn.microsoft.com/en-us/agent-framework/integrations/a2a?tabs=dotnet-cli%2Cuser-secrets&pivots=programming-language-csharp)
- [AG-UI Integration with Agent Framework](https://learn.microsoft.com/en-us/agent-framework/integrations/ag-ui/?pivots=programming-language-csharp)
- [Using MCP tools with Agents](https://learn.microsoft.com/en-us/agent-framework/agents/tools/local-mcp-tools?pivots=programming-language-csharp)
- [Microsoft Learn MCP Server developer reference](https://learn.microsoft.com/en-us/training/support/mcp-developer-reference?source=recommendations)
- [UTCP vs MCP](https://www.utcp.io/utcp-vs-mcp)
- [agentgateway Kubernetes documentation](https://agentgateway.dev/docs/kubernetes/latest/)
- [Agent Framework observability](https://learn.microsoft.com/en-us/agent-framework/agents/observability?pivots=programming-language-csharp)
- [Azure API Center](https://learn.microsoft.com/en-us/azure/api-center/)
- [Azure Container Apps logs](https://learn.microsoft.com/en-us/azure/container-apps/log-options)
