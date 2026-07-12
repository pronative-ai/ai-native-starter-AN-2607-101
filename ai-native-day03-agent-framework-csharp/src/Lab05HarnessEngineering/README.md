# Lab 05 - Harness Engineering

## Use Case

This lab demonstrates the official Microsoft Agent Framework Harness - the runtime layer around the model that manages tools, approvals, files, todo/mode state, loop evaluation, compaction, evidence capture, and OpenTelemetry.

Key components:

- **AsHarnessAgent()** - Creates harness-managed agent with all built-in providers
- **HarnessAgentOptions** - Configures compaction, tool approval, file memory/access, and telemetry
- **LoopEvaluators** - `CompletionMarkerLoopEvaluator("DONE")` stops when response contains marker
- **File memory/access** - `FileSystemAgentFileStore` for evidence persistence
- **Live Azure OpenAI** - Direct model call through `AzureOpenAIClient`

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- Azure CLI installed and authenticated (`az login`)
- Azure subscription with Azure OpenAI endpoint

### Steps

1. **Authenticate with Azure:**
   ```powershell
   az login
   ```

2. **Set environment variables** (see [Root README](../../README.md#required-foundry-settings) for values):
   ```powershell
   $env:AZURE_OPENAI_ENDPOINT="<your-openai-endpoint>"
   $env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
   ```

3. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab05HarnessEngineering\Lab05HarnessEngineering.csproj
   ```

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| Azure CLI | Authentication |
| Azure OpenAI endpoint | Live model calls |
| GPT-5-mini deployment | Powers the agent |

## Sample Input

**Default prompt (automatic):**
```
Review whether the Day 3 Lab 05 harness is ready for classroom delivery.
BatchId: AN-2607-101
StudentId: ST-2606-1000

Use the harness-managed tools and context providers...
```

## Expected Output

```
Step 1 - Harness configuration evidence
=======================================
Package: Microsoft.Agents.AI.Harness
Azure OpenAI endpoint: https://...
Model deployment: gpt-5-mini
Loop evaluator: CompletionMarkerLoopEvaluator("DONE")

Step 2 - Run repeatable harness prompt
======================================
[Prompt text]

Step 3 - Harness response
=========================
Evidence
========
[Tool-based evidence from assess_harness_readiness]

Evaluation
==========
[Tool-based evaluation from capture_harness_evidence]

Completion
==========
DONE

Step 4 - Session evidence
=========================
[Serialized StateBag metadata]

Step 5 - Evidence artifact
==========================
Harness evidence written to: output/day03-lab05-harness-evidence.json
```

## Key Learning Points

1. **Harness = runtime** - Controls tools, approval, files, telemetry around the model
2. **Tool approval** - Harness manages auto-approval rules through options
3. **Loop evaluation** - Stops when "DONE" appears in response
4. **File memory/access** - Evidence persisted to file store
5. **OpenTelemetry** - Traces emitted through harness configuration
6. **Compaction** - Context window managed via `MaxContextWindowTokens`

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Authentication error | Run `az account show` to verify login |
| Model deployment not found | Check `AZURE_OPENAI_CHAT_DEPLOYMENT=gpt-5-mini` |
| Loop doesn't terminate | Ensure agent instructions include "DONE" in final response |
| Evidence file not created | Check `fileStoreDirectory` permissions |

## Reference

- [Agent Framework Harness](https://learn.microsoft.com/en-us/agent-framework/agents/harness?pivots=programming-language-csharp)
