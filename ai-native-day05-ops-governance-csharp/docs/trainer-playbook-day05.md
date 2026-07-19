# Trainer Playbook - Day 5

Program: ProNative AI-Native Fullstack Engineering  
Day: Day 5  
Batch: `AN-2607-101`  
Theme: AI Operations, Governance, FinOps, and Hybrid AI walkthrough  
Primary stack: Microsoft Foundry, Azure Monitor, Application Insights, Log Analytics, AgentGateway, Agent 365, Entra, Purview, Defender, Azure Cost Management, Foundry Local on CPU, Runpod

This is the trainer's single delivery reference for Day 5. The final Day 5 delivery pack should contain only three markdown files: trainer playbook, student playbook, and environment readiness.

## 1. Day 5 Storyline

Day 1 and Day 2 created generative AI apps and Foundry agents.  
Day 3 and Day 4 elevated them into workflows, skills, memory, multi-agent systems, protocols, and gateway patterns.  
Day 5 asks the operational question:

```text
If these AI-native systems are real enterprise systems, how do we operate, govern, secure, observe, evaluate, and control cost?
```

Day 5 is intentionally walkthrough-heavy. Students should not leave with a new large codebase. They should leave with an operating model that prepares them for Day 6 Foundation Pods.

## 2. Delivery Principles

| Principle | Trainer behavior |
|---|---|
| Use live platform surfaces where possible | Prefer Foundry, Azure Monitor, App Insights, Log Analytics, Cost Management, AgentGateway, and admin surfaces. |
| Be honest about readiness | If Agent 365, AgentGateway, Runpod, or Foundry Local is not fully ready, explain the boundary and show the fallback evidence. |
| Tie every topic to Day 1-4 | Do not teach operations as an abstract discipline. Anchor it to apps, agents, workflows, tools, memory, and gateway calls. |
| Prepare Day 6 | Every walkthrough should end with "what Foundation Pod must provide." |
| Avoid over-coding | This day is platform walkthrough, operations thinking, and architecture readiness. |
| Keep DGX Spark as future option | This batch uses local CPU for Foundry Local and Runpod for neocloud. |

## 3. Day 5 Agenda

| Time | Session | Mode | Outcome |
|---:|---|---|---|
| 20 min | Opening and Day 1-4 recap | Trainer framing | Students see why operations is now unavoidable. |
| 60 min | AI Operations walkthrough | Platform walkthrough | Students map requests to runtime, model, tool, memory, gateway, and logs. |
| 75 min | LLMOps and GenAIOps walkthrough | Concept plus platform walkthrough | Students understand prompt/model/eval/trace/release/feedback loops. |
| 60 min | Observability walkthrough | Foundry, App Insights, Log Analytics | Students see latency, failures, traces, and cost signals. |
| 60 min | AgentGateway operations | Gateway walkthrough | Students understand routing, policy, rate limit, identity, and attribution. |
| 75 min | Governance and security | Admin/security walkthrough | Students connect responsible AI, Agent 365, Entra, Purview, Defender, and approval boundaries. |
| 60 min | FinOps for AI | Cost walkthrough | Students understand budget, Azure service cost, token usage, model cost, and weekend controls. |
| 90 min | Hybrid AI walkthrough | Foundry Local CPU and Runpod | Students compare Azure-hosted, local CPU, and neocloud execution. |
| 40 min | Day 6 bridge | Foundation Pod planning | Students know what Day 6 must provision and validate. |

## 3.1 Day 5 Code Lab Pack

Day 5 is still walkthrough-led, but the following compact labs give the walkthrough concrete evidence.

Repo:

```text
outputs/starter-repositories/ai-native-day05-ops-governance-csharp
```

Run labs at the point where the session needs the evidence. Do not run them strictly in numeric order at the beginning of the day.

| Session | Topic | Lab action | What it proves |
|---|---|---|---|
| Session 1 | AI Operations | Run Lab 02 Foundry Operational Trace | Model calls need latency, model/deployment, token, request, and trace evidence. |
| Session 2 | LLMOps / GenAIOps | Reference Lab 01 / Lab 02 evidence | Prompt, model, eval, telemetry, and feedback assets need lifecycle evidence. |
| Session 3 | Observability | Run Lab 01 Observability Telemetry | Live Foundry calls can emit AI-specific operational tags to App Insights / Log Analytics. |
| Session 4 | AgentGateway | Run Lab 03 AgentGateway Operational Client | Gateway-mediated AI traffic should carry route, trace, identity, and cost attribution headers. |
| Session 5 | Governance and security | M365 admin walkthrough; preview Lab 07 concept only | Governance needs inventory, identity, access, data protection, security, policy, and audit evidence. |
| Session 6 | FinOps | Run Lab 04 AI FinOps Evidence | Cost control depends on Azure cost, model usage, budget evidence, and runtime attribution. |
| Session 7 | Hybrid AI | Run Lab 05 Foundry Local CPU and Lab 06 Runpod Neocloud Client | Local and neocloud runtimes need lifecycle, secrets, latency, worker, and cost controls. |
| Day 5 close | Governance evidence check | Run Lab 07 Governance Policy Check after Labs 01-06, or mark optional if time is short | Live operational evidence should be mapped to deterministic governance decisions. |

Run only the labs that fit the day timing. Do not position missing secrets or missing telemetry as successful lab completion; use those as readiness findings.

Important: Lab 07 is not a normal Session 5 lab. Session 5 is an M365-admin-led governance walkthrough from a separate tenant. Lab 07 is best used near the end of Day 5, after Labs 01-06 have generated live telemetry.

## 4. Opening

### 4.1 Trainer Narrative

Use this opening:

```text
For the first four days, we focused on building AI-native capability.
Today we switch roles. We think like platform engineers, AI operations engineers, security reviewers, and FinOps owners.

An enterprise agent is not production-ready because it can answer a prompt.
It is production-ready only when we can observe it, govern it, secure it, evaluate it, control cost, and recover when something fails.
```

### 4.2 Poll

Ask:

```text
Which failure would worry you most in a production AI-native system?
```

Options:

- wrong answer with high confidence
- tool call executed without approval
- runaway token/cost usage
- no trace explaining what happened
- sensitive data leakage

Use the answers to decide which examples to emphasize during Day 5.

## 5. Session 1 - AI Operations Walkthrough

### 5.1 Goal

Students should understand that AI operations is the control system around model-powered work.

### 5.2 Core Model

Explain the operational path:

```text
user request
  -> app or agent
  -> instructions
  -> model
  -> tools
  -> memory/context
  -> gateway/policy
  -> telemetry
  -> evaluation
  -> cost attribution
  -> governance review
```

### 5.3 Trainer Walkthrough

Use this as an operations control-room walkthrough. Open the tools in this order so students see how the same request becomes an operational story across AI, application, platform, cost, and governance surfaces.

| Step | Tool / portal surface | Exact navigation | What to show | Teaching focus |
|---|---|---|---|---|
| 1 | Azure portal - landing zone resource groups | Azure portal -> Resource groups -> `rg-ai-shared-platform-an2607101`, `rg-ai-observability-an2607101`, `rg-ai-governance-hub-an2607101`, and one student resource group | resource names, locations, tags, shared vs student-specific grouping | Operations starts with ownership, scope, and resource boundaries. |
| 2 | Microsoft Foundry project | Microsoft Foundry -> select `proj-an2607101-default` -> project home / Build area | project, model deployment, agents, evaluations, traces/monitoring tabs if available | Foundry is the AI asset and AI runtime visibility surface. |
| 3 | Model deployment evidence | Foundry -> Models -> Deployments -> `gpt-5-mini` | deployment name, model, region, deployment type, quota/capacity if visible | Every incident or cost review must know which deployment served the request. |
| 4 | Agent evidence | Foundry -> Agents -> select a Day 2 agent -> Playground / Details / Traces / Monitor | instructions, tools, knowledge/grounding, traces, monitor view if populated | Agents are operational assets, not just prompts. |
| 5 | Application Insights | Azure portal -> search `proj-an2607101-default-resource-appinsights` or open it from `rg-ai-shared-platform-an2607101` if deployed there | request rate, failed requests, response time, availability widgets if populated | App-side telemetry tells whether the application is alive and healthy. |
| 6 | Application map | Application Insights -> Application map | components, dependencies, highlighted failures/latency if available | A model, tool, database, gateway, or API call should eventually appear as a dependency. |
| 7 | Transaction investigation | Application Insights -> Transaction search / Search -> open a recent operation | operation ID, request, dependencies, traces, exceptions | Debugging starts with one correlated operation, not averages. |
| 8 | Log Analytics | Azure portal -> Resource groups -> `rg-ai-observability-an2607101` -> Log Analytics workspace -> Logs | KQL query editor and tables such as requests/dependencies/traces/exceptions or App-prefixed equivalents | KQL is the reliable path when portal charts are insufficient. |
| 9 | AgentGateway on AKS | Azure portal -> Resource groups -> `rg-ai-governance-hub-an2607101` -> AKS cluster -> Workloads / Services and ingresses / Insights / Logs | gateway deployment, pods, services, ingress/DNS, logs, metrics | Gateway operations control route, policy, rate limit, identity, and attribution. |
| 10 | Cost view | Azure portal -> Subscription -> Cost Management -> Cost analysis | accumulated cost, group by resource group, service name, resource, and tags | AI operations must connect runtime activity to spend. |
| 11 | Tags and cleanup | Azure portal -> open resource or resource group -> Tags | `BatchId`, `EnvironmentId`, `StudentId`, `CostCenter`, `DeleteAfter`, `Owner` | Tags are operational metadata used for cost, cleanup, ownership, and audit. |

