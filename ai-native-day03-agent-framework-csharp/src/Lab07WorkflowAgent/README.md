# Lab 07 - Workflow Agent

## Use Case

This lab demonstrates native Microsoft Agent Framework workflow-agent orchestration using `AgentWorkflowBuilder` to compose specialist agents inside a structured sequential pipeline.

The workflow processes training change requests through four specialist agents:

1. **Intent Analyst** - Extracts category, goal, constraints, and gaps
2. **Planning Agent** - Creates implementation steps, owners, and resources
3. **Risk Reviewer** - Identifies learner impact, cost, timing, and governance risks
4. **Finalizer Agent** - Produces enterprise-ready recommendation with structured output

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- Azure CLI installed and authenticated (`az login`)
- Azure AI Foundry project with GPT-5-mini deployment

### Steps

1. **Authenticate with Azure:**
   ```powershell
   az login
   ```

2. **Set environment variables** (see [Root README](../../README.md#required-foundry-settings) for values):
   ```powershell
   $env:AZURE_AI_PROJECT_ENDPOINT="<your-project-endpoint>"
   $env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
   $env:AZURE_OPENAI_ENDPOINT="<your-openai-endpoint>"
   ```

3. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab07WorkflowAgent\Lab07WorkflowAgent.csproj
   ```

4. **Enter a training change request** when prompted, or press Enter for the default.

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| Azure CLI | Authentication |
| Azure AI Foundry Project | Hosts the specialist agents |
| GPT-5-mini deployment | Powers all four agents |
| Azure OpenAI endpoint | Structured output normalization |

## Sample Input

**Default request (press Enter):**
```
ProNative wants to add an optional evening support clinic before Day 4 for students who struggled with
Agent Framework skills, workflow state, and Azure AI Search grounding. Evaluate the intent, create an
execution plan, review delivery risk, and produce a final structured action recommendation.
```

**Custom request example:**
```
We need to migrate all student session checkpoints from Cosmos DB to a new region before Day 4.
Evaluate the intent, plan the migration, review risks, and recommend next steps.
```

## Expected Output

```
[workflow:event] started

[Intent Analyst Agent Response]
Intent category: Service Extension
Business goal: Add optional evening support clinic...
Constraints: Must not conflict with Day 4 schedule...

[Planning Agent Response]
Proposed steps: 1. Notify students, 2. Set up support room...
Owner: Training operations team...

[Risk Reviewer Agent Response]
Learner impact: Low - optional attendance...
Timing risk: Medium - short notice...

[Finalizer Agent Response]
Decision: Approved with conditions
Rationale: The request is safe to proceed...
Action Plan: [1. Notify..., 2. Provision..., 3. Schedule...]
Risks: [Low learner impact, Medium timing risk]
Approval: Required from training lead
Evidence To Capture: [Student attendance, Session recordings]

Structured Output Normalization
===============================
{
  "terminalStatus": "approved_with_conditions",
  "intentSummary": "Optional evening support clinic...",
  "decision": "Approved with conditions",
  "rationale": "The request addresses learner needs...",
  "nextAction": "Notify students and provision support room",
  "approvalRequired": "Training lead sign-off",
  "agentSequence": ["Intent Analyst", "Planning Agent", "Risk Reviewer", "Finalizer"],
  "actionPlan": ["Notify students", "Provision room", "Schedule clinic"],
  "risks": ["Low learner impact", "Medium timing risk"],
  "evidenceToCapture": ["Student attendance", "Session recordings"]
}

Evidence artifact: .../artifacts/ST-2606-1000/day03-lab07-workflow-agent-result.json
```

## Key Learning Points

1. **Workflow-agent pattern** - Specialist agents inside a structured workflow
2. **Sequential pipeline** - Agents run in order with intermediate output visible
3. **Specialist roles** - Each agent has clear responsibilities (intent, plan, risk, final)
4. **Structured output** - `GetResponseAsync<T>` normalizes transcript into typed JSON
5. **Chain-only responses** - Only agent responses flow through the workflow
6. **Enterprise-ready output** - Decision, Rationale, Action Plan, Risks, Approval, Evidence

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Authentication error | Run `az account show` to verify login |
| Model deployment not found | Check `AZURE_OPENAI_CHAT_DEPLOYMENT=gpt-5-mini` |
| Structured output fails | Ensure `AZURE_OPENAI_ENDPOINT` is set correctly |
| Workflow doesn't complete | Check all four specialist agents are defined |

## Reference

- [Agents in Workflows](https://learn.microsoft.com/en-us/agent-framework/workflows/agents-in-workflows)
