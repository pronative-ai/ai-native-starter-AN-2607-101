# Environment Dry Run Checklist - Day 3 and Day 4

Program: ProNative AI-Native Fullstack Engineering  
Batch: `AN-2607-101`  
Purpose: verify the local machine, Azure access, shared platform resources, starter repositories, and live-lab dependencies before Day 3 and Day 4 delivery.

This file is intentionally lean. It links to the trainer playbook for commands and detailed walkthroughs so the same command is not maintained in multiple places.

## 1. Source Of Commands

| Need | Use this source |
|---|---|
| Common setup commands | [Trainer Playbook - Common Setup Commands](trainer-playbook-day03-day04.md#4-common-setup-commands) |
| Build commands | [Trainer Playbook - Build Dry Run](trainer-playbook-day03-day04.md#5-build-dry-run) |
| Day 3 lab run commands | [Trainer Playbook - Day 3 Detailed Trainer Guide](trainer-playbook-day03-day04.md#7-day-3-detailed-trainer-guide) |
| Day 4 lab run commands | [Trainer Playbook - Day 4 Detailed Trainer Guide](trainer-playbook-day03-day04.md#9-day-4-detailed-trainer-guide) |
| Student-facing flow | [Student Playbook](student-playbook-day03-day04.md) |

## 2. Dry Run Scope

Dry run must confirm:

- local .NET and Azure CLI readiness
- correct Azure tenant/subscription login
- Foundry project endpoint and model deployment readiness
- Cosmos DB readiness for Day 3 Lab 04 and Lab 06
- Azure API Center metadata readiness for skill/A2A catalog discussion
- A2A provider/consumer readiness for Day 4 Lab 03
- AgentGateway dry-run readiness and live endpoint readiness if live route will be shown
- observability access for Azure Container Apps logs, Log Analytics, and Application Insights

## 3. Local Machine Preflight

| Check | Expected result | Status |
|---|---|---|
| Windows PowerShell is used | Trainer can run PowerShell from repo root | [ ] |
| .NET SDK | `10.x` | [ ] |
| Azure CLI | `az account show` succeeds | [ ] |
| NuGet restore/build | Day 3 and Day 4 projects restore/build | [ ] |
| Network | Foundry, Cosmos DB, NuGet, and gateway endpoints are reachable | [ ] |
| VS Code | Optional, useful for code walkthrough | [ ] |

Command source: [Common Setup Commands](trainer-playbook-day03-day04.md#4-common-setup-commands)

## 4. Environment Variables

| Variable | Expected value or purpose | Status |
|---|---|---|
| `BATCH_ID` | `AN-2607-101` | [ ] |
| `STUDENT_ID` | Dry-run student, usually `ST-2606-1000` | [ ] |
| `AZURE_AI_PROJECT_ENDPOINT` | Project-scoped Foundry endpoint, not raw OpenAI endpoint | [ ] |
| `AZURE_OPENAI_ENDPOINT` | Azure OpenAI compatible endpoint root | [ ] |
| `AZURE_OPENAI_CHAT_DEPLOYMENT` | `gpt-5-mini` | [ ] |
| `COSMOS_ENDPOINT` | `https://cosmos-an2607101.documents.azure.com:443/` | [ ] |
| `COSMOS_DATABASE` | `db-an2607101-training` | [ ] |
| `COSMOS_CONTAINER` | `training-knowledge` | [ ] |
| `COSMOS_SESSION_CONTAINER` | `agent-session-checkpoints` | [ ] |
| `A2A_BASE_URL` | `http://localhost:5063` for local provider | [ ] |
| `PN_AGENTGATEWAY_ENDPOINT` | `https://agentgateway-an2607101.azurecontainerapps.io` | [ ] |
| `PN_AGENTGATEWAY_LIVE` | `false` for dry run, `true` only for live gateway demo | [ ] |

Command source: [Common Setup Commands](trainer-playbook-day03-day04.md#4-common-setup-commands)

## 5. Azure Resource Readiness

### 5.1 Shared Platform

| Resource | Expected state | Status |
|---|---|---|
| `rg-ai-shared-platform-an2607101` | Exists | [ ] |
| Foundry project | `proj-an2607101-default` available | [ ] |
| Model deployment | `gpt-5-mini` callable | [ ] |
| Cosmos DB | `cosmos-an2607101` available | [ ] |
| Cosmos database | `db-an2607101-training` exists | [ ] |
| Cosmos knowledge container | `training-knowledge`, partition key `/batchId` | [ ] |
| Cosmos Lab 04 checkpoint container | `agent-session-checkpoints`, partition key `/batchId` | [ ] |
| Azure API Center | `apic-an2607101-fec2ed` available | [ ] |

### 5.2 Observability

| Resource | Expected state | Status |
|---|---|---|
| `rg-ai-observability-an2607101` | Exists | [ ] |
| Log Analytics | Query access works | [ ] |
| Application Insights | Access works where used | [ ] |
| Azure Container Apps logs | Accessible for gateway if live route is used | [ ] |

### 5.3 Governance And Gateway

| Resource | Expected state | Status |
|---|---|---|
| `rg-ai-governance-hub-an2607101` | Exists | [ ] |
| AgentGateway endpoint | Reachable if live demo is planned | [ ] |
| Gateway routes | Model, MCP-shaped, and A2A-shaped route config reviewed | [ ] |
| Gateway logs | Logs visible in Azure Monitor or Container Apps logs | [ ] |

## 6. RBAC And Access

| Principal | Scope | Required access | Status |
|---|---|---|---|
| Trainer | Shared platform RG | Contributor or equivalent setup access | [ ] |
| Trainer/student | Foundry project/resource | Azure AI Developer plus Cognitive Services OpenAI User or equivalent | [ ] |
| Trainer/student | Azure OpenAI endpoint | Cognitive Services OpenAI User or equivalent | [ ] |
| Trainer | Cosmos DB | Cosmos DB Built-in Data Contributor for seed/setup | [ ] |
| Student/test identity | Cosmos DB | Reader or contributor depending on Lab 04 checkpoint path | [ ] |
| Trainer/platform owner | Azure API Center | Permission to register/update catalog metadata | [ ] |
| AgentGateway managed identity | Foundry/Azure AI backend | Backend model-call permission if live route is used | [ ] |

## 7. Build Dry Run

Use the build commands in [Trainer Playbook - Build Dry Run](trainer-playbook-day03-day04.md#5-build-dry-run).

| Area | Expected result | Status |
|---|---|---|
| Day 3 all projects | Build succeeds | [ ] |
| Day 4 all projects | Build succeeds | [ ] |
| Package restore | No missing package failures | [ ] |
| SDK compatibility | No .NET SDK mismatch | [ ] |

## 8. Day 3 Lab Dry Run

| Lab | Run command source | Required evidence | Status |
|---|---|---|---|
| Lab 01 - Agentic AI Reasoning Loop | [Trainer Lab 01](trainer-playbook-day03-day04.md#lab-01---agentic-ai-reasoning-loop) | Foundry call succeeds, tool calls visible, middleware visible, session checkpoint visible | [ ] |
| Lab 02 - Flow Engineering | [Trainer Lab 02](trainer-playbook-day03-day04.md#lab-02---flow-engineering) | Workflow events, branch, approval pause, typed final output | [ ] |
| Lab 03 - Skill-Driven Development | [Trainer Lab 03](trainer-playbook-day03-day04.md#lab-03---skill-driven-development) | File, inline, class-based skills load; optional API Center/MCP path understood | [ ] |
| Lab 04 - Conversations, State, and Memory | [Trainer Lab 04](trainer-playbook-day03-day04.md#lab-04---conversations-state-and-memory) | Local session restore works; optional Cosmos checkpoint upsert/read works if enabled | [ ] |
| Lab 05 - Harness Engineering | [Trainer Lab 05](trainer-playbook-day03-day04.md#lab-05---harness-engineering) | Official harness path compiles/runs; evidence output visible | [ ] |
| Lab 06 - Retrieval-Grounded RAG Workflow | [Trainer Lab 06](trainer-playbook-day03-day04.md#lab-06---retrieval-grounded-rag-for-agentic-workflow) | Cosmos records seeded, retrieval returns records, answer cites docs, verifier runs | [ ] |
| Lab 07 - Workflow Agent | [Trainer Lab 07](trainer-playbook-day03-day04.md#lab-07---workflow-agent) | Specialist outputs, streaming events, structured result | [ ] |

## 9. Day 4 Lab Dry Run

| Lab | Run command source | Required evidence | Status |
|---|---|---|---|
| Lab 01 - Multi-Agent Architecture | [Trainer Day 4 Lab 01](trainer-playbook-day03-day04.md#lab-01---multi-agent-architecture) | Sequential, concurrent, handoff, and group-chat patterns visible | [ ] |
| Lab 02 - Magentic-Style Orchestration | [Trainer Day 4 Lab 02](trainer-playbook-day03-day04.md#lab-02---magentic-style-coordinator-worker-orchestration) | Coordinator and workers visible; rounds/stalls/resets explainable | [ ] |
| Lab 03 - A2A Exposure, Discovery, and Consumption | [Trainer Day 4 Lab 03](trainer-playbook-day03-day04.md#lab-03---a2a-exposure-discovery-and-consumption) | Provider starts, Agent Card resolves, consumer sends message | [ ] |
| Lab 04 - AG-UI / A2UI Boundary | [Trainer Day 4 Lab 04](trainer-playbook-day03-day04.md#lab-04---ag-ui--a2ui-agent-user-interaction-boundary) | Events, interrupt/resume, approval payload, A2UI comparison visible | [ ] |
| Lab 05 - MCP vs UTCP Boundary | [Trainer Day 4 Lab 05](trainer-playbook-day03-day04.md#lab-05---mcp-vs-utcp-tool-boundary) | MCP metadata and UTCP request shape visible | [ ] |
| Lab 06 - AgentGateway Baseline | [Trainer Day 4 Lab 06](trainer-playbook-day03-day04.md#lab-06---agentgateway-baseline) | Dry-run request shapes visible; live gateway optional | [ ] |
| Lab 07 - Gateway Observability and Control | [Trainer Day 4 Lab 07](trainer-playbook-day03-day04.md#lab-07---gateway-observability-and-control) | Correlation IDs, traceparent, logs/rate-limit evidence reviewed | [ ] |
| Optional - Multi-Agent Evaluation | [Trainer Optional Extension](trainer-playbook-day03-day04.md#optional-extension---multi-agent-evaluation) | Optional evaluation dimensions discussed if time permits | [ ] |

## 10. Observability Dry Run

| Check | Expected result | Status |
|---|---|---|
| Foundry run visibility | Trainer can inspect model/agent activity where available | [ ] |
| Application logs | Local console logs show middleware/workflow/gateway signals | [ ] |
| Gateway correlation | `x-request-id`, `x-correlation-id`, and `traceparent` are visible in gateway labs | [ ] |
| Azure Monitor | Trainer can find relevant gateway/container logs if live gateway is used | [ ] |
| Rate-limit evidence | Dry-run or live route demonstrates rate-limit behavior/config | [ ] |

## 11. Go / No-Go Decision

### Green

Proceed when:

- all Day 3 and Day 4 projects build
- Foundry project/model access works
- Cosmos DB seed and retrieval are ready
- A2A provider/consumer are ready
- AgentGateway dry-run is ready
- live gateway route is either ready or explicitly marked optional

### Amber

Proceed with trainer-led fallback when:

- optional API Center/MCP path is not ready
- live AgentGateway is not ready but dry-run route is ready
- Cosmos checkpoint persistence is not ready but local Lab 04 session path works

### Red

Pause or re-scope when:

- Foundry project/model calls fail
- Day 3/4 projects do not build
- Cosmos DB retrieval cannot be seeded or queried for Lab 06
- A2A provider/consumer cannot run for Day 4 Lab 03

## 12. Final Pre-Class Snapshot

| Item | Value |
|---|---|
| Dry-run date/time |  |
| Trainer identity |  |
| Azure subscription/tenant verified |  |
| Day 3 build status |  |
| Day 4 build status |  |
| Cosmos seed status |  |
| A2A status |  |
| AgentGateway dry-run status |  |
| AgentGateway live status |  |
| Known risks |  |
| Final decision | Green / Amber / Red |

