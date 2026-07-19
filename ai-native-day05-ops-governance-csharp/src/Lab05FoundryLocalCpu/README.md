# Lab 05 - Foundry Local CPU Walkthrough

## Use Case

Demonstrate the full lifecycle of running a local AI model using Microsoft Foundry Local for CPU inference. This lab discovers execution providers, downloads a model artifact, loads it into memory, performs streaming chat completion, and unloads the model — all on a local CPU device.

## What It Does

1. Loads batch identity from environment (uses its own simplified `LocalConfig` loader)
2. Creates a `FoundryLocalManager` instance with application configuration
3. Discovers available execution providers and downloads/registers any that are missing
4. Fetches the model catalog and retrieves a model by alias (default: `qwen2.5-0.5b`)
5. Downloads the model artifact with progress reporting
6. Loads the model into memory
7. Obtains a streaming chat client and sends a user prompt
8. Streams the response token by token to the console
9. Unloads the model from memory
10. Emits an operational event reporting the full lifecycle duration

## Key Lifecycle Stages

1. **Manager creation**: initialize Foundry Local runtime
2. **Execution provider discovery**: detect/register available compute backends (DirectML, CPU, etc.)
3. **Catalog query**: list available models by alias
4. **Model download**: download model artifacts (may be cached after first run)
5. **Model load**: load model into memory for inference
6. **Streaming inference**: send prompt, stream response tokens
7. **Model unload**: release model from memory

## What You Learn

- How to use the Microsoft.AI.Foundry.Local SDK for on-device inference
- How to manage the model lifecycle: discover, download, load, infer, unload
- How to perform streaming (token-by-token) chat completions
- How execution providers (DirectML, CPU) enable local inference
- How local model caching works across runs
- How to discuss hybrid runtime decisions (local vs cloud vs neocloud)
- How to handle lifecycle failures and fallback scenarios

## Dependencies

- .NET 8 (targets `net8.0-windows10.0.18362`)
- `Microsoft.AI.Foundry.Local`
- `Betalgo.Ranul.OpenAI`

## Required Environment Variables

| Variable | Description |
|---|---|
| `FOUNDRY_LOCAL_MODEL_ALIAS` | Model alias to use (default: `qwen2.5-0.5b`) |
| `BATCH_ID`, `STUDENT_ID`, `ENVIRONMENT_ID` | Batch identity |

## Run

```powershell
dotnet run --project src/Lab05FoundryLocalCpu/Lab05FoundryLocalCpu.csproj
```

First run will be slow as execution providers and model artifacts are downloaded.
