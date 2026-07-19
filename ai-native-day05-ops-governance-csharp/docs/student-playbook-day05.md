# Student Playbook - Day 5

Program: ProNative AI-Native Fullstack Engineering  
Day: Day 5  
Batch: `AN-2607-101`  
Theme: AI Operations, Governance, FinOps, and Hybrid AI walkthrough  
Primary stack: Microsoft Foundry, Azure Monitor, Application Insights, Log Analytics, AgentGateway, Agent 365, Entra, Purview, Defender, Azure Cost Management, Foundry Local on CPU, Runpod

This playbook is your live-session guide. Keep it open while the trainer walks through platform operations and governance.

## 1. What Day 5 Is About

The first four days focused on building AI-native capability:

```text
Generative AI app
  -> Foundry agent
  -> workflow agent
  -> skills and memory
  -> multi-agent system
  -> protocols and gateway
```

Day 5 asks:

```text
How do we operate, govern, secure, observe, evaluate, and control cost for these systems?
```

Day 5 is not a heavy coding day. It is a trainer-led walkthrough of real operating concerns.

## 2. What You Should Be Able To Explain By End Of Day

You should be able to explain:

- why AI-native systems need more than normal application logs
- how LLMOps and GenAIOps connect to prompts, agents, evaluations, traces, and releases
- what operational signals matter for AI apps, agents, workflows, and multi-agent systems
- where AgentGateway fits for routing, policy, rate limiting, observability, and cost attribution
- why governance includes identity, access, data protection, runtime controls, audit evidence, and Agent 365 readiness
- how FinOps applies to tokens, model choice, always-on resources, GPU/neocloud usage, and weekend cleanup
- how Azure-hosted Foundry, Foundry Local on CPU, and Runpod fit different deployment needs

## 3. Day 5 Flow

| Session | What you will observe |
|---|---|
| Opening | How Day 1-4 systems create operational responsibility |
| AI Operations | Request path across app, agent, model, tool, memory, gateway, logs, cost |
| LLMOps / GenAIOps | Prompt, evaluation, automation, monitoring, tracing, feedback loop |
| Observability | Foundry, Application Insights, Log Analytics, traces, latency, errors |
| AgentGateway | Routing, policy, rate limit, backend identity, trace/cost attribution |
| Governance | Responsible AI, Agent 365, Entra, Purview, Defender, approval boundaries |
| FinOps | Budget, cost views, tags, token/model cost, deprovisioning |
| Hybrid AI | Foundry Local on CPU and Runpod neocloud walkthrough |
| Day 6 bridge | How Foundation Pod turns this into reusable platform capability |

## 3.1 Code Labs

Day 5 includes compact code labs. These are not large application builds. They create operational evidence for the trainer walkthrough.

Repo:

```powershell
cd C:\Users\Madan\Documents\Codex\2026-06-29\we-are-now-focusing-on-pronative-2\outputs\starter-repositories\ai-native-day05-ops-governance-csharp
```

Use the labs when the trainer asks. Do not run them all at the beginning of the day.

| Session | Lab | Command |
|---|---|---|
| Session 1 - AI Operations | Lab 02 Foundry Operational Trace | `dotnet run --project .\src\Lab02FoundryOperationalTrace\Lab02FoundryOperationalTrace.csproj` |
| Session 2 - LLMOps / GenAIOps | Reference Lab 01 / Lab 02 evidence | No separate run required unless trainer asks. |
| Session 3 - Observability | Lab 01 Observability Telemetry | `dotnet run --project .\src\Lab01ObservabilityTelemetry\Lab01ObservabilityTelemetry.csproj` |
| Session 4 - AgentGateway | Lab 03 AgentGateway Operational Client | `dotnet run --project .\src\Lab03AgentGatewayOperationalClient\Lab03AgentGatewayOperationalClient.csproj` |
| Session 5 - Governance | M365 admin walkthrough from a separate tenant | No student lab run. Lab 07 is previewed conceptually only. |
| Session 6 - FinOps | Lab 04 AI FinOps Evidence | `dotnet run --project .\src\Lab04AiFinOpsEvidence\Lab04AiFinOpsEvidence.csproj` |
| Session 7 - Hybrid AI | Lab 05 Foundry Local CPU | `dotnet run --project .\src\Lab05FoundryLocalCpu\Lab05FoundryLocalCpu.csproj` |
| Session 7 - Hybrid AI | Lab 06 Runpod vLLM Neocloud Client | `dotnet run --project .\src\Lab06RunpodNeocloudClient\Lab06RunpodNeocloudClient.csproj` |
| Day 5 close / optional | Lab 07 Governance Evidence and Policy Check | `dotnet run --project .\src\Lab07GovernancePolicyCheck\Lab07GovernancePolicyCheck.csproj` |