Use the Day 1-4 artifacts as the narrative thread:

| Day artifact | Operational question | Where to inspect |
|---|---|---|
| Day 1 Foundry app | Which model was used, how much latency occurred, how many tokens were consumed? | Foundry deployment, Application Insights dependencies/traces, Cost Management |
| Day 1 RAG grounding | Which source was used, was the answer grounded, can we inspect evidence? | app logs, trace properties, data source/resource used |
| Day 2 Foundry agent | What instructions and tools were active? | Foundry -> Agents -> selected agent -> Details / Tools / Knowledge / Traces |
| Day 3 workflow | Which step failed or retried? | console output, workflow trace, Application Insights traces if instrumented |
| Day 3 memory | Where did state live and how was it retrieved? | Cosmos DB / local state output / trace properties |
| Day 4 multi-agent flow | Which agent made which decision? | console trace, trace IDs, agent names, span attributes |
| Day 4 gateway flow | Which route, policy, backend, and identity were used? | AKS gateway logs, gateway route config, Application Insights dependencies |

### 5.4 What To Show

Do not stop at "we have logs." Show the exact evidence students should learn to look for.

| Tool | Menu / UI element | Evidence to point at | Why it matters |
|---|---|---|---|
| Microsoft Foundry | Project selector -> `proj-an2607101-default` | correct project context | Prevents debugging the wrong project or deployment. |
| Microsoft Foundry | Models -> Deployments -> `gpt-5-mini` | model/deployment/region/capacity | Establishes runtime identity and reproducibility. |
| Microsoft Foundry | Agents -> select agent -> Details | instructions, model, connected tools, knowledge | Shows what the agent was allowed to know and do. |
| Microsoft Foundry | Agents -> select agent -> Traces | trace list, spans, inputs/outputs, tool calls, latency, cost if available | Explains "why did the agent answer that way?" |
| Microsoft Foundry | Evaluations | evaluation runs, metrics, row-level samples if available | Connects quality and safety to repeatable evidence. |
| Application Insights | Overview | requests, failed requests, response time | Establishes service health. |
| Application Insights | Application map | dependency nodes and edges | Shows app -> model/tool/database/gateway dependency shape. |
| Application Insights | Transaction search / Search | operation ID, request, dependencies, traces | Reconstructs one end-to-end request. |
| Application Insights | Failures | failed requests, exceptions, failed dependencies | Separates runtime bugs from model quality issues. |
| Application Insights | Performance | slow operations and dependency latency | Supports latency and routing decisions. |
| Application Insights | Logs | KQL editor | Enables exact operational questions. |
| Log Analytics workspace | Logs | workspace-level KQL across App Insights tables | Supports batch/workspace-level analysis. |
| Application Insights | Agents (Preview) | AI agent monitoring page if populated; "Get started" page if telemetry is missing | This view requires AI-agent telemetry collection and recent agent activity. Treat an empty page as a readiness finding, not as a demo failure. |
| AKS | Workloads | AgentGateway deployment, pod status, restart count, image | Confirms gateway runtime health. |
| AKS | Services and ingresses | external endpoint, service, ingress, DNS | Confirms how traffic enters the gateway. |
| AKS | Insights | CPU, memory, pod health | Connects gateway reliability to cluster operations. |
| AKS / Log Analytics | Logs | gateway access logs, status codes, latency, route/backend fields if emitted | Shows policy and route evidence. |
| Cost Management | Cost analysis -> Group by | resource group, service name, resource, tag | Shows cost attribution and top spend drivers. |
| Resource group / resource | Tags | `BatchId`, `EnvironmentId`, `StudentId`, `DeleteAfter` | Shows cleanup and ownership metadata. |

If a surface has no data, do not skip it. Say:

```text
Empty evidence is also an operations finding. It tells us the Foundation Pod must add instrumentation, tags, or trace propagation before live project delivery.
```

### 5.5 Key Teaching Points

- AI operations must include model, app, agent, tool, data, gateway, identity, and cost signals.
- Normal application logs are useful but insufficient for agentic systems.
- The more autonomy an agent has, the stronger the need for traceability and policy.
- Day 6 Foundation Pod must make these controls repeatable.

## 6. Session 2 - LLMOps And GenAIOps Walkthrough

### 6.1 Goal

Students should connect LLMOps and GenAIOps to the engineering lifecycle they have already experienced.

### 6.2 LLMOps Inner And Outer Loop

Explain:

| Loop | Meaning | Day 1-4 connection |
|---|---|---|
| Inner loop | Design, prompt, build, test, evaluate, refine | Day 1 apps, Day 2 agents, Day 3 harness/evals |
| Outer loop | Deploy, monitor, govern, collect feedback, improve | Day 5 operations, Day 6 Foundation Pod |

### 6.3 GenAIOps Operating Loop

Use this flow:

```text
plan
  -> version prompts/instructions
  -> evaluate outputs
  -> automate checks
  -> deploy
  -> monitor performance/cost
  -> trace/debug
  -> collect feedback
  -> improve
```

### 6.4 Trainer Walkthrough

Use this as a lifecycle walkthrough, not a generic discussion. The goal is to show where each LLMOps / GenAIOps artifact lives and how it becomes repeatable in Day 6.

| Step | Lifecycle area | Exact navigation or artifact | What to show | Teaching focus |
|---|---|---|---|---|
| 1 | Prompt and instruction source | Foundry -> Agents -> select agent -> Details / Instructions | system/developer instructions, model selection, tool/knowledge configuration | Instructions are deployable assets and must be reviewed/versioned. |
| 2 | App prompt source | Starter repository -> `src` project -> prompt-building code / configuration | prompt templates, structured output instructions, model deployment config | App prompts should not be hidden in random code paths. |
| 3 | Versioning | GitHub / local repo -> commits, branches, `docs`, `prompts`, `evals` folders where available | commit history or folder convention | LLMOps starts when AI assets are source-controlled. |
| 4 | Evaluation design | Foundry -> Evaluations -> create/view run OR code-based eval folder | dataset, evaluator list, run status, aggregate metrics, sample-level rows | Evaluation converts opinion into measurable evidence. |
| 5 | Harness and regression thinking | Day 3 Lab 05 output / harness evidence | repeatable prompts, expected checks, run comparison | Harness output becomes regression checks before release. |
| 6 | Runtime trace | Foundry -> Agents -> selected agent -> Traces OR Application Insights -> Transaction search | one trace with spans, inputs/outputs, tool call, duration | Traces connect quality findings to runtime behavior. |
| 7 | Monitoring | Foundry -> agent Monitor / Agent Monitoring Dashboard if available, plus Application Insights -> Overview / Performance / Failures | token usage, latency, success/failure, quality signals where populated | GenAIOps monitors production behavior, not only lab output. |
| 8 | Feedback capture | Foundry evaluation samples, issue tracker, trainer notes, or future Day 6 feedback file | bad answer, corrected answer, source evidence, label | Feedback becomes evaluation data or prompt changes. |
| 9 | Release readiness | GitHub Actions concept / Day 6 checklist | build, eval, policy, telemetry, cost, approval checklist | A model app should not be released only because it compiles. |

#### 6.4.1 Foundry Agent Instruction Walkthrough

Open:

```text
Microsoft Foundry
  -> select `proj-an2607101-default`
  -> Agents
  -> select one Day 2 agent
  -> Details
```

Show these UI elements:

| UI element | What to explain |
|---|---|
| Agent name | Naming is the first governance handle. |
| Model / deployment | Quality, latency, and cost depend on model choice. |
| Instructions | This is an LLMOps asset and should be versioned. |
| Tools | Tool availability changes the agent's action boundary. |
| Knowledge / grounding | Grounding configuration affects accuracy and data governance. |
| Publish/status fields if present | Runtime lifecycle matters: draft, tested, published, retired. |

Ask students:

```text
If this agent gives a wrong answer tomorrow, which fields here help us reproduce what happened?
```

#### 6.4.2 Evaluation Walkthrough

Open:

```text
Microsoft Foundry
  -> Evaluations
  -> open an existing evaluation run if available
```

If a run exists, show:

| UI element | What to explain |
|---|---|
| Run name / timestamp / creator | Evaluation runs need ownership and reproducibility. |
| Dataset | The quality gate is only as good as the test cases. |
| Evaluators | Groundedness, relevance, coherence, fluency, safety, or agent/tool metrics depending configured evaluators. |
| Aggregate scores | Good for trend view, not enough for debugging. |
| Row-level results | This is where students inspect individual failures. |
| Compare runs if available | This shows whether a prompt/model change improved or regressed behavior. |

