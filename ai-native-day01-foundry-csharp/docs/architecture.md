# Day 1 Architecture

```mermaid
flowchart LR
    STUDENT["Student Device\n.NET SDK 10"] --> APP["Day01FoundryChat"]
    APP --> FOUNDRY["Azure AI Foundry / Azure OpenAI\nshared deployment"]
    APP --> SEARCH["Azure AI Search\nstudent index"]
    APP --> TRACE["Console Trace\nApp Insights-ready metadata"]
```

## Live Azure Resources

| Resource | Expected Name Pattern |
|---|---|
| Shared platform RG | `rg-ai-shared-platform-an2607101` |
| Observability RG | `rg-ai-observability-an2607101` |
| Student RG | `rg-st-2606-1000-ai-native-an2607101` |
| Search service | `srch-an2607101` |
| Student Search index | `idx-st26061000-grounding` |