Some labs need trainer-provided secrets or platform readiness. If a lab exits because a live dependency is missing, treat that as an environment readiness finding, not as lab completion.

Important: Lab 07 depends on live telemetry from earlier labs. Run it only near the end of Day 5, or when the trainer confirms that enough telemetry exists.

## 4. AI Operations

### 4.1 Before The Walkthrough, Understand This

An AI-native system is not just:

```text
prompt -> model -> response
```

It is closer to:

```text
request
  -> app or agent
  -> instructions
  -> model
  -> tools
  -> memory or retrieval
  -> gateway or policy
  -> telemetry
  -> evaluation
  -> cost attribution
  -> governance
```

### 4.2 Watch For

| Signal | Why it matters |
|---|---|
| model deployment | tells us what actually answered |
| prompt/instruction version | helps explain behavior changes |
| retrieved context | proves whether answer was grounded |
| tool calls | shows external action and risk |
| latency | affects user experience |
| token usage | affects cost |
| errors | reveal reliability issues |
| trace/session ID | helps reconstruct what happened |

### 4.3 You Are Complete When

You can describe the operational path of one AI request from user input to model/tool execution and telemetry.

## 5. LLMOps And GenAIOps

### 5.1 Before The Walkthrough, Understand This

LLMOps manages the lifecycle of LLM-based applications. GenAIOps adapts operations to generative AI apps and agents.

The useful mental model is:

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

### 5.2 Watch For

- where prompts and instructions are stored
- how evaluation data is created
- how evaluation results can become a release gate
- how traces help debug complex workflows
- how feedback can become future test data

### 5.3 You Are Complete When

You can explain how Day 3 harness and evaluation ideas become Day 5 operations and Day 6 Foundation Pod capabilities.

Reference:

- https://learn.microsoft.com/en-us/training/paths/operationalize-gen-ai-apps/
- https://learn.microsoft.com/en-us/ai/playbook/technology-guidance/generative-ai/mlops-in-openai/

## 6. Observability

### 6.1 Before The Walkthrough, Understand This

Observability answers:

```text
What happened?
Where did it happen?
Why did it happen?
How expensive was it?
Can we prove it?
```

For AI-native systems, observability must include the model and agent context, not only application errors.

### 6.2 Watch For

- Foundry monitoring and traces
- Application Insights requests, traces, and exceptions
- Log Analytics queries
- gateway logs where available
- missing telemetry that must be fixed in Foundation Pod

### 6.3 Useful Query Shapes

The trainer may show queries like:

```kusto
requests
| where timestamp > ago(24h)
| summarize RequestCount=count(), AvgDurationMs=avg(duration) by cloud_RoleName
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

### 6.4 You Are Complete When

You can name the minimum telemetry needed to debug a bad agent answer.

## 7. AgentGateway Operations

### 7.1 Before The Walkthrough, Understand This

AgentGateway is the runtime boundary between agents/apps and downstream models, tools, APIs, MCP servers, and A2A agents.

It helps centralize:

- routing
- authentication
- authorization
- rate limiting
- retries and timeouts
- traffic policy
- logs, metrics, and traces
- cost and workload attribution

### 7.2 Watch For

| Gateway concern | What it means |
|---|---|
| route | where the request goes |
| backend | which model/tool/API receives the request |
| policy | what is allowed or denied |
| rate limit | how runaway usage is controlled |
| identity | how backend access is secured |
| logs/traces | how operators debug and audit |
| tags | how cost and ownership are attributed |

### 7.3 You Are Complete When

You can explain why enterprise agents should not directly call every model, tool, and service without a control boundary.

Reference:

- https://agentgateway.dev/docs/standalone/latest/

## 8. Governance And Security

### 8.1 Before The Walkthrough, Understand This

AI governance is not only policy documentation. It must appear in the runtime and platform.

| Governance area | Example |
|---|---|
| Identity | Entra users, groups, managed identities |
| Access | RBAC, least privilege |
| Data | Purview, DLP, classification |
| Runtime | gateway policy, approvals, sandboxing |
| Agent inventory | Agent 365 where available |
| Safety | content filters, evaluations |
| Threat protection | Defender |
| Audit | logs, traces, approvals |

### 8.2 Agent 365

Agent 365 is positioned as Microsoft's enterprise control plane for observing, governing, and securing agents where tenant licensing and prerequisites are available.

In this session, treat Agent 365 as a trainer/admin walkthrough unless the trainer explicitly says students have hands-on access.

### 8.3 Citadel And Agent Governance Toolkit

Two references help you connect the walkthroughs:

| Reference | What it helps you understand |
|---|---|
| Foundry Citadel Platform | The enterprise architecture pattern: governance hub, AI control plane, identity, security, observability, and FinOps. |
| Agent Governance Toolkit | Runtime policy thinking: allowed actions, approval, sandboxing, audit, and tool-call governance. |

Day 5 does not require you to code directly against these repositories. Use them as architecture and engineering references for Day 6-8 live projects.

### 8.4 You Are Complete When

You can explain why "the model was instructed to be safe" is not enough for enterprise governance.

References:

- https://learn.microsoft.com/en-us/microsoft-agent-365/overview
- https://github.com/Azure-Samples/foundry-citadel-platform
- https://github.com/microsoft/agent-governance-toolkit

## 9. FinOps For AI

### 9.1 Before The Walkthrough, Understand This

AI cost is shaped by engineering choices.

| Choice | Cost impact |
|---|---|
| long prompt | more input tokens |
| verbose answer | more output tokens |
| large model | higher per-call cost |
| repeated retries | more model/tool calls |
| always-on compute | cost even when idle |
| Azure AI Search | hourly metered service |
| GPU/neocloud worker | cost while active or configured |
| excessive logs | monitoring/storage cost |

### 9.2 Batch Budget

The Day 5 FinOps walkthrough uses:

```text
Batch budget framing: INR 20,000
```

This budget is a control target. It does not replace engineering discipline.

### 9.3 Watch For

- cost by resource group
- cost by service
- budget and alert concepts
- tags such as `BatchId`, `StudentId`, `CostCenter`, `DeleteAfter`
- weekend shutdown and deprovisioning decisions

### 9.4 You Are Complete When

You can explain why deprovisioning high-cost idle resources is stronger than only setting a budget alert.

References:

- https://www.finops.org/wg/finops-for-ai-overview/
- https://github.com/Azure-Samples/azure-finops-agent

## 10. Hybrid AI Walkthrough

### 10.1 Before The Walkthrough, Understand This

Hybrid AI means the model/runtime does not always have to be one Azure-hosted endpoint.

For this batch:

| Runtime | Status |
|---|---|
| Azure Foundry hosted model | Main managed training baseline |
| Foundry Local on CPU | Trainer walkthrough |
| Runpod neocloud | Trainer walkthrough |
| NVIDIA DGX Spark | Future enterprise option, not available in this batch |

### 10.2 Foundry Local On CPU

Foundry Local demonstrates local model lifecycle:

```text
discover runtime support
  -> download model
  -> load model
  -> run chat
  -> unload model