If no run exists, show the menu location and say:

```text
This is a Day 6 Foundation Pod requirement: create a small baseline evaluation run before live projects begin.
```

#### 6.4.3 Trace And Monitor Walkthrough

Open:

```text
Microsoft Foundry
  -> Agents
  -> select agent
  -> Traces
```

Then open:

```text
Azure portal
  -> Application Insights
  -> Transaction search
```

Show the relationship:

| Foundry / App Insights evidence | What students should learn |
|---|---|
| trace/span | one operation is made of nested steps |
| input/output | prompt and answer are inspectable evidence and may contain sensitive data |
| tool call span | tool use must be observable and governable |
| duration/latency | latency should be measured per step |
| token/cost fields if emitted | FinOps depends on instrumentation |
| operation ID / trace ID | correlation makes troubleshooting possible |

#### 6.4.4 Feedback To Improvement Walkthrough

Use one bad or weak answer from earlier labs and show the improvement loop:

```text
bad answer
  -> capture user prompt and response
  -> classify failure reason
  -> add or update evaluation case
  -> adjust instruction / grounding / tool / model route
  -> rerun evaluation
  -> deploy only if quality and operations checks pass
```

Use this classification table:

| Failure type | Likely fix |
|---|---|
| missing context | improve grounding or retrieval |
| wrong tool | adjust tool instructions or policy |
| unsafe action | add deterministic approval/policy |
| verbose or costly answer | prompt/output constraints, cheaper model route |
| slow response | inspect dependency latency and model route |
| inconsistent answer | add evaluation cases and stricter instructions |

### 6.5 Poll

Ask:

```text
Which asset should be versioned first in a GenAIOps process?
```

Options:

- prompts and instructions
- model deployment name
- evaluation dataset
- gateway route config
- all of the above

Expected discussion:

All matter, but prompts/instructions and evaluation datasets are usually the first practical entry point for the training workflow.

### 6.6 Day 6 Bridge

Day 6 Foundation Pod must produce:

- prompt/instruction versioning approach
- evaluation folder or dataset pattern
- baseline eval run
- telemetry capture
- release checklist
- feedback capture path

## 7. Session 3 - Observability Walkthrough

### 7.1 Goal

Students should understand what observability must capture for AI-native systems.

### 7.2 Signals To Explain

| Signal | Why it matters |
|---|---|
| Latency | User experience, model selection, routing decisions |
| Token usage | Cost, prompt design, RAG strategy |
| Model/deployment | Reproducibility and incident review |
| Prompt/instructions version | Change control |
| Tool call | Security and correctness |
| Gateway route | Runtime control and attribution |
| Retrieval source | Grounding and auditability |
| Agent/session ID | Multi-step traceability |
| Error/exception | Reliability |
| Evaluation score | Quality gate |

### 7.3 Foundry Walkthrough

Open Microsoft Foundry first, before Azure Monitor, so students understand the AI-specific source of the telemetry.

```text
Microsoft Foundry
  -> select `proj-an2607101-default`
```

| Step | Foundry menu / UI element | What to show | Trainer narration |
|---|---|---|---|
| 1 | Project selector / project home | project name, connected Azure resources if visible | "Always verify the project before inspecting traces or evaluations." |
| 2 | Models -> Deployments | `gpt-5-mini`, deployment type, region, status | "The deployment name must be captured in telemetry for reproducibility and cost review." |
| 3 | Agents -> select an agent | model, instructions, tools, knowledge/grounding | "Agent behavior is determined by configuration, not only by code." |
| 4 | Agents -> selected agent -> Traces | recent traces, trace details, spans, inputs/outputs, tool calls, latency | "Tracing explains how the agent reached an answer." |
| 5 | Agents -> selected agent -> Monitor / Agent Monitoring Dashboard if available | token usage, latency, success/failure, evaluation or quality signals if populated | "Monitoring is the production view; traces are the investigation view." |
| 6 | Evaluations | evaluation run list, aggregate metrics, sample-level rows, compare runs if available | "Evaluation turns quality into evidence." |
| 7 | Guardrails / content safety / responsible AI area if available | configured filters or safety controls | "Safety controls are part of operations, not a separate afterthought." |
| 8 | Connected resources / Application Insights connection if visible | linked Application Insights resource | "Foundry traces flow into Azure Monitor/Application Insights for enterprise observability." |

If Foundry has no traces:

```text
No trace data here means one of three things: tracing is not connected, the current agent type is not emitting traces, or the trainer account lacks telemetry access. That is an operations readiness finding.
```

### 7.4 App Insights And Log Analytics Walkthrough

Show basic queries:

```kusto
requests
| where timestamp > ago(24h)
| summarize RequestCount=count(), AvgDurationMs=avg(duration) by cloud_RoleName
| order by RequestCount desc
```

```kusto
exceptions
| where timestamp > ago(24h)
| take 25
```

```kusto
traces
| where timestamp > ago(24h)
| take 50
```

If custom telemetry is not yet available:

```text
This is exactly why Day 6 Foundation Pod needs a standard logging and tracing convention.
```

### 7.5 Detailed Observability Walkthrough Script

Use this as the actual trainer runbook. The goal is to make observability concrete, even if the current alpha-batch telemetry is incomplete.

#### 7.5.1 Open The Surfaces In This Order

| Step | Surface | Exact navigation | What to show | Trainer narration |
|---|---|---|---|---|
| 1 | Foundry project | Microsoft Foundry -> `proj-an2607101-default` | project, model deployment, agents/evaluations/monitoring surfaces | "Foundry is where AI assets are built and some AI-specific visibility begins." |
| 2 | Application Insights Overview | Azure portal -> search `proj-an2607101-default-resource-appinsights` -> Overview | health, usage, response time if available | "This is the app-side observability resource." |
| 3 | Application map | Application Insights -> Application map | dependency nodes/edges if available | "A model call, tool call, database call, and gateway call should eventually appear as dependencies." |
| 4 | Transaction search / Search | Application Insights -> Transaction search or Search | one operation or recent traces | "This is where we investigate a single request." |
| 5 | Failures | Application Insights -> Failures | exceptions and failed dependencies | "AI failures are not only crashes; failed tool/model/gateway calls also matter." |
| 6 | Performance | Application Insights -> Performance | latency by operation and dependency | "Latency must be decomposed, not guessed." |
| 7 | Live metrics | Application Insights -> Live metrics | live request/failure stream if available | "This is useful during demos and load tests." |
| 8 | Logs | Application Insights -> Logs or Log Analytics workspace -> Logs | KQL query editor | "When UI views are sparse, KQL is the trainer's reliable path." |
| 9 | Workbooks | Application Insights -> Workbooks | workbook gallery and custom workbook option | "Day 6 can turn these queries into repeatable dashboards." |
| 10 | Agents (Preview) | Application Insights -> Agents (Preview) | AI agent metrics if data exists, or "Get started with monitoring AI agents" if no data exists | "This view is only useful after AI-agent telemetry is configured and recent agent calls have flowed in." |

#### 7.5.2 Explain Application Insights Views

| View | What it answers | AI-native interpretation |
|---|---|---|
| Overview | Is the app alive and emitting telemetry? | If no telemetry exists, instrumentation is incomplete. |
| Application map | What dependencies does the app call? | Look for Foundry/model, Cosmos, gateway, tool APIs, MCP/A2A endpoints. |
| Live metrics | What is happening right now? | Useful during live traffic or load tests. |
| Transaction search | What happened in one request? | Needed for prompt -> model -> tool -> response trace. |
| Failures | What failed? | Tool failures, model endpoint failures, gateway failures, auth failures. |
| Performance | What is slow? | Helps compare model latency, retrieval latency, gateway overhead. |
| Logs | What can we query precisely? | KQL enables custom operational questions. |
| Workbooks | What can we standardize? | Day 6 Foundation Pod should provide reusable dashboards. |

#### 7.5.3 KQL Walkthrough Pack

Start with request volume:

```kusto
requests
| where timestamp > ago(24h)
| summarize RequestCount=count(), AvgDurationMs=avg(duration), Failures=countif(success == false) by cloud_RoleName
| order by RequestCount desc
```

Then show dependency health:

```kusto
dependencies
| where timestamp > ago(24h)
| summarize Calls=count(), AvgDurationMs=avg(duration), Failures=countif(success == false) by target, type
| order by Calls desc
```

Then show failures:

```kusto
exceptions
| where timestamp > ago(24h)
| project timestamp, cloud_RoleName, type, outerMessage, operation_Id
| order by timestamp desc
| take 25
```

Then show traces:

```kusto
traces
| where timestamp > ago(24h)
| project timestamp, cloud_RoleName, message, severityLevel, operation_Id
| order by timestamp desc
| take 50
```

If there is an operation ID:

```kusto
let op = "<paste-operation-id>";
union requests, dependencies, traces, exceptions
| where operation_Id == op
| order by timestamp asc
```

If the workspace uses App-prefixed tables:

```kusto
AppRequests
| where TimeGenerated > ago(24h)
| summarize RequestCount=count(), AvgDurationMs=avg(DurationMs), Failures=countif(Success == false) by AppRoleName
| order by RequestCount desc
```

