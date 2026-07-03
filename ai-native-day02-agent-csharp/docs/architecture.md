# Day 2 Architecture

```mermaid
flowchart LR
    STUDENT["Student Device\n.NET SDK 10"] --> API["Day02AgentApi"]
    API --> AGENT["Agent Runner\ninstruction + routing"]
    AGENT --> MODEL["Foundry Model Deployment"]
    AGENT --> TOOL["Custom Tool\n/tools/order-status"]
    API --> MCP["Tool Discovery\n/.well-known/mcp/tools"]
    API --> TRACE["Console Trace\nApp Insights-ready metadata"]
```

## Live Azure Resources

| Resource | Expected Name Pattern |
|---|---|
| Shared platform RG | `rg-ai-shared-platform-an2607101` |
| Observability RG | `rg-ai-observability-an2607101` |
| Student RG | `rg-st-2606-1000-ai-native-an2607101` |
| Student managed identity | `id-st26061000-training-cin` |