```

The trainer may use a small model such as `qwen2.5-0.5b` because CPU-based inference should stay lightweight.

Reference:

- https://learn.microsoft.com/en-us/azure/foundry-local/get-started?tabs=windows&pivots=programming-language-csharp

### 10.3 Runpod Neocloud

Runpod demonstrates external serverless/GPU endpoint thinking:

```text
request
  -> Runpod endpoint
  -> worker/container
  -> model/runtime
  -> result
```

Watch for:

- API key handling
- endpoint configuration
- worker lifecycle
- cold start
- logs
- cost controls

References:

- https://docs.runpod.io/overview
- https://docs.runpod.io/serverless/overview

### 10.4 You Are Complete When

You can explain when to prefer:

- Azure-hosted Foundry
- Foundry Local on CPU
- Runpod neocloud
- future on-prem GPU hardware such as DGX Spark

## 11. Day 6 Bridge

Day 5 prepares Day 6 Foundation Pod.

| Day 5 topic | Day 6 implication |
|---|---|
| LLMOps | version prompts/instructions and evaluation assets |
| GenAIOps | create feedback, monitoring, tracing, and release checks |
| Observability | standard App Insights and Log Analytics setup |
| AgentGateway | route, policy, rate limit, and attribution baseline |
| Governance | identity, access, approval, and agent inventory readiness |
| FinOps | budget, tags, dashboards, shutdown/deprovisioning |
| Hybrid AI | runtime decision model for Azure, local, and neocloud |

## 12. Personal Notes Template

Use this during the walkthrough:

```text
Most important operational signal I learned:

Most important governance control I learned:

Most important cost control I learned:

One Day 6 Foundation Pod item that must not be missed:

One question I still have:
```

## 13. Curated References

| Topic | Reference |
|---|---|
| GenAIOps | https://learn.microsoft.com/en-us/training/paths/operationalize-gen-ai-apps/ |
| LLMOps | https://learn.microsoft.com/en-us/ai/playbook/technology-guidance/generative-ai/mlops-in-openai/ |
| Agent 365 | https://learn.microsoft.com/en-us/microsoft-agent-365/overview |
| AgentGateway | https://agentgateway.dev/docs/standalone/latest/ |
| Foundry Local | https://learn.microsoft.com/en-us/azure/foundry-local/get-started?tabs=windows&pivots=programming-language-csharp |
| Runpod | https://docs.runpod.io/overview |
| Runpod Serverless | https://docs.runpod.io/serverless/overview |
| FinOps for AI | https://www.finops.org/wg/finops-for-ai-overview/ |