#### 7.5.4 AI-Specific Fields To Discuss

Open one log row and inspect properties/custom dimensions.

| Field | Why it matters | Day 6 requirement if missing |
|---|---|---|
| `BatchId` | batch cost and trace attribution | add standard telemetry tags |
| `StudentId` | student-level attribution | add student identity/tag propagation |
| `agentName` / `agentId` | agent inventory and debugging | name every agent consistently |
| `sessionId` / `threadId` | multi-turn trace | propagate session IDs |
| `model` / `deployment` | model reproducibility | log deployment name on each call |
| `promptVersion` | prompt governance | version prompts/instructions |
| `toolName` | tool safety and failure review | log tool calls |
| `totalTokens` | FinOps | capture token usage |
| `gatewayRoute` | gateway operations | propagate gateway route |
| `groundingSource` | RAG audit | log retrieval source/document |

#### 7.5.5 Trainer Explanation For Sparse Telemetry

Use this wording:

```text
Sparse telemetry is not a failure of today's topic. It is evidence that operations must be designed, not assumed.
The Day 6 Foundation Pod should standardize logging, tracing, tags, operation IDs, token usage, and dashboards before live projects begin.
```

### 7.6 Poll

Ask:

```text
For debugging a bad agent answer, which signal is least optional?
```

Options:

- original user input
- final answer
- tool calls
- retrieved context
- model and prompt/instruction version

Expected discussion:

All are important. The key point is that final answer alone is not enough.

## 8. Session 4 - AgentGateway Operations

### 8.1 Goal

Students should understand why gateway control becomes important when multiple agents, tools, and model endpoints exist.

### 8.2 Gateway Positioning

Explain:

```text
Applications and agents should not directly scatter calls to every model, tool, MCP server, and A2A endpoint.
The gateway gives one operational control plane for routing, authentication, policy, rate limits, retries, timeouts, observability, and attribution.
```

### 8.3 Walkthrough Areas

| Area | Exact AKS / gateway surface | What to show | Teaching focus |
|---|---|---|---|
| Runtime | Azure portal -> `rg-ai-governance-hub-an2607101` -> AKS cluster -> Overview | cluster status, region, node pool health | Gateway is a live runtime, not only configuration. |
| Workloads | AKS -> Workloads | AgentGateway deployment, replica count, pod status, restart count, image/version | Runtime health and rollout status must be visible. |
| Exposure | AKS -> Services and ingresses | service, ingress, public/private endpoint, DNS `aks-an2607101-agw-dns-xywyud9k.hcp.centralindia.azmk8s.io` | Students should know where traffic enters. |
| Routing | gateway config / route manifest / GitHub Ops repo if available | route path, backend model/tool/API/MCP/A2A endpoint | Gateway routing removes scattered direct calls. |
| Backend identity | AKS -> Workloads / service account / managed identity or workload identity configuration | how gateway authenticates to Foundry or services | Gateway should not rely on hard-coded secrets. |
| Policy | route policy config | allow/deny, timeout, retry, approval, auth policy | Policy belongs at the runtime boundary. |
| Rate limit | route policy config / gateway docs | request or token-based limits | Runaway model usage needs deterministic limits. |
| Observability | AKS -> Insights / Logs and Log Analytics -> Logs | request logs, status codes, latency, route/backend fields | Gateway logs connect operations, security, and FinOps. |
| Cost tags | request headers / trace properties / logs | `BatchId`, `StudentId`, `EnvironmentId`, route/workload | Cost attribution must travel with each call. |

### 8.4 Detailed AgentGateway Walkthrough Script

#### 8.4.1 Show The Runtime Path

Draw or narrate this path before opening the portal:

```text
student/app/agent
  -> AgentGateway route
  -> policy and auth check
  -> backend model/tool/API/MCP/A2A endpoint
  -> response
  -> logs/metrics/traces/cost attribution
```

Emphasize:

```text
The gateway is where operations teams can control traffic without editing every agent.
```

#### 8.4.2 Open AKS / Gateway Runtime

In Azure portal:

```text
Resource groups
  -> rg-ai-governance-hub-an2607101
  -> AKS cluster
```

Walk through these menu items:

| Portal menu / UI element | What to click or inspect | What to explain |
|---|---|---|
| Overview | status, location, Kubernetes version, node pools summary | Confirms the cluster is available before discussing gateway behavior. |
| Node pools | node pool size, VM SKU, autoscale status if visible | Explains the compute cost and scale boundary for the gateway. |
| Workloads | AgentGateway deployment/stateful set if visible | Shows desired replicas, available replicas, image, age, restart count. |
| Pods under Workloads | select a gateway pod | Show pod phase, restarts, events, and container status. |
| Services and ingresses | gateway service/ingress | Show public/private exposure and DNS name. Use `https://aks-an2607101-agw-dns-xywyud9k.hcp.centralindia.azmk8s.io` as the current endpoint. |
| ConfigMaps | gateway route/config entries if surfaced | Show that backend URLs and route behavior are configuration, not app code. |
| Secrets | presence only, never reveal values | Explain secret references and key rotation expectations. |
| Service accounts / Identity if visible | workload identity or managed identity binding | Explain identity-based access to Azure services. |
| Insights | CPU, memory, pod health, restart trend | Gateway reliability is observable like any platform service. |
| Logs | container logs or Log Analytics query link | Show request, route, status, latency, and error evidence when available. |

If the Azure portal does not expose the config cleanly, open the GitHub Ops/Terraform/Kubernetes manifest used to deploy AgentGateway and show:

| Config item | Why students should see it |
|---|---|
| route path | The caller-facing contract. |
| backend endpoint | The actual model/tool/API/MCP/A2A target. |
| auth policy | Who can call the route. |
| rate limit | How spend and runaway traffic are constrained. |
| timeout/retry | How reliability is controlled. |
| logging/telemetry setting | How operations evidence is produced. |

#### 8.4.3 Route And Policy Discussion

Use a table even if the live config is not fully ready:

| Route type | Example | Policy concern |
|---|---|---|
| model route | `/openai/deployments/gpt-5-mini` | model allowlist, token/cost attribution |
| tool route | `/tools/inventory` | auth, allowed operations |
| MCP route | `/mcp/*` | tool server authorization |
| A2A route | `/a2a/*` | agent identity, agent card, trust boundary |
| admin route | `/health`, `/metrics` | restricted access |

For each route, ask:

```text
Who is allowed to call it?
Which backend does it reach?
What is the timeout?
Is it rate limited?
How is it logged?
How is cost attributed?
```

#### 8.4.4 Rate Limit And Cost Control Narrative

Use this example:

```text
Without a gateway, every student app or agent may call the model directly.
With a gateway, we can apply per-route or per-student limits and capture the route, backend, and caller identity.
```

Explain likely controls:

- per-route rate limits
- per-student or per-client quotas
- backend model allowlist
- route-level timeout
- retry rules
- request/response logging
- batch/student/workload headers

#### 8.4.5 Gateway Evidence To Capture

If live gateway logs are available, capture:

| Evidence | Why |
|---|---|
| timestamp | incident sequence |
| route | operational boundary |
| caller/client | ownership |
| backend | model/tool/API reached |
| status code | success/failure |
| latency | performance |
| token/cost fields if available | FinOps |
| trace ID | correlation with app logs |

If unavailable, explicitly mark it as a Day 6 requirement.

#### 8.4.6 Gateway Fallback Script

If the endpoint is not live, say:

```text
The gateway endpoint is not available for a live request right now.
We will still walk through the route, policy, identity, observability, and FinOps design because these are exactly the controls Day 6 Foundation Pod must standardize.
```

### 8.5 AgentGateway Concepts To Mention

AgentGateway can front LLM inference, MCP tool servers, A2A agent traffic, and regular APIs. It supports traffic policies such as rate limits, retries, timeouts, authorization, and observability.

### 8.6 Poll

Ask:

```text
What is the strongest reason to use an agent gateway in enterprise AI?
```

Options:

- central routing
- security policy
- cost control
- observability
- all of the above

Expected answer:

All of the above. The gateway is valuable because it combines them into one runtime boundary.

## 9. Session 5 - Governance And Security Walkthrough

This session is delivered by the Microsoft 365 administrator from a separate tenant. Treat it as an admin walkthrough, not a student hands-on lab.

### 9.1 Goal

Students should understand that AI governance is not a document. It must appear in identity, access, policy, data protection, runtime controls, and audit evidence.

### 9.2 Governance Layers

| Layer | Control |
|---|---|
| Identity | Entra users, groups, managed identities, workload identities |
| Access | RBAC, least privilege, resource scope |
| Data | Purview, DLP, data classification, grounding boundaries |
| Runtime | gateway policies, approval boundaries, sandboxing |
| Agent inventory | Agent 365 registry and lifecycle visibility where available |
| Safety | content filters, evaluations, responsible AI checks |
| Threat protection | Defender and security monitoring |
| Audit | logs, traces, decisions, approvals |

### 9.3 Agent 365 Walkthrough

Show only what the tenant supports.

Suggested flow:

| Step | Exact navigation | What to show | Teaching focus |
|---|---|---|---|
| 1 | Microsoft 365 admin center -> Billing -> Licenses -> Subscriptions | Agent 365 / Microsoft 365 Copilot license readiness if visible | Governance surfaces depend on tenant licensing and roles. |
| 2 | Microsoft 365 admin center -> Agents -> Overview | total agents, activity, insights, risk/governance tasks if visible | Observe starts with inventory and health. |
| 3 | Microsoft 365 admin center -> Agents -> All agents / Agent registry | list of agents, owner, type, publisher, availability/status if visible | Inventory is the first governance control. |
| 4 | Select one agent -> Details pane -> Details tab | description, instructions, publish status, availability, publisher, deployment, agent type, channel, platform, version | Agent metadata supports lifecycle and audit review. |
| 5 | Select one agent -> Details pane -> Users / Availability if available | discover/install/preinstall scope, org-wide or group-specific availability | Agent rollout should be group-based and governed. |
| 6 | Select one agent -> Details pane -> Data & tools tab | capabilities, knowledge sources, tools, external/internal sources | Governance must know what the agent can read and do. |
| 7 | Microsoft 365 admin center -> Agents -> Shadow AI if available | unmanaged agents or preview detection surface | Shadow AI is the risk of agent usage outside IT visibility. |
| 8 | Microsoft 365 admin center -> Agents -> Tools if available | tool or connector management surface | Tools expand capability and risk. |
| 9 | Microsoft 365 admin center -> Agents -> Settings -> User access | All users / No users / Specific users/groups | Tenant-level access control. |
| 10 | Microsoft 365 admin center -> Agents -> Settings -> Sharing | sharing policy options | Controls who can broadly share agents. |
| 11 | Microsoft 365 admin center -> Agents -> Settings -> Agent management rules / Allowed agent types / Security templates if available | rules, allowed categories, policy templates | Central policy makes governance repeatable. |
| 12 | Microsoft Entra admin center -> Users / Groups / Enterprise applications / Managed identities | identities and groups used by agents/apps | Agent access should map to least privilege. |
| 13 | Microsoft Purview portal -> Data Loss Prevention / Information Protection / Audit if available | DLP, labels, audit surfaces | Agent data access must respect data governance. |
| 14 | Microsoft Defender portal -> incidents / alerts / cloud security posture if available | security monitoring and threat signals | Agents introduce runtime misuse and attack paths. |

Trainer note:

```text
Do not position Agent 365 as fully hands-on for students unless tenant licensing and permissions are ready.
```

### 9.4 Detailed Agent 365 / Governance Walkthrough Script

#### 9.4.1 Start With The Governance Problem

Use this framing:

```text
When there are five agents, we can remember them.
When there are hundreds or thousands of agents, we need inventory, ownership, access control, security monitoring, and lifecycle management.
Agent governance is the difference between agent adoption and agent sprawl.
```

#### 9.4.2 Open Microsoft 365 Admin Center

Suggested navigation, depending on tenant readiness:

```text
Microsoft 365 admin center
  -> Copilot
  -> Agents
```

or:

```text
Microsoft 365 admin center
  -> Agents
```

Show any available:

| Surface | Exact menu / tab | What to show | What to say |
|---|---|---|---|
| Overview | Agents -> Overview | agent count, activity, insights, critical tasks | "This is the control-plane mindset for agents." |
| All agents / Agent registry | Agents -> All agents | agent list, owner, type, publisher, status | "Inventory is the first governance control." |
| Agent details | Select agent -> Details tab | description, instructions, status, availability, version | "Metadata supports lifecycle, ownership, and audit." |
| Agent users | Select agent -> Users / availability tab if present | discover/install/preinstall scope and group targeting | "Access should be group-based, not ad hoc." |
| Data & tools | Select agent -> Data & tools tab | capabilities, knowledge sources, tools, MCP/custom actions if shown | "Governance must know what the agent can read and do." |
| Shadow AI | Agents -> Shadow AI | unmanaged agent detection if tenant supports it | "Unmanaged agents are a governance and security risk." |
| Tools | Agents -> Tools | connector/tool management if available | "Tools expand agent capability and risk." |
| Settings - User access | Agents -> Settings -> User access | All users / No users / Specific users/groups | "Tenant-level access policy affects who can use agents." |
| Settings - Sharing | Agents -> Settings -> Sharing | All users / No users / Specific users/groups if visible | "Sharing controls reduce uncontrolled spread." |
| Settings - Rules/templates | Agents -> Settings -> Agent management rules / Allowed agent types / Security templates if visible | rules, allowed categories, templates | "Policy templates make governance repeatable." |
| Billing readiness | Billing -> Licenses -> Subscriptions | Agent 365 / Copilot license availability | "Some features depend on licensing and roles." |

If the tenant only shows partial views, say:

```text
This tenant does not expose every Agent 365 capability for hands-on use. We are using the available admin surfaces to understand the operating model.
```

#### 9.4.3 Explain Observe, Govern, Secure

Use this table live:

| Agent 365 pillar | What students should understand | Example evidence |
|---|---|---|
| Observe | know which agents exist, who owns them, how they are used, and whether they are healthy | registry, overview, activity, agent list |
| Govern | manage lifecycle, access, permissions, compliance, and sharing | settings, groups, access controls |
| Secure | protect agents, users, data, and tool actions | Entra, Purview, Defender, runtime protection discussion |

#### 9.4.4 Connect Agent 365 To Day 1-4

Use examples students already know:

| Prior day artifact | Agent 365 / governance question |
|---|---|
| Day 2 Foundry agent | Who owns this agent and who can use it? |
| Day 2 tools | Which tools can the agent call? |
| Day 3 memory | What data can the agent remember or retrieve? |
| Day 3 workflow | Which actions require approval? |
| Day 4 A2A agent | Can another agent discover or call it? |
| Day 4 AgentGateway | Are runtime calls controlled and logged? |

#### 9.4.5 Entra, Purview, Defender Walkthrough

Do this as a connected governance story. The goal is not to teach each admin portal fully; it is to show where agent controls connect to enterprise controls.

| Tool | Exact navigation | What to open/show | Message |
|---|---|---|---|
| Microsoft Entra | Entra admin center -> Identity -> Users | trainer/student accounts if appropriate | "Agents and agent users must map to tenant identities." |
| Microsoft Entra | Entra admin center -> Identity -> Groups | training groups / access groups if available | "Group-based access is the scalable pattern." |
| Microsoft Entra | Entra admin center -> Applications -> Enterprise applications | app registrations / enterprise apps used by tools or APIs if available | "Agent tools often need app identities." |
| Azure portal / Entra | Resource group -> Managed identities OR Entra -> Managed identities if available | student or workload managed identities | "Runtime model/tool calls should use managed identity where possible." |
| Microsoft Purview | Purview portal -> Data Loss Prevention | DLP policies if available | "Agents need controls for data movement." |
| Microsoft Purview | Purview portal -> Information Protection | labels/classification if available | "Grounding and generated content should respect data sensitivity." |
| Microsoft Purview | Purview portal -> Audit | audit search if available | "Governance needs evidence of user/admin activity." |
| Microsoft Defender | Microsoft Defender portal -> Incidents & alerts | active or sample security alerts if available | "Agent misuse or compromised tools should become security signals." |
| Microsoft Defender for Cloud | Azure portal -> Defender for Cloud | secure score/recommendations for Azure resources | "AI runtime resources still need cloud security posture management." |
| Microsoft Foundry | Foundry -> Agents / Evaluations / Guardrails | agent config, evaluations, content safety/guardrail surfaces | "AI safety and quality checks must connect to enterprise controls." |
| AgentGateway on AKS | AKS -> Workloads / Services and ingresses / Logs | route policy/logging boundary | "Runtime tool/model/API calls need deterministic controls." |

#### 9.4.6 Tool-Call Governance Example

Use this scenario:

```text
An agent can read a customer record and draft an email.
Should it be allowed to send the email automatically?
Should it be allowed to export the customer list?
Should it be allowed to call an external API with customer data?
```

Map it:

| Risk | Control |
|---|---|
| sends email without review | human approval |
| exports data | DLP / policy deny |
| calls external API | gateway allowlist / network policy |
| uses excessive permissions | least-privilege identity |
| cannot prove action | logs, trace, policy decision |

#### 9.4.7 Agent 365 Fallback Script

If the live tenant has limited capability:

```text
The important part for Day 5 is not that every Agent 365 button is enabled in this tenant.
The important part is the operating model: inventory, ownership, access, lifecycle, security, data protection, and audit.
Day 6 Foundation Pod must decide which controls are implemented through tenant services, gateway policy, code, and operational process.
```

### 9.5 Agent Governance Toolkit Positioning

Position it as an engineering reference for deterministic controls around agent actions:

- policy enforcement
- zero-trust identity
- execution sandboxing
- logging and audit evidence
- tool-call governance

Do not turn Day 5 into a toolkit coding lab.

Use it to explain why Lab 07 evaluates observed operations with deterministic policy:

| Toolkit concern | Day 5 handling |
|---|---|
| tool-call governance | Lab 07 policy decision before risky runtime actions |
| identity and least privilege | Entra, managed identity, AgentGateway identity discussion |
| sandboxing and execution boundary | AgentGateway, local runtime, Runpod external runtime comparison |
| logging and audit evidence | Labs 01-04 and Lab 07 telemetry evidence |
| approval boundary | `RequireApproval` decisions for external/neocloud or mutating operations |

### 9.6 Foundry Citadel Platform Positioning

Position Citadel as the architecture lens, not as a Day 5 coding dependency.

Use this framing:

```text
Our ProNative landing zone is a lean training version of a Citadel-style enterprise AI control architecture.
Citadel helps us explain why governance, observability, gateway policy, identity, security, and FinOps must live together.
```

| Citadel-style layer | Day 5 mapping |
|---|---|
| Governance Hub | AgentGateway on AKS, route/policy/rate-limit/cost attribution |
| AI Control Plane | Foundry traces, App Insights, Log Analytics, model usage evidence |
| Agent Identity | Agent 365, Entra, managed identities, agent ownership |
| Security Fabric | Purview, Defender, approval boundaries, policy decisions |
| Hybrid Runtime | Azure-hosted Foundry, Foundry Local CPU, Runpod vLLM |

### 9.7 Poll

Ask:

```text
Which control is stronger for a risky tool call?
```

Options:

- prompt instruction only
- hide the tool name
- policy before execution
- log after execution

Expected answer:

Policy before execution.

## 10. Session 6 - FinOps For AI

### 10.1 Goal

Students should understand AI cost as an engineering and operating concern, not only a billing concern.

### 10.2 Cost Drivers

| Cost driver | Example |
|---|---|
| Tokens | Long prompts, chat history, verbose tool context |
| Model choice | Larger model for every request |
| Retrieval | Search/vector/database cost |
| Always-on compute | App containers, GPU workers, gateway, AKS |
| Observability | Log volume and retention |
| Neocloud/GPU | Runpod workers, cold start, active worker settings |
| Inefficient workflows | retries, loops, repeated calls, poor caching |

### 10.3 Batch Budget Framing

Use:

```text
Batch AN-2607-101 budget ceiling: INR 20,000
```

Explain:

- budget is a control, not a guarantee
- cost alerts can lag
- high-cost resources must be deprovisioned, not only ignored
- tags make cost attribution possible

### 10.4 Walkthrough

Use a live Azure Cost Management walkthrough, then explicitly connect it to AI-specific cost drivers.

| Step | Exact navigation / UI action | What to show | Trainer narration |
|---|---|---|---|
| 1 | Azure portal -> Subscriptions -> select `sub-as-2606-101` or active training subscription -> Cost Management -> Cost analysis | accumulated cost for current period | "Start with total spend, but do not stop here." |
| 2 | Cost analysis -> Scope selector | subscription or resource group scope | "Scope determines what cost story you are telling." |
| 3 | Cost analysis -> View selector / smart views if available | Resources / Resource groups / Services / Subscriptions views | "Different views answer different FinOps questions." |
| 4 | Cost analysis -> Group by -> Resource group name | shared vs student resource group costs | "This separates platform cost from student-specific cost." |
| 5 | Cost analysis -> Group by -> Service name | Azure AI Search, Azure Kubernetes Service, Log Analytics, Cosmos DB, Azure AI services | "This identifies expensive service families." |
| 6 | Cost analysis -> Group by -> Resource | highest-cost individual resources | "This is where we decide what to stop, scale down, or delete." |
| 7 | Cost analysis -> Add filter -> Resource group | filter `rg-ai-shared-platform-an2607101`, `rg-ai-observability-an2607101`, `rg-ai-governance-hub-an2607101`, and a student group | "Filtering helps isolate ownership." |
| 8 | Cost analysis -> Add filter -> Tag | filter/group by `BatchId`, `EnvironmentId`, `StudentId`, `CostCenter`, `DeleteAfter` if cost data supports tags | "Tags must exist before usage; cost tags are not retroactive." |
| 9 | Cost analysis -> Download / Export | CSV/export option if available | "FinOps review should be evidence-driven, not screenshot-driven only." |
| 10 | Cost Management -> Budgets | budget list, create/edit budget, thresholds, recipients/action group | "Budgets alert; they do not replace cleanup." |
| 11 | Azure portal -> Resource groups -> deleted/deprovisioned AI Search discussion | Day 1 Azure AI Search cost lesson | "Hourly-metered services may need deletion between weekends." |
| 12 | Runpod console -> Billing/usage and endpoint settings | external/neocloud usage outside Azure Cost Management | "External GPU cost must be reviewed outside Azure billing." |
| 13 | Foundry / App Insights logs | token/model usage if emitted by app/gateway | "AI FinOps needs token and model evidence, not only Azure resource cost." |

When discussing model usage, use this bridge:

```text
Azure Cost Management tells us which Azure resources are costing money.
It does not automatically explain which prompt, agent, model route, or tool loop caused the spend.
That is why Day 6 needs token/model telemetry and gateway attribution.
```

### 10.5 Detailed FinOps Walkthrough Script

#### 10.5.1 Open Cost Management In This Order

In Azure portal:

```text
Subscriptions
  -> sub-as-2606-101 or active training subscription
  -> Cost Management
  -> Cost analysis
```

Show these views:

| Step | View | Exact UI action | Trainer narration |
|---|---|---|---|
| 1 | Accumulated cost | set date range to current billing period | "This shows total spend trend, but not root cause." |
| 2 | Cost by resource group | Group by -> Resource group name | "This separates shared platform cost from student-specific cost." |
| 3 | Cost by service | Group by -> Service name | "This identifies expensive service families." |
| 4 | Cost by resource | Group by -> Resource | "This is where we identify what to stop or delete." |
| 5 | Cost by tag | Add filter or Group by -> Tag -> `BatchId` / `StudentId` if available | "This is how batch/student/workload attribution should work." |
| 6 | Drill down | select a row or use row action menu / ellipsis if available | "Drill down from group to resource to meter where possible." |
| 7 | Download/export | Download / Export option | "FinOps is evidence-driven; export supports review." |

#### 10.5.2 Use The AN-2607-101 Cost Lens

Use this table during the walkthrough:

| Scope | What to inspect |
|---|---|
| subscription | total cost and forecast |
| `rg-ai-shared-platform-an2607101` | shared Foundry, model, API Center, Cosmos/platform assets, and Foundry-created Application Insights if deployed there |
| `rg-ai-observability-an2607101` | Log Analytics and any observability resources deployed there |
| `rg-ai-governance-hub-an2607101` | AgentGateway and governance services |
| student resource groups | per-student resources and cleanup |
| Runpod | external/neocloud cost outside Azure Cost Management |
| local CPU server | not Azure cost, but still compute/runtime resource |

#### 10.5.3 Budget Walkthrough

Open:

```text
Cost Management
  -> Budgets
```

Explain recommended training budget setup:

| Budget item | Recommended framing |
|---|---|
| Name | `budget-an2607101-training` |
| Scope | subscription or training resource group scope |
| Amount | INR 20,000 |
| Reset period | monthly or batch-specific depending available billing model |
| Alert thresholds | 50%, 80%, 90%, 100% |
| Alert types | actual cost; forecasted cost if useful |
| Recipients | trainer/admin group |
| Action group | optional for automation |

Explain:

```text
A budget is an alerting control. It does not stop all spend automatically.
For high-cost idle services, deletion or scale-down is the real control.
```

#### 10.5.4 AI-Specific Cost Driver Walkthrough

Use the Day 1 Azure AI Search experience as an anchor:

```text
We learned that some resources cost money simply by existing.
AI Search Standard is valuable, but for a four-weekend program it must be deprovisioned when not needed.
```

Then compare:

| Cost driver | What causes it | Control |
|---|---|---|
| model tokens | long prompts, long outputs, repeated history | prompt discipline, summarization, model choice |
| model selection | expensive model for simple task | route by task complexity |
| tool loops | agents retry or loop without termination | max iterations, approval, evaluation |
| RAG service | search/vector/database service cost | choose service carefully; delete hourly-metered services |
| gateway | always-on container or high traffic | scale rules, route limits |
| observability | excessive log ingestion | filter noisy logs, set retention |
| Runpod | active workers, warm GPUs, long jobs | active workers zero, endpoint limits |
| local server | power/maintenance/manual operations | unload model, stop demo service |

#### 10.5.5 Tags And Attribution

Show expected tags:

| Tag | Purpose |
|---|---|
| `BatchId=AN-2607-101` | batch attribution |
| `EnvironmentId=an2607101` | environment attribution |
| `StudentId=ST-2606-xxxx` | student attribution where applicable |
| `CostCenter=Training` | internal cost reporting |
| `DeleteAfter=<date>` | cleanup automation |
| `Owner=<trainer/team>` | operational ownership |

Explain:

```text
If a resource is not tagged, it becomes harder to defend, optimize, or clean up.
```

#### 10.5.6 Deprovisioning Vs Stop Vs Scale Down

Use this decision table:

| Resource | Stop | Scale down | Delete/deprovision | Day 5 guidance |
|---|---|---|---|---|
| Azure AI Search Standard | no meaningful stop for billing | not enough | yes | delete between weekends if not needed |
| AKS AgentGateway workload | no simple stop if cluster remains online | scale deployment replicas where safe | maybe | scale down gateway workload or node pool only when not needed |
| Runpod endpoint | reduce active workers | yes | maybe | keep active workers zero unless demoing |
| Cosmos DB serverless | not usually needed | pay-per-use | maybe after batch | retain if low cost and used Day 6-8 |
| model deployment | no direct stop pattern | quota/capacity consideration | maybe | retain shared deployment if needed |
| Log Analytics | retention/data controls | reduce ingestion | not during batch | control noisy logs |

#### 10.5.7 FinOps Close

Use:

```text
FinOps for AI is not "spend less at any cost."
It is "spend intentionally, with evidence, ownership, and controls."
```

### 10.6 Weekend Control Checklist

Explain what should happen after each weekend:

| Resource type | Action |
|---|---|
| Azure AI Search | Deprovision if not needed; hourly metered service |
| Container apps | Scale down if not needed |
| GPU/neocloud endpoints | Stop or reduce active workers |
| Local models | Unload model |
| Model deployments | Retain only required shared deployments |
| Logs | Keep retention reasonable |
| Student resources | Delete with `DeleteAfter` policy where applicable |

### 10.7 Poll

Ask:

```text
Which cost control is most effective for training batches?
```

Options:

- smaller prompts
- cheaper models
- budgets and alerts
- shut down idle services
- review cost later

Expected answer:

Shut down idle services. Smaller prompts, cheaper models, and budgets help, but expensive idle resources must be stopped, scaled down, or deleted between weekends.

## 11. Session 7 - Hybrid AI Walkthrough

### 11.1 Goal

Students should understand when AI workloads belong in Azure-hosted Foundry, local CPU/server, future DGX Spark/on-prem GPU, or Runpod neocloud.

### 11.2 Positioning

| Runtime | Day 5 status | Why it matters |
|---|---|---|
| Azure Foundry hosted model | Existing managed baseline | Enterprise identity, scale, monitoring, managed model endpoint |
| Foundry Local on CPU | Trainer walkthrough | Local model lifecycle without cloud dependency |
| NVIDIA DGX Spark | Not available in this batch | Future enterprise local/on-prem GPU pattern |
| Runpod | Trainer walkthrough | Neocloud/serverless GPU or worker endpoint pattern |

### 11.3 Foundry Local CPU Walkthrough

Run this as a local runtime walkthrough. Since this batch does not have DGX Spark ready, keep expectations anchored to CPU-friendly small models and focus on operational lifecycle.

Trainer surfaces to use:

| Surface | Exact action | What to show |
|---|---|---|
| Terminal | `foundry --help` or `foundry model --help` if CLI is installed | CLI availability and model command surface |
| Terminal | `foundry model list` | model aliases and available local variants |
| Terminal | `foundry model list --filter alias=qwen*` if supported | how alias filtering helps choose a small model |
| Terminal | `foundry model run qwen2.5-0.5b` | first-run download and interactive local inference |
| Terminal | `foundry service status` | local service endpoint/port if a local service is running |
| C# starter code | `Lab05FoundryLocalCpu` | SDK lifecycle: manager, catalog, download, load, chat, unload |
| Task Manager / system monitor | CPU/memory during local run | local inference still consumes real machine resources |

Explain the lifecycle:

```text
initialize local manager
  -> discover execution providers
  -> download/register runtime support
  -> select small model
  -> download model
  -> load model
  -> run chat completion
  -> unload model
```

Use small model guidance such as `qwen2.5-0.5b` for CPU feasibility.

Package direction from Microsoft quickstart:

```powershell
dotnet add package Microsoft.AI.Foundry.Local.WinML
dotnet add package OpenAI
```

Teaching points:

- local inference is useful for privacy, offline/edge, cost experimentation, and developer workflows
- CPU local models must be small and expectations must be realistic
- GPU hardware changes the throughput/latency equation
- local inference still needs evaluation, safety, telemetry, and versioning

### 11.4 Detailed Foundry Local CPU Walkthrough Script

#### 11.4.1 Start With The Runtime Contrast

Say:

```text
Until now our primary runtime has been Azure-hosted Foundry.
Foundry Local shows another pattern: the model runtime can live on a local machine or local server.
For this alpha batch we are using CPU, not DGX Spark, so we choose a small model and focus on lifecycle.
```

#### 11.4.2 Show The Lifecycle As Operations

Use this table while running or showing prepared output:

| Step | What happens | Operational question |
|---|---|---|
| initialize manager | local runtime manager starts | which app/runtime owns the model lifecycle? |
| discover execution providers | hardware/runtime support is discovered | CPU/GPU/NPU capability? |
| download/register providers | runtime dependencies are prepared | what is cached locally? |
| get catalog | model catalog is queried | which models are allowed? |
| download model | model artifact is cached | where is model stored and how large is it? |
| load model | model is loaded into memory | what memory/latency impact? |
| run chat | local inference happens | what quality/latency tradeoff? |
| unload model | model resources are released | how do we avoid idle resource usage? |

#### 11.4.3 Commands / Code Evidence

Use Microsoft quickstart direction. If showing code, highlight:

```csharp
await FoundryLocalManager.CreateAsync(config, Utils.GetAppLogger());
var mgr = FoundryLocalManager.Instance;
var catalog = await mgr.GetCatalogAsync();
var model = await catalog.GetModelAsync("qwen2.5-0.5b");
await model.DownloadAsync(...);
await model.LoadAsync();
var chatClient = await model.GetChatClientAsync();
await model.UnloadAsync();
```

If using CLI instead of C# during the walkthrough, show:

```powershell
foundry model list
foundry model list --filter alias=qwen*
foundry model run qwen2.5-0.5b
foundry service status
```

Explain the difference:

| Path | What it is best for |
|---|---|
| CLI | quick trainer demo, model list, interactive local run |
| C# SDK | application integration, code review, repeatable project implementation |

Do not spend the session debugging local hardware. If it fails, use prepared output and explain the lifecycle.

#### 11.4.4 What Students Should Observe

Ask students to note:

- model alias
- first-run download behavior
- load time
- response latency
- unload step
- difference from Azure-hosted Foundry
- missing enterprise controls that would need to be added

#### 11.4.5 Foundry Local Close

Use:

```text
Foundry Local changes where inference runs.
It does not remove the need for evaluation, safety, versioning, observability, governance, or FinOps.
```

### 11.5 Runpod Walkthrough

Use Runpod as the neocloud runtime walkthrough. Keep the walkthrough operational: endpoint, workers, logs, scale settings, API key handling, billing/usage, and shutdown discipline.

| Step | Runpod console / UI element | What to show | Teaching focus |
|---|---|---|---|
| 1 | Runpod console -> Serverless | endpoint list | Serverless endpoints are the invocation boundary. |
| 2 | Serverless -> select endpoint | endpoint ID and API URL | Clients call the endpoint, not a model directly. |
| 3 | Endpoint -> Overview / Metrics if available | request count, queue depth, latency, errors | Neocloud still needs runtime observability. |
| 4 | Endpoint -> Workers | active workers, worker status, request history | Workers are the compute containers that process jobs. |
| 5 | Endpoint -> Logs | endpoint logs, worker logs, errors, cold start messages | Logs are required for support and incident review. |
| 6 | Endpoint -> Settings / Edit endpoint | active workers, max workers, idle timeout, execution timeout, GPU type | These settings control cost and latency. |
| 7 | API keys / account settings | location only; never reveal key | Secret handling is part of governance. |
| 8 | Billing / usage | usage/cost view if available | Runpod cost is outside Azure Cost Management. |
| 9 | Endpoint action menu | edit/stop/delete options if available | Weekend shutdown and cost controls must be explicit. |

Teaching points:

- Runpod gives neocloud execution outside Azure
- serverless workers can scale from zero but cold starts matter
- GPU cost must be controlled through endpoint settings and usage discipline
- enterprise use requires security, network, governance, and data-boundary decisions

### 11.6 Detailed Runpod Walkthrough Script

#### 11.6.1 Start With The Neocloud Runtime Model

Say:

```text
Runpod is our neocloud example.
Instead of using an Azure-hosted model or a local CPU model, we send work to an external endpoint backed by workers.
That can be useful for GPU burst and model/runtime flexibility, but it creates governance, secret, data-boundary, and cost questions.
```

#### 11.6.2 Open Runpod Dashboard

Walk through these surfaces:

| Surface | Exact navigation / UI element | What to explain |
|---|---|---|
| account/project | Runpod console account/project selector | where neocloud resources are managed |
| serverless endpoints | Serverless -> endpoint list | endpoint as the invocation boundary |
| endpoint ID/API URL | select endpoint -> endpoint details | what clients call |
| workers | endpoint -> Workers tab | containers that process requests and show request history |
| logs | endpoint -> Logs tab; worker-level logs under Workers if available | where request failures, cold starts, and handler output are diagnosed |
| endpoint settings | endpoint -> Edit endpoint / Settings | active workers, max workers, idle timeout, execution timeout, GPU/runtime options |
| API keys | account/API keys area | show location only; do not reveal key |
| billing/usage | Billing / Usage | where external cost must be reviewed |
| endpoint actions | endpoint menu / settings | where to reduce active workers, update limits, or delete endpoint after the weekend |

