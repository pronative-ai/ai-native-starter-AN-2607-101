# Environment Readiness Dry Run - Day 5

Program: ProNative AI-Native Fullstack Engineering  
Day: Day 5  
Batch: `AN-2607-101`  
Theme: AI Operations, Governance, FinOps, and Hybrid AI walkthrough  
Primary stack: Microsoft Foundry, Azure Monitor, Application Insights, Log Analytics, AgentGateway, Agent 365, Entra, Purview, Defender, Azure Cost Management, Foundry Local on CPU, Runpod

## 1. Purpose

Day 5 is a trainer-led operations walkthrough day. It is not a coding-heavy day and it is not a live-project build day.

The dry run confirms that the trainer can show:

- how Day 1-4 AI-native workloads are observed and operated
- how GenAIOps and LLMOps map to prompts, agents, evaluations, traces, deployments, and feedback
- how AgentGateway supports routing, policy, observability, rate limits, and attribution
- how governance surfaces such as Agent 365, Entra, Purview, Defender, and content safety fit the enterprise control model
- how FinOps is applied to model/token usage, Azure costs, budgets, and weekend shutdown
- how hybrid AI can be demonstrated using Foundry Local on a CPU-based local server and Runpod as the neocloud option

## 2. Day 5 Delivery Mode

| Area | Mode |
|---|---|
| AI Operations | Trainer walkthrough |
| LLMOps / GenAIOps | Trainer walkthrough, mapped to Day 6 Foundation Pod |
| Observability | Trainer walkthrough using Foundry, App Insights, Log Analytics |
| AgentGateway | Trainer walkthrough using configured gateway or fallback architecture view |
| Governance | Trainer walkthrough using available tenant/admin surfaces |
| FinOps | Trainer walkthrough using Azure Cost Management and batch budget framing |
| Hybrid AI | Trainer walkthrough using Foundry Local on CPU and Runpod |
| Student activity | Observe, run compact operational labs where appropriate, answer checks, document operating model |

## 2.1 Day 5 Code Lab Pack Readiness

Starter repo:

```text
outputs/starter-repositories/ai-native-day05-ops-governance-csharp
```

Dry-run build:

```powershell
cd C:\Users\Madan\Documents\Codex\2026-06-29\we-are-now-focusing-on-pronative-2\outputs\starter-repositories\ai-native-day05-ops-governance-csharp
dotnet build .\ai-native-day05-ops-governance-csharp.slnx --nologo
```

Expected:

```text
Build succeeded.
```

Lab pack:

| Lab | Dry-run requirement |
|---|---|
| Lab 02 Foundry Operational Trace | Requires `AZURE_OPENAI_API_KEY` or `AZURE_OPENAI_BEARER_TOKEN`. |
| Lab 01 Observability Telemetry | Requires Foundry auth for live call; App Insights export requires `APPLICATIONINSIGHTS_CONNECTION_STRING`. |
| Lab 03 AgentGateway Operational Client | Requires reachable `AGENTGATEWAY_ENDPOINT` and configured AI/model route, not `/health`. |
| Lab 04 AI FinOps Evidence | Requires `az login`, subscription Cost Management access, and Log Analytics workspace access. |
| Lab 05 Foundry Local CPU | Windows-specific `net8.0-windows10.0.18362`; first run can download EP/model assets. |
| Lab 06 Runpod vLLM Neocloud Client | Requires `RUNPOD_ENDPOINT_ID`, `RUNPOD_API_KEY`, and `RUNPOD_MODEL`. |
| Lab 07 Governance Evidence and Policy Check | Requires `LOG_ANALYTICS_WORKSPACE_ID` and live telemetry from earlier labs. |

Recommended dry-run order:

| Order | Session | Lab |
|---:|---|---|
| 1 | Session 1 - AI Operations | Lab 02 Foundry Operational Trace |
| 2 | Session 3 - Observability | Lab 01 Observability Telemetry |
| 3 | Session 4 - AgentGateway | Lab 03 AgentGateway Operational Client |
| 4 | Session 6 - FinOps | Lab 04 AI FinOps Evidence |
| 5 | Session 7 - Hybrid AI | Lab 05 Foundry Local CPU |
| 6 | Session 7 - Hybrid AI | Lab 06 Runpod Neocloud Client |
| 7 | Day 5 close / optional | Lab 07 Governance Evidence and Policy Check |

Do not use Lab 07 as the first governance demo. It is best run after Labs 01-06 have generated live telemetry. Session 5 itself is an M365-admin-led walkthrough from a separate tenant.

## 3. Batch Baseline

| Item | Value |
|---|---|
| Batch ID | `AN-2607-101` |
| Region | `Central India` unless service requires otherwise |
| Batch budget framing | INR 20,000 |
| Foundry project endpoint | `https://proj-an2607101-default-resource.services.ai.azure.com/api/projects/proj-an2607101-default` |
| Azure OpenAI endpoint | `https://proj-an2607101-default-resource.openai.azure.com/` |
| Model deployment | `gpt-5-mini` |
| API Center | `apic-an2607101-fec2ed` |
| API Center runtime URL | `https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms` |
| AgentGateway endpoint | `https://aks-an2607101-agw-dns-xywyud9k.hcp.centralindia.azmk8s.io` |
| Cosmos DB endpoint | `https://cosmos-an2607101.documents.azure.com:443/` |

## 4. Resource Group Readiness

| Resource group | Expected role on Day 5 | Dry-run check |
|---|---|---|
| `rg-ai-shared-platform-an2607101` | Foundry, model deployment, Cosmos DB, API Center, shared platform assets, and possibly Foundry-created Application Insights | Confirm Foundry project, model deployment, API Center, Cosmos DB, and the `proj-an2607101-default-resource-appinsights` resource if it is deployed here. |
| `rg-ai-observability-an2607101` | Log Analytics and/or Application Insights depending current deployment | Confirm trainer can open Log Analytics and any observability resources placed here. |
| `rg-ai-governance-hub-an2607101` | AgentGateway and governance/control-plane services | Confirm AgentGateway service, logs, and configuration are reachable. |
| Student-specific resource groups | Student attribution and later live-project isolation | Confirm no unexpected high-cost resources are running before the weekend. |

## 5. Trainer Device Readiness

Run these from Windows PowerShell.

```powershell
dotnet --version
az version
az account show
```

Expected:

- `.NET SDK 10.x` for the program baseline
- Azure CLI installed and logged in
- Trainer account has access to the batch subscription and relevant resource groups

Foundry Local uses .NET 8 or later. The program baseline of .NET 10 satisfies this requirement.

## 6. Azure Portal And Foundry Readiness

### 6.1 Foundry Project

Open the Foundry project and confirm:

| Check | Expected |
|---|---|
| Project opens | `proj-an2607101-default` is accessible |
| Model deployment visible | `gpt-5-mini` is visible |
| Agents area visible | Day 2 agents or trainer-created agents are visible if retained |
| Monitor / Traces / Evaluation areas visible | Trainer can show where operational signals appear |
| Content filters / safety surfaces visible | Trainer can show responsible AI controls at a high level |

Fallback:

If run history or traces are sparse, use screenshots from Day 1-4 and explain that Day 6 Foundation Pod will standardize capture and dashboards.

### 6.2 Application Insights And Log Analytics

Open:

- Application Insights resource `proj-an2607101-default-resource-appinsights`, currently seen under `rg-ai-shared-platform-an2607101` in the portal; if the deployment changes, search by name.
- Log Analytics workspace in `rg-ai-observability-an2607101`.

Dry-run example queries:

```kusto
requests
| where timestamp > ago(24h)
| summarize RequestCount=count(), AvgDurationMs=avg(duration) by cloud_RoleName
| order by RequestCount desc
```

