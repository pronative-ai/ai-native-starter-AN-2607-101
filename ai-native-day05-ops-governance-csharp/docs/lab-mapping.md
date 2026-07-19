# Day 5 Lab Mapping

## Purpose

Day 5 labs are operational evidence labs. They are intentionally smaller than Day 3/4 agent framework labs.

Each lab supports a trainer walkthrough:

- generate live telemetry
- inspect traces
- correlate gateway calls
- inspect Azure service cost and AI model usage
- compare runtime options
- apply deterministic governance controls to observed evidence

## Lab Map

| Lab | Topic | Code focus | Trainer platform walkthrough |
|---|---|---|---|
| Lab 01 | LLMOps / GenAIOps observability baseline | live Foundry call, OpenTelemetry ActivitySource, Azure Monitor exporter, AI tags | Application Insights, Log Analytics, KQL |
| Lab 02 | Foundry operational troubleshooting | direct Foundry chat completion with latency, token usage, request IDs, trace fields | Foundry model usage, deployment/auth/throttling evidence |
| Lab 03 | AgentGateway runtime control | gateway-mediated AI/model request with trace and batch/student headers | AgentGateway route, policy, logs, rate limits, cost attribution |
| Lab 04 | AI FinOps evidence | Azure Cost Management query plus Log Analytics model/gateway usage query | Cost Management, model tokens, budget review, weekend controls |
| Lab 05 | Foundry Local CPU | local model lifecycle using Foundry Local WinML | local runtime, model download/load/unload, CPU constraints |
| Lab 06 | Runpod vLLM neocloud | OpenAI-compatible vLLM endpoint invocation | Runpod endpoint, worker lifecycle, cold start, GPU/cost control |
| Lab 07 | Governance evidence and policy | Log Analytics evidence query plus deterministic policy decision | Agent 365, Agent Governance Toolkit, Citadel control-plane mapping |

## Day 6 Bridge

| Day 5 lab | Day 6 Foundation Pod implication |
|---|---|
| Lab 01 | Standard telemetry conventions and App Insights setup |
| Lab 02 | Standard operational envelope for model calls and troubleshooting |
| Lab 03 | Gateway route, policy, and trace correlation baseline |
| Lab 04 | Cost/token evidence and weekend control checklist |
| Lab 05 | Hybrid runtime decision model |
| Lab 06 | Neocloud secret/cost/logging controls |
| Lab 07 | Deterministic policy checks, governance evidence, and Agent 365 readiness |

## Reference Architecture Mapping

| Reference | How Day 5 uses it |
|---|---|
| Foundry Citadel Platform | Architecture lens for Governance Hub, AI Control Plane, Agent Identity, and Security Fabric. |
| Agent Governance Toolkit | Engineering reference for inline policy enforcement, tool-call governance, identity, sandboxing, and audit evidence. |
| Microsoft Agent 365 | Admin/control-plane reference for observing, governing, and securing enterprise agents where tenant readiness allows. |
