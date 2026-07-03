# AI Native Starter — AN-2607-101

C# starter pack for the ProNative AI training — two days of hands-on labs using Azure AI Foundry and .NET 10.

## Repository Structure

```
.
├── ai-native-day01-foundry-csharp/     # Day 1: Generative AI Apps
│   ├── src/Day01FoundryChat            # Foundry SDK chat console app
│   ├── samples/                        # Sample configuration files
│   ├── data/                           # Lab data assets
│   ├── harness/                        # Repeatable prompts & observations
│   └── docs/                           # Student & trainer guides, architecture
│
└── ai-native-day02-agent-csharp/       # Day 2: AI Agents
    ├── src/                            # Lab projects (01–08)
    │   ├── Lab01FoundryAgentClient     # Core: invoke Foundry agent
    │   ├── Lab02CustomToolsAgent       # Core: custom tool implementation
    │   ├── Lab03McpAgentClient         # Core/trainer-led: MCP approval flow
    │   ├── Lab04FoundryIqAgentClient   # Trainer-led: Foundry IQ connected agent
    │   ├── Lab05/                      # Lab 05
    │   ├── Lab06WorkflowClient         # Preview: workflow invocation
    │   ├── Lab07AgentFrameworkSingleAgent  # Preview: single-agent pattern
    │   ├── Lab08AgentFrameworkMultiAgent   # Preview: multi-agent pattern
    │   └── Pronative.AgentTraining.Shared  # Shared utilities
    ├── samples/                        # Sample configurations
    ├── harness/                        # Lab harness infrastructure
    └── docs/                           # Architecture, lab mapping, guides
```

## Day 1 — Generative AI Apps

Follows the MS Learn AI-3016 flow, adapted for the ProNative live Azure landing zone.

| Topic | Lab |
|---|---|
| Foundry orientation | Explore AI Foundry project & settings |
| Model catalog & evaluation | Capture model/deployment metadata |
| Foundry SDK app | C# chat call using live model deployment |
| RAG / grounding | Azure AI Search grounding hook |
| Responsible AI | Content-safety reflection & trace |

```powershell
cd ai-native-day01-foundry-csharp/src/Day01FoundryChat
dotnet run -- --ask "What is AI-native architecture?"
```

## Day 2 — AI Agents

Follows the MS Learn AI-3026 agent labs, adapted to a ProNative C# flow for live Azure.

| Lab | Project | Depth |
|---|---|---|
| 01 | `Lab01FoundryAgentClient` | Core hands-on |
| 02 | `Lab02CustomToolsAgent` | Core hands-on |
| 03 | `Lab03McpAgentClient` | Core or trainer-led |
| 04 | `Lab04FoundryIqAgentClient` | Trainer-led |
| 05 | `Lab05` | Trainer-led: Portal practical |
| 06 | `Lab06WorkflowClient` | Preview |
| 07 | `Lab07AgentFrameworkSingleAgent` | Preview |
| 08 | `Lab08AgentFrameworkMultiAgent` | Preview |

```powershell
cd ai-native-day02-agent-csharp/src/Lab01FoundryAgentClient
dotnet run
```

Set `USE_LIVE_FOUNDRY=true` and configure endpoint environment variables to target the live Azure deployment.

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- Azure CLI — logged in with the assigned student identity
- Access to the student resource group, shared Foundry, Search, and observability resources

## Architecture

### Day 1

```
Student Device → Day01FoundryChat → Azure AI Foundry / Azure OpenAI
                                         → Azure AI Search (grounding)
                                         → Console Trace
```

### Day 2

```
Student Device → Agent API → Agent Runner (instruction + routing)
                                → Foundry Model Deployment
                                → Custom Tools
                                → MCP Tool Discovery
                                → Console Trace
```

## Auth Model

Both days support two modes:

1. **BearerToken** — Entra / managed identity token flow (production-grade, no secrets committed)
2. **ApiKey** — fallback for trainer-controlled demos where approved (Current option for training purpose only)

## Azure Resource Naming

| Resource | Name Pattern |
|---|---|
| Shared platform RG | `rg-ai-shared-platform-an2607101` |
| Observability RG | `rg-ai-observability-an2607101` |
| Student RG | `rg-st-{id}-ai-native-an2607101` |
| Search service | `srch-an2607101` |
| Student Search index | `idx-st{id}-grounding` |
| Student managed identity | `id-st{id}-training-cin` |

## Trainer Reference

See the `docs/` folders under each day for:

- **Student guides** — walkthroughs for each lab
- **Trainer guides** — delivery notes and discussion points
- **Architecture diagrams** — system flow and resource mapping
- **Lab mapping** — MS Learn source lab → C# adaptation mapping