```kusto
traces
| where timestamp > ago(24h)
| take 50
```

```kusto
exceptions
| where timestamp > ago(24h)
| take 25
```

Acceptance:

- Trainer can open the workspace.
- At least one query succeeds.
- If there is no recent telemetry, trainer can still show the query shape and explain what Day 6 must configure.

### 6.3 Detailed Observability Dry Run

This is the most important Day 5 platform dry run. Do it before class, not live for the first time.

#### 6.3.1 Application Insights Portal Surfaces

Open the Application Insights resource and verify the following surfaces are available.

| Surface | What to check | What the trainer should be ready to explain |
|---|---|---|
| Overview | resource is healthy; data exists or absence is understood | App Insights is the application telemetry landing zone. |
| Application map | components/dependencies appear if telemetry exists | AI-native apps need dependency visibility across app, model, tools, gateway, and storage. |
| Live metrics | page opens even if traffic is low | Live metrics is useful during load or active demos. |
| Transaction search / Search | recent operations, traces, or dependencies can be searched | This is where a bad user request can be investigated. |
| Failures | exceptions/failures view opens | Failures are not only HTTP 500s; tool/model/gateway failures matter too. |
| Performance | request duration view opens | Latency must be decomposed by app, model, retrieval, tool, and gateway. |
| Logs | KQL query editor opens | KQL is the trainer's most reliable fallback when UI views are sparse. |
| Workbooks | workbook gallery opens | Day 6 can create ProNative standard workbooks. |

Application Insights now includes AI-agent-oriented guidance through Azure Monitor and OpenTelemetry. During the dry run, verify whether an agent details view or agent-related experiences are available in the tenant. If not visible, explain it as readiness rather than a failed demo.

#### 6.3.2 Log Analytics Table Discovery

Open the Log Analytics workspace and expand the left-side table list.

Check whether these table families exist:

| Table family | Possible tables | Meaning |
|---|---|---|
| Application telemetry | `requests`, `dependencies`, `traces`, `exceptions` | Common Application Insights workspace tables |
| Alternate App Insights schema | `AppRequests`, `AppDependencies`, `AppTraces`, `AppExceptions` | Some workspaces expose App-prefixed tables |
| AKS workload logs | `ContainerLogV2`, `KubePodInventory`, `InsightsMetrics`, `AzureDiagnostics` | Useful for AgentGateway on AKS when Container Insights / diagnostic settings are enabled |
| Azure resource logs | `AzureActivity`, `AzureMetrics`, `AzureDiagnostics` | Control plane and platform signals |
| Custom logs | workspace-specific `_CL` tables | Gateway or app-specific custom ingestion |

If a table is missing, do not improvise. Say:

```text
This table does not have data in the current workspace. The Day 6 Foundation Pod must standardize telemetry ingestion and naming.
```

#### 6.3.3 Minimum KQL Query Pack

Run these before delivery and save the working versions.

Requests:

```kusto
requests
| where timestamp > ago(24h)
| summarize RequestCount=count(), AvgDurationMs=avg(duration), Failures=countif(success == false) by cloud_RoleName
| order by RequestCount desc
```

Dependencies:

```kusto
dependencies
| where timestamp > ago(24h)
| summarize Calls=count(), AvgDurationMs=avg(duration), Failures=countif(success == false) by target, type
| order by Calls desc
```

Exceptions:

```kusto
exceptions
| where timestamp > ago(24h)
| project timestamp, cloud_RoleName, type, outerMessage, operation_Id
| order by timestamp desc
| take 25
```

Traces:

```kusto
traces
| where timestamp > ago(24h)
| project timestamp, cloud_RoleName, message, severityLevel, operation_Id
| order by timestamp desc
| take 50
```

Correlate one operation:

```kusto
let op = "<paste-operation-id>";
union requests, dependencies, traces, exceptions
| where operation_Id == op
| order by timestamp asc
```

