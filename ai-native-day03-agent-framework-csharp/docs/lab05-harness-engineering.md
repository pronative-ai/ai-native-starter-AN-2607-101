# Lab 05 - Harness Engineering

## Purpose

Lab 05 teaches that an agent harness is the runtime around the model. The model generates text and tool-call requests; the harness provides operational scaffolding for longer-running work: tools, approval, context providers, file memory/access, todo/mode tracking, compaction, looping, and observability.

This lab now uses the official Microsoft Agent Framework Harness directly.

## Required Code Evidence

The lab must compile with these exact evidence points:

```xml
<PackageReference Include="Microsoft.Agents.AI.Harness" Version="1.13.0-preview.260703.1" />
```

```csharp
AIAgent harnessAgent = chatClient.AsHarnessAgent(new HarnessAgentOptions
{
    // harness configuration
});
```

No scripted `IChatClient`, no manual `ChatClientAgent` composition, and no hand-built harness substitute is allowed in this lab.

## Official Components Used

| Harness Concern | Lab Implementation |
|---|---|
| Live model client | `AzureOpenAIClient` with `AzureCliCredential` |
| `IChatClient` bridge | `GetChatClient(config.ModelDeployment).AsIChatClient()` |
| Harness runtime | `AsHarnessAgent(new HarnessAgentOptions { ... })` |
| Tool calling | `AIFunctionFactory.Create(...)` tools passed through `ChatOptions.Tools` |
| Tool approval | `ToolApprovalAgentOptions` configured on `HarnessAgentOptions` |
| Looping | `LoopEvaluators = [new CompletionMarkerLoopEvaluator("DONE")]` |
| Compaction | `MaxContextWindowTokens`, `MaxOutputTokens`, `DisableCompaction = false` |
| Todo tracking | `DisableTodoProvider = false` so harness default provider is active |
| Mode tracking | `DisableAgentModeProvider = false` so harness default provider is active |
| File memory | `DisableFileMemory = false`, `FileMemoryStore = fileStore` |
| File access | `DisableFileAccess = false`, `FileAccessStore = fileStore` |
| Observability | `DisableOpenTelemetry = false`, `OpenTelemetrySourceName = "pronative.day03.lab05.harness"` |
| Evidence capture | `output/day03-lab05-harness-evidence.json` in the harness file store |

## Environment

```powershell
az login
$env:AZURE_OPENAI_ENDPOINT="https://proj-an2607101-default-resource.openai.azure.com/"
$env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
$env:BATCH_ID="AN-2607-101"
$env:STUDENT_ID="ST-2606-1000"
```

`AZURE_OPENAI_ENDPOINT` should be the Azure OpenAI-compatible endpoint root. If the value accidentally ends with `/openai/v1`, the lab trims that suffix before constructing `AzureOpenAIClient`.

## Run

```powershell
dotnet run --project .\src\Lab05HarnessEngineering\Lab05HarnessEngineering.csproj
```

## Review Checklist

- Confirm the build succeeds.
- Confirm the code contains `PackageReference Include="Microsoft.Agents.AI.Harness"`.
- Confirm the code constructs the agent with `AsHarnessAgent(new HarnessAgentOptions { ... })`.
- Confirm no `ScriptedHarnessChatClient` or manual `ChatClientAgent` composition exists in Lab 05.
- Confirm the final response includes `Evidence`, `Evaluation`, `Completion`, and ends with `DONE`.
- Confirm `output/day03-lab05-harness-evidence.json` is written under the harness file store.

## Trainer Notes

This lab intentionally disables hosted web search because the selected Azure OpenAI endpoint may not expose hosted web-search support. That does not reduce the harness lesson: the focus is the harness runtime, loop evaluators, compaction, tool approval, file memory/access, todo/mode providers, and OpenTelemetry.

If the live Azure model call fails, treat it as an environment/RBAC/model deployment issue. Do not replace the harness with a fake client.
