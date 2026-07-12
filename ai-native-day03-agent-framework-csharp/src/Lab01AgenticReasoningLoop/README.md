# Lab 01 - Agentic AI Reasoning Loop

## Use Case

This lab demonstrates the core agentic reasoning loop pattern used in enterprise AI systems. Instead of directly answering questions, the agent follows a structured process:

```
Goal → Plan → Action → Observation → Reflection
```

The agent connects to Azure AI Foundry, uses real tools to gather operational evidence, and produces a structured, inspectable response. This pattern is foundational for building trustworthy AI systems where every decision can be traced and reviewed.

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- Azure CLI installed
- Azure subscription with Azure AI Foundry project access

### Steps

1. **Authenticate with Azure:**
   ```powershell
   az login
   ```

2. **Set environment variables** (see [Root README](../../README.md#required-foundry-settings) for values):
   ```powershell
   $env:AZURE_AI_PROJECT_ENDPOINT="<your-project-endpoint>"
   $env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
   $env:BATCH_ID="<your-batch-id>"
   $env:STUDENT_ID="<your-student-id>"
   ```

3. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab01AgenticReasoningLoop\Lab01AgenticReasoningLoop.csproj
   ```

4. **Enter a prompt** when asked, or press Enter to use the default readiness question.

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| Azure CLI | Authentication with Azure |
| Azure AI Foundry Project | Hosts the agent and model |
| GPT-5-mini deployment | Powers the agent's reasoning |
| Azure CLI credential | Authenticates to Foundry (no API keys needed) |

## Sample Input

**Default prompt (press Enter):**
```
For batch AN-2607-101 and student ST-2606-1000, demonstrate the agentic reasoning loop.
You must:
1. State the business goal.
2. Create a short milestone plan.
3. Take an action by calling the get_batch_readiness_signal tool.
4. Use the tool result as the observation.
5. Reflect on whether the plan is ready for Lab 02.
```

**Custom prompt example:**
```
What is the readiness status for my training batch?
```

## Expected Output

The agent produces a structured response with five sections:

```
Goal
[States the business objective]

Plan
[Shows milestone breakdown from build_milestone_plan tool]

Action
[Lists the tool calls made]

Observation
[Summarizes readiness data from get_batch_readiness_signal tool]

Reflection
[Evaluates whether to proceed to Lab 02]
```

Followed by:
- **Session Checkpoint Boundary**: Serialized metadata showing what was stored
- **Middleware logs**: Shows inbound messages, tool validation, and execution timing

## Key Learning Points

1. **Structured reasoning** - Agents should follow repeatable patterns, not random outputs
2. **Tool governance** - Middleware blocks dangerous operations (like delete commands)
3. **Auto-approval** - Safe tools (read-only) run without manual approval
4. **Session state** - Checkpoints capture what happened for audit and recovery
5. **Transparency** - Every step (goal, plan, action, observation, reflection) is visible

## Troubleshooting

| Problem | Solution |
|---------|----------|
| `az login` fails | Run `az account show` to verify, or try `az login --use-device-code` |
| Authentication error | Ensure you're logged in with the correct tenant |
| Model deployment not found | Check `AZURE_OPENAI_CHAT_DEPLOYMENT=gpt-5-mini` |
| Project endpoint error | Verify `AZURE_AI_PROJECT_ENDPOINT` ends with `/api/projects/<project-name>` |
| Tool call not appearing | Ensure the agent instructions mention using tools |
| Missing sections in output | The loop evaluator will request revision; check console for feedback |

## Reference

- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/en-us/agent-framework/overview/)
- [Agent Middleware Guide](https://learn.microsoft.com/en-us/agent-framework/agents/middleware/)