If the workspace uses App-prefixed tables, use:

```kusto
AppRequests
| where TimeGenerated > ago(24h)
| summarize RequestCount=count(), AvgDurationMs=avg(DurationMs), Failures=countif(Success == false) by AppRoleName
| order by RequestCount desc
```

#### 6.3.4 AI-Specific Telemetry Fields To Look For

During dry run, inspect one trace or dependency record and check `customDimensions` / properties for:

| Field type | Example names to look for | Why it matters |
|---|---|---|
| model/deployment | `model`, `deployment`, `modelDeployment`, `ai.model.name` | Reproducibility and cost |
| prompt/instruction version | `promptVersion`, `instructionsVersion`, `agentName` | Behavior change control |
| token usage | `promptTokens`, `completionTokens`, `totalTokens` | Cost and optimization |
| gateway route | `gatewayRoute`, `backend`, `routeName` | Runtime path |
| agent/session | `agentId`, `agentName`, `sessionId`, `threadId`, `operation_Id` | Traceability |
| tool calls | `toolName`, `mcpServer`, `functionName` | Security and debugging |
| retrieval | `index`, `source`, `documentId`, `groundingSource` | Grounding evidence |
| cost tags | `BatchId`, `StudentId`, `CostCenter`, `EnvironmentId` | FinOps |

If these fields are absent, mark them as Day 6 Foundation Pod telemetry requirements.

#### 6.3.5 Observability Acceptance

Before class, trainer should be able to complete this sentence with evidence:

```text
For one AI request, I can show at least one of: request trace, dependency call, exception, model/tool log, gateway log, or cost attribution path.
```

## 7. AgentGateway Readiness

Confirm:

| Check | Expected |
|---|---|
| Gateway endpoint reachable | `https://aks-an2607101-agw-dns-xywyud9k.hcp.centralindia.azmk8s.io` responds or shows an expected gateway response |
| AKS resource visible | AKS cluster, namespace, AgentGateway deployment/service/ingress are visible |
| Logs available | AKS pod logs, Container Insights, or Log Analytics logs can be inspected |
| Policy/routing config available | Trainer can show route, backend, timeout/retry/rate-limit concepts |
| Managed identity / backend auth understood | Trainer can explain how gateway reaches Foundry/model/tool backends |

Fallback:

If the live gateway is unavailable, use the Day 4 gateway lab output and architecture diagram. Make the limitation explicit:

```text
The gateway walkthrough is using architecture and prior lab evidence because the live gateway endpoint is unavailable.
```

### 7.1 Detailed AgentGateway Dry Run

AgentGateway must be shown as an operations control plane, not only a network proxy.

#### 7.1.1 Azure Resource Checks

In Azure portal, check:

| Area | Expected evidence |
|---|---|
| Resource group | `rg-ai-governance-hub-an2607101` exists |
| AKS cluster | AgentGateway runs on AKS instead of Azure Container Apps |
| Namespace/workload | AgentGateway deployment, pods, service, and ingress are visible |
| Managed identity/workload identity | identity is assigned if backend Azure resources require it |
| Kubernetes secrets/config | backend endpoints and auth references are configured without exposing secrets |
| Logs | pod logs / Container Insights logs available |
| Ingress | AKS ingress or load balancer exposes the AgentGateway endpoint and TLS status is understood |
| Scale | replica count, HPA/KEDA settings, and node pool capacity are understood |

#### 7.1.2 Gateway Configuration Checks

Trainer should be ready to show or explain these gateway concepts:

| Gateway feature | Dry-run question |
|---|---|
| Route | Which incoming path maps to which backend? |
| Backend | Is backend a Foundry endpoint, model endpoint, MCP server, A2A endpoint, or ordinary API? |
| Auth | Is auth API key, bearer token, managed identity, JWT, or another mechanism? |
| Rate limit | What happens when a student/app sends too many calls? |
| Timeout | What is the maximum wait for slow model/tool calls? |
| Retry | Which failures are retried and which are not? |
| Logs | Where can a gateway request be found? |
| Trace correlation | Which header or operation ID connects app and gateway logs? |
| Cost attribution | Which tags/headers identify batch, student, route, or workload? |

