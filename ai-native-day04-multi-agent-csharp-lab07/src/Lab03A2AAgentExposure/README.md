# Lab 03 - A2A Agent Exposure and Consumption

## Use Case

This lab demonstrates Agent-to-Agent (A2A) interoperability with two components:

- **Provider** (Lab03A2AAgentExposure) - Exposes an Agent Framework agent over A2A protocol
- **Consumer** (Lab03A2AAgentConsumer) - Discovers the Agent Card and invokes the remote agent

Key concepts:

- **Agent Card** - Describes agent identity, capabilities, and supported interfaces
- **JSON-RPC** - Protocol for agent message exchange
- **API Center** - Enterprise catalog/governance metadata (not runtime broker)

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- Azure CLI installed and authenticated (`az login`)
- Azure AI Foundry project with GPT-5-mini deployment

### Steps

1. **Start the A2A Provider** (Terminal 1):
   ```powershell
   dotnet run --project .\src\Lab03A2AAgentExposure\Lab03A2AAgentExposure.csproj
   ```

2. **Run the A2A Consumer** (Terminal 2):
   ```powershell
   $env:A2A_BASE_URL="http://localhost:5000"
   $env:A2A_AGENT_CARD_PATH="/a2a/training-ops/v1/card"
   dotnet run --project .\src\Lab03A2AAgentConsumer\Lab03A2AAgentConsumer.csproj
   ```

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the applications |
| Azure CLI | Authentication |
| Azure AI Foundry Project | Hosts the training-ops-agent |
| GPT-5-mini deployment | Powers the agent |

## Sample Input

**Default message (automatic):**
```
As a consumer agent, ask the training operations A2A agent to assess readiness for Day 4 protocol coverage.
```

## Expected Output

**Provider (Terminal 1):**
```
ProNative AI-Native Fullstack Engineering - Day 4
Lab 03 - A2A Agent Exposure
Batch: AN-2607-101 | Student: ST-2606-1000
Foundry project endpoint: https://...
Model deployment: gpt-5-mini

After the web app starts:
- Agent card:     GET  /a2a/training-ops/v1/card
- Message stream: POST /a2a/training-ops/v1/message:stream
```

**Consumer (Terminal 2):**
```
Discover Agent Card
===================
Name: training-ops-agent
Description: Enterprise training operations assistant...
Version: 1.0.0
Supported interfaces: 1
- JsonRpc 1.0: http://localhost:5000/a2a/training-ops/v1

Invoke Remote Agent
===================
Selected endpoint: http://localhost:5000/a2a/training-ops/v1

A2A Response
============
Payload: Message
Role: assistant
Message ID: ...
Context ID: ...
[Agent response text]

Training Takeaway
=================
Provider exposes an Agent Card and message endpoint.
API Center stores the enterprise registration and governance metadata.
Consumer resolves the Agent Card and invokes the selected A2A interface.
```

## Key Learning Points

1. **A2A Provider** - Exposes agent over protocol boundary; agent remains Agent Framework agent
2. **Agent Card** - Describes identity, capabilities, supported interfaces
3. **A2A Consumer** - Resolves card, invokes endpoint, handles response
4. **API Center role** - Enterprise catalog/governance; not runtime broker
5. **AgentGateway** - Optional runtime route/policy point between consumer and provider

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Consumer fails to connect | Ensure provider is running on `http://localhost:5000` |
| Agent card not found | Check `A2A_AGENT_CARD_PATH` matches provider path |
| Authentication error | Verify `az login` and Foundry project access |

## Reference

- [A2A Specification](https://a2a-protocol.org/latest/specification/)