#### 11.6.3 Explain Request Flow

Use:

```text
client
  -> Runpod endpoint
  -> request queued if no worker is ready
  -> worker starts or receives request
  -> container/model processes input
  -> response returned or status polled
  -> worker remains warm briefly
  -> idle worker shuts down
```

#### 11.6.4 Show Cost And Cold Start Tradeoff

Explain:

| Setting | Lower cost behavior | Better latency behavior |
|---|---|---|
| active workers | set to zero | keep one or more warm |
| max workers | low cap | higher cap |
| model size | smaller model | larger/better model may cost more |
| container image | optimized image | large image may cold start slower |
| request type | async/polling acceptable | synchronous response desired |

#### 11.6.5 Security And Governance Questions

Ask these aloud:

```text
What data are we sending to Runpod?
Where is the API key stored?
Who can deploy or change the endpoint?
How do we log requests?
How do we cap cost?
How do we evaluate output quality?
How do we shut it down after the weekend?
```

#### 11.6.6 Runpod Close

Use:

```text
Runpod is a useful neocloud option, but it must be treated as an external runtime.
That means API key security, data boundary review, observability, cost controls, and clear ownership are mandatory.
```

### 11.7 Azure Hosted vs Local vs Runpod Decision Matrix

| Need | Prefer |
|---|---|
| Enterprise identity and managed governance | Azure Foundry |
| No cloud dependency / local demo / edge exploration | Foundry Local |
| GPU burst outside Azure | Runpod |
| Dedicated enterprise on-prem GPU appliance | DGX Spark or similar future option |
| Lowest operational friction for students | Shared Azure Foundry deployment |
| Cost-sensitive intermittent GPU experiments | Runpod serverless with strict limits |

### 11.8 Poll

Ask:

```text
When should we not use local or neocloud inference?
```

Options:

- identity is unclear
- data boundary is unclear
- observability is missing
- cost limits are missing
- all of these

Expected answer:

All of these.

## 12. Day 6 Bridge

Day 6 should turn Day 5 walkthroughs into Foundation Pod setup.

### 12.1 Foundation Pod Outcomes

| Foundation Pod area | Day 5 input |
|---|---|
| AI Landing Zone | Resource group, identity, policy, cost, cleanup |
| Platform Engineering | Reusable provisioning, naming, tags, automation |
| LLMOps | Prompt/model/eval/versioning workflow |
| GenAIOps | traces, feedback, monitoring, release checks |
| Observability | App Insights, Log Analytics, dashboards |
| Governance | Agent inventory, access, approval, policy |
| FinOps | budget, token/model/cost dashboard, scale-down checklist |

### 12.2 Trainer Close

Use:

```text
Day 5 is the operating model.
Day 6 is where we begin turning this operating model into foundation assets.
Days 7 and 8 are where application pods must inherit these controls instead of inventing them project by project.
```

## 13. Lab-Aligned Polls

Use these optional checks when running the compact Day 5 labs.

| Lab | Poll question | A | B | C | D | Preferred |
|---|---|---|---|---|---|---|
| Lab 01 Observability Telemetry | Which field best correlates one AI request across app, model, gateway, and cost evidence? | Console color | Trace ID | Machine name | Final answer | B |
| Lab 02 Foundry Operational Trace | Why capture model/deployment, latency, and token usage? | Longer logs | Repro, latency, FinOps | Fewer evaluations | No governance needed | B |
| Lab 03 AgentGateway Client | What should a gateway-mediated AI call carry? | Trace and ownership headers | No metadata | Prompt text only | Response only | A |
| Lab 04 AI FinOps Evidence | Why is Azure Cost Management alone insufficient for AI FinOps? | It has no AI value | It lacks runtime cause | It replaces token logs | It deletes idle services | B |
| Lab 05 Foundry Local CPU | What is the correct operational lesson from local inference? | Controls are removed | Runtime changes; controls remain | CPU always wins | Local has no cost | B |
| Lab 06 Runpod Neocloud Client | What changes when using Runpod as neocloud runtime? | External runtime controls | No security controls | Azure captures all cost | Workers are free when idle | A |
| Lab 07 Governance Policy Check | What should happen before a risky action? | Model decides silently | Policy before execution | Log after action | Disable all tools | B |

Trainer signal:

- Weak Lab 01 / Lab 02 answers mean revisit traces, model metadata, tokens, and operation IDs.
- Weak Lab 03 answers mean revisit route, identity, policy, rate limit, and cost attribution.
- Weak Lab 04 answers mean revisit Azure cost versus token/model/runtime attribution.
- Weak Lab 05 / Lab 06 answers mean revisit hosted versus local versus neocloud controls.
- Weak Lab 07 answers mean revisit deterministic pre-execution governance.

## 14. Trainer Full Reference Library

| Topic | Reference |
|---|---|
| GenAIOps learning path | https://learn.microsoft.com/en-us/training/paths/operationalize-gen-ai-apps/ |
| LLMOps | https://learn.microsoft.com/en-us/ai/playbook/technology-guidance/generative-ai/mlops-in-openai/ |
| LLMOps Workshop | https://microsoft.github.io/llmops-workshop/ |
| Foundry observability | https://learn.microsoft.com/en-us/azure/foundry/concepts/observability |
| Foundry agent tracing overview | https://learn.microsoft.com/en-us/azure/foundry/observability/concepts/trace-agent-concept |
| Foundry tracing setup | https://learn.microsoft.com/en-us/azure/foundry/observability/how-to/trace-agent-setup |
| Foundry agent monitoring dashboard | https://learn.microsoft.com/en-us/azure/foundry/observability/how-to/how-to-monitor-agents-dashboard |
| Foundry evaluation results | https://learn.microsoft.com/en-us/azure/foundry/how-to/evaluate-results |
| Application Insights | https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview |
| Application map | https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-map |
| Log Analytics | https://learn.microsoft.com/en-us/azure/azure-monitor/logs/log-analytics-overview |
| Agent 365 | https://learn.microsoft.com/en-us/microsoft-agent-365/overview |
| Agent 365 admin overview | https://learn.microsoft.com/en-us/microsoft-365/admin/manage/agent-365-overview?view=o365-worldwide |
| Agent 365 agent details | https://learn.microsoft.com/en-us/microsoft-365/admin/manage/agent-details?view=o365-worldwide |
| Agent 365 settings | https://learn.microsoft.com/en-us/microsoft-365/admin/manage/agent-settings?view=o365-worldwide |
| Agent 365 Shadow AI | https://learn.microsoft.com/en-us/microsoft-365/admin/manage/agent-shadow-ai?view=o365-worldwide |
| Agent Governance Toolkit | https://github.com/microsoft/agent-governance-toolkit |
| AgentGateway | https://agentgateway.dev/docs/standalone/latest/ |
| AgentGateway rate limiting | https://agentgateway.dev/docs/local/latest/configuration/resiliency/rate-limits/ |
| Foundry Local quickstart | https://learn.microsoft.com/en-us/azure/foundry-local/get-started?tabs=windows&pivots=programming-language-csharp |
| Foundry Local CLI reference | https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-cli |
| Runpod overview | https://docs.runpod.io/overview |
| Runpod serverless | https://docs.runpod.io/serverless/overview |
| Runpod serverless endpoint overview | https://docs.runpod.io/serverless/endpoints/overview |
| Runpod endpoint settings | https://docs.runpod.io/serverless/endpoints/endpoint-configurations |
| Runpod logs | https://docs.runpod.io/serverless/development/logs |
| Azure Cost Analysis | https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/quick-acm-cost-analysis |
| Azure Cost Analysis views | https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/customize-cost-analysis-views |
| Azure tag-based cost grouping | https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/enable-tag-inheritance |
| Azure Budgets | https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/tutorial-acm-create-budgets |
| Azure FinOps Agent | https://github.com/Azure-Samples/azure-finops-agent |
| FinOps for AI | https://www.finops.org/wg/finops-for-ai-overview/ |

## 15. Trainer Troubleshooting

| Problem | What to say | What to do |
|---|---|---|
| Foundry traces unavailable | "Trace capture needs consistent instrumentation; this is a Day 6 Foundation Pod requirement." | Show Foundry areas and App Insights query shape. |
| AgentGateway endpoint unavailable | "Gateway concept remains valid; live endpoint is unavailable for this dry run." | Use Day 4 output and architecture flow. |
| Agent 365 surface unavailable | "Agent 365 depends on licensing and tenant readiness." | Show overview and admin readiness path. |
| Foundry Local model download slow | "First run downloads runtime/model artifacts." | Show prepared output or quickstart code path. |
| Runpod endpoint unavailable | "Runpod readiness includes endpoint, workers, API key, and cost settings." | Show dashboard/request model. |
| Cost data stale | "Azure cost data can lag." | Show budget/tag/cost-control model. |