#### 7.1.3 Test Request Shape

Prepare one safe request to the gateway endpoint. Do not include real secrets in the playbook.

```powershell
$env:AGENTGATEWAY_ENDPOINT="https://aks-an2607101-agw-dns-xywyud9k.hcp.centralindia.azmk8s.io"
Invoke-RestMethod -Method Get -Uri "$env:AGENTGATEWAY_ENDPOINT/health"
```

If there is no `/health` route, use the configured readiness route or show the expected route table instead.

Confirm during dry run that this hostname is the AgentGateway workload ingress. If it is only the AKS API-server FQDN, replace it with the actual ingress/load balancer hostname before using it as `AGENTGATEWAY_ENDPOINT`.

#### 7.1.4 Gateway Acceptance

Before delivery, trainer should know:

- which gateway routes are real in the environment
- which routes are architecture walkthrough only
- where gateway logs appear
- how rate limiting and cost attribution will be explained
- how this prepares Day 6 Foundation Pod gateway baseline

## 8. Agent 365 / Admin Governance Readiness

Confirm trainer access to the relevant Microsoft 365 admin surfaces.

| Check | Expected |
|---|---|
| Microsoft 365 admin center opens | Trainer can access tenant admin view |
| Agent/Copilot admin areas visible | Trainer can show where agent governance is managed |
| Agent 365 licensing/readiness understood | Trainer can explain if full Agent 365 functionality is enabled or only shown as a readiness walkthrough |
| Entra visible | Trainer can show identity and access control concepts |
| Purview/Defender discussion ready | Trainer can explain data and threat-protection boundaries |

Do not promise student hands-on for Agent 365 unless licenses and tenant permissions are ready.

### 8.1 Detailed Agent 365 / Admin Governance Dry Run

Agent 365 should be treated as an enterprise governance walkthrough. The goal is to show what must be governed when agents become numerous and distributed.

#### 8.1.1 Tenant Surface Checks

Open Microsoft 365 admin center and check available navigation. Based on tenant readiness, the exact labels can vary.

| Area | What to look for | Teaching use |
|---|---|---|
| Copilot / Agents navigation | Overview, All agents, Shadow AI, Tools, Settings where available | Shows agent control moving into admin experience |
| Agent list / registry | agent names, owners, status, source, activity if available | Agent inventory and lifecycle |
| Tools / connectors | approved or available tools/connectors | Tool governance |
| Settings | who can access or create/share agents | Access and policy boundary |
| Users / groups | users and groups for access control | Entra-based governance |
| Roles | admin roles and least privilege | Separation of duties |

If Agent 365 is not fully enabled, show the nearest available admin surfaces and explain the prerequisites.

#### 8.1.2 Observe, Govern, Secure Walkthrough

Prepare one example for each:

| Pillar | Trainer evidence | Explanation |
|---|---|---|
| Observe | registry, dashboard, activity, agent list, or screenshot | Operators need visibility into which agents exist and how they behave. |
| Govern | settings, groups, access, lifecycle, sharing controls | Agents require ownership, lifecycle, permission, and compliance controls. |
| Secure | Entra, Purview, Defender linkage discussion | Agents need identity, data protection, and threat protection. |

#### 8.1.3 Questions To Be Ready For

| Student question | Trainer answer direction |
|---|---|
| Can students create agents here? | Not unless tenant licensing and permissions are explicitly enabled. This is a trainer/admin walkthrough. |
| Is Agent 365 required for every Foundry agent? | Position it as enterprise control plane where available; local app-level controls and gateway controls still matter. |
| What is shadow AI? | Agents or AI tools operating outside approved governance/inventory. |
| How does Entra fit? | Identity, access, groups, conditional access, and workload identity boundaries. |
| How does Purview fit? | Data classification, DLP, compliance, and data-risk visibility. |
| How does Defender fit? | Threat detection and runtime protection posture. |

## 9. FinOps Readiness

Open Azure Cost Management for the batch subscription.

Confirm:

| Check | Expected |
|---|---|
| Subscription cost view visible | Trainer can filter by subscription/resource group/tag |
| Batch budget framing ready | INR 20,000 budget ceiling can be explained |
| Tag filters understood | `BatchId`, `EnvironmentId`, `StudentId`, `CostCenter`, `DeleteAfter` where available |
| High-cost services identified | Azure AI Search, GPU services, always-on compute, model deployments |
| Weekend scale-down/deprovisioning checklist ready | Trainer can explain what is deleted, stopped, or retained |

Day 5 FinOps is a walkthrough. Day 6 should turn it into Foundation Pod operating checks.

### 9.1 Detailed FinOps Dry Run

FinOps must be shown as an operating discipline, not only a billing screen.

#### 9.1.1 Azure Cost Management Views

Open Azure portal:

```text
Subscription -> Cost Management -> Cost analysis
```

Prepare these views:

| View | Group by | Filter | Purpose |
|---|---|---|---|
| Cost by resource group | Resource group | current billing period | show shared vs student-specific cost |
| Cost by service | Service name | current billing period | identify cost-heavy services |
| Cost by resource | Resource | current billing period | isolate costly resources |
| Cost by tag | Tag if available | `BatchId=AN-2607-101` | show attribution model |
| Forecast / accumulated cost | time | subscription or RG | discuss budget risk |

#### 9.1.2 Budget Checks

Open:

```text
Cost Management -> Budgets
```

Dry-run values:

| Item | Recommended training value |
|---|---|
| Budget name | `budget-an2607101-training` |
| Scope | subscription or training resource group scope |
| Amount | INR 20,000 framing for the batch |
| Alert thresholds | 50%, 80%, 90%, 100% |
| Alert type | actual cost and optionally forecasted |
| Recipients | trainer/admin distribution list |
| Action group | optional, if configured for automation |

Do not rely only on budget alerts. Cost data can lag. High-cost idle services must be stopped or deleted deliberately.

#### 9.1.3 AI-Specific Cost Checks

Prepare a table in class:

| Cost area | What to inspect | Control |
|---|---|---|
| Model usage | deployment/model usage, token dashboards if available | model choice, prompt length, caching |
| Azure AI Search | service existence and SKU | delete when not needed; hourly cost lesson |
| AKS AgentGateway workload | pod replicas, node pool capacity, ingress exposure | scale deployment/node pool only when not needed |
| Log Analytics | ingestion and retention | filter noise, set sane retention |
| Cosmos DB | throughput/serverless mode and storage | serverless/pay-per-use where appropriate |
| Runpod | active workers, endpoint settings | active workers zero unless needed |
| Local models | loaded model on local server | unload after demo |

#### 9.1.4 FinOps Acceptance

Trainer should be able to answer:

- What is the current batch cost view?
- Which services are the top cost drivers?
- What should be deleted or stopped after the weekend?
- Which tags support cost attribution?
- What is the alert plan for INR 20,000?
- What changes in Day 6 Foundation Pod to make this repeatable?

## 10. Foundry Local On CPU Readiness

Day 5 uses a local CPU-based server or trainer machine. DGX Spark is not available for this batch.

Dry-run prerequisites:

| Check | Expected |
|---|---|
| OS | Windows machine or Windows server preferred for this walkthrough |
| .NET | .NET 10 installed |
| Disk | Enough space for model/runtime downloads |
| Network | Can download NuGet packages and model artifacts |
| Model choice | Small local model such as `qwen2.5-0.5b` for CPU feasibility |

Reference package direction from Microsoft Foundry Local quickstart:

```powershell
dotnet add package Microsoft.AI.Foundry.Local.WinML
dotnet add package OpenAI
```

Dry-run acceptance:

- Trainer can explain local model lifecycle: discover, download, load, run chat, unload.
- If the machine cannot complete model download in time, trainer shows the quickstart, package names, and prepared screenshots or prior output.
- Make clear this is CPU-based for alpha batch; DGX Spark remains a future enterprise hardware option.

### 10.1 Detailed Foundry Local CPU Dry Run

Foundry Local should be shown as a model lifecycle walkthrough.

#### 10.1.1 Pre-download If Possible

If internet/model download time is uncertain, pre-run the demo before class.

Checklist:

| Step | Evidence |
|---|---|
| packages restored | `dotnet restore` succeeds |
| execution providers discovered | console lists providers |
| small model selected | model alias such as `qwen2.5-0.5b` |
| model downloaded | cached locally |
| model loaded | console shows loaded/ready |
| chat completion works | response appears |
| model unloaded | console confirms unload |

#### 10.1.2 CPU Framing

Make these boundaries explicit:

- CPU local inference is for lifecycle understanding, not high-throughput production.
- Small models are appropriate for demo.
- DGX Spark is not part of this batch's hands-on environment.
- Local inference still needs evaluation, versioning, safety, and observability.

#### 10.1.3 Foundry Local Acceptance

Trainer must be ready to explain:

```text
Azure-hosted Foundry gives managed enterprise endpoint.
Foundry Local gives local model lifecycle.
Runpod gives external neocloud worker/endpoint pattern.
All three still need operations and governance.
```

## 11. Runpod Neocloud Readiness

Runpod is the neocloud walkthrough option for this batch.

Confirm:

| Check | Expected |
|---|---|
| Runpod account access | Trainer can sign in |
| API key handling | API key is stored securely and never displayed to students |
| Serverless endpoint view | Trainer can show endpoint, worker, logs, cold start, and cost/runtime knobs |
| Endpoint invocation option | Trainer can call a public/prepared endpoint or show request shape |
| Cost control | Minimum workers, idle shutdown, endpoint limits, and cold start tradeoff are explained |

Suggested environment variables if invoking from local machine:

```powershell
$env:RUNPOD_API_KEY="<do-not-share>"
$env:RUNPOD_ENDPOINT_ID="<endpoint-id>"
```

Do not put real Runpod API keys in any playbook.

### 11.1 Detailed Runpod Dry Run

Runpod should be shown as a neocloud operating model, not merely an API endpoint.

#### 11.1.1 Dashboard Checks

Before class, trainer should be able to show:

| Runpod area | What to show |
|---|---|
| account/project | where endpoints and usage are managed |
| serverless endpoint | endpoint ID, status, worker config |
| workers | active/idle behavior |
| logs | where request failures and cold starts are diagnosed |
| endpoint settings | GPU/CPU choice, scaling, timeouts, active workers |
| API key page | show location only; do not expose real key |
| billing/usage | where cost control is reviewed |

#### 11.1.2 Request Flow

Explain this flow:

```text
client request
  -> Runpod endpoint
  -> queue if no worker is warm
  -> worker/container starts or receives job
  -> model/runtime processes request
  -> result is returned or polled
  -> worker remains warm briefly
  -> idle worker shuts down
```

#### 11.1.3 Cost Control Checks

Confirm:

| Setting | Training recommendation |
|---|---|
| active workers | `0` unless a live demo requires warm worker |
| max workers | low limit for training |
| timeout | explicit and reasonable |
| endpoint logs | enabled/available |
| API key | stored outside repo/playbooks |
| test payload | small and non-sensitive |

#### 11.1.4 Runpod Acceptance

Trainer should be able to explain:

- why cold start happens
- what a worker is
- why serverless can still become expensive
- why secrets and data boundary matter
- how Runpod differs from Azure-hosted Foundry
- how Runpod differs from local CPU Foundry Local

## 12. Day 6 Bridge Readiness

Day 5 must prepare Day 6 Foundation Pod setup.

Before delivery, confirm the trainer can explain which Day 5 walkthroughs become Day 6 foundation assets:

| Day 5 walkthrough | Day 6 Foundation Pod implication |
|---|---|
| LLMOps | Prompt/model/evaluation lifecycle setup |
| GenAIOps | Trace, feedback, eval, release, and rollback loop |
| Observability | App Insights, Log Analytics, dashboards, KQL queries |
| AgentGateway | Routing, policy, token/cost attribution, trace correlation |
| Governance | Identity, access, Agent 365 readiness, Purview/Defender boundaries |
| FinOps | Budget, tags, dashboard, weekend shutdown/deprovisioning |
| Hybrid AI | Azure hosted vs local CPU vs Runpod neocloud decision model |

## 13. Dry Run Acceptance Checklist

| Item | Pass criteria | Status |
|---|---|---|
| Trainer Azure login works | `az account show` succeeds |  |
| Foundry project opens | Project and model deployment are visible |  |
| Foundry monitoring surface opens | Monitor/traces/evaluation areas are visible |  |
| App Insights opens | Application telemetry surface is visible |  |
| Log Analytics query runs | At least one query succeeds |  |
| AgentGateway walkthrough ready | Live endpoint or fallback architecture evidence ready |  |
| Agent 365/admin walkthrough ready | Admin screens or fallback screenshots ready |  |
| Cost Management opens | Batch cost view can be demonstrated |  |
| Foundry Local CPU walkthrough ready | Live demo or prepared output ready |  |
| Runpod walkthrough ready | Dashboard/API flow ready without exposing secrets |  |
| Day 6 bridge ready | Foundation Pod implications can be explained |  |

## 14. Trainer Fallback Rules

| Problem | Fallback |
|---|---|
| No recent telemetry | Show query shape and explain Day 6 instrumentation requirement. |
| AgentGateway unavailable | Use Day 4 gateway lab evidence and architecture view. |
| Agent 365 license/surface unavailable | Use admin readiness walkthrough and explain prerequisite boundary. |
| Foundry Local download too slow | Use prepared screenshots/output and explain model lifecycle. |
| Runpod endpoint unavailable | Use Runpod dashboard and request/worker/cold-start flow without running inference. |
| Cost data delayed | Explain Azure Cost Management delay and use budget/tagging examples. |

## 15. References

| Topic | Reference |
|---|---|
| GenAIOps | https://learn.microsoft.com/en-us/training/paths/operationalize-gen-ai-apps/ |
| LLMOps | https://learn.microsoft.com/en-us/ai/playbook/technology-guidance/generative-ai/mlops-in-openai/ |
| LLMOps Workshop | https://microsoft.github.io/llmops-workshop/ |
| Application Insights | https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview |
| Log Analytics | https://learn.microsoft.com/en-us/azure/azure-monitor/logs/log-analytics-overview |
| Agent 365 | https://learn.microsoft.com/en-us/microsoft-agent-365/overview |
| Agent Governance Toolkit | https://github.com/microsoft/agent-governance-toolkit |
| AgentGateway | https://agentgateway.dev/docs/standalone/latest/ |
| Foundry Local quickstart | https://learn.microsoft.com/en-us/azure/foundry-local/get-started?tabs=windows&pivots=programming-language-csharp |
| Runpod overview | https://docs.runpod.io/overview |
| Runpod serverless | https://docs.runpod.io/serverless/overview |
| Azure Cost Analysis | https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/quick-acm-cost-analysis |
| Azure Budgets | https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/tutorial-acm-create-budgets |
| Azure FinOps Agent | https://github.com/Azure-Samples/azure-finops-agent |
| FinOps for AI | https://www.finops.org/wg/finops-for-ai-overview/ |
