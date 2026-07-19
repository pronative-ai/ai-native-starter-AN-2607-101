# Lab 06 - Runpod vLLM Neocloud Runtime

## Use Case

Call a Runpod serverless vLLM endpoint using the OpenAI-compatible API and capture latency, cold-start behavior, token usage, and operational evidence for neocloud AI inference.

## What It Does

1. Loads Runpod configuration (endpoint ID, API key, model name)
2. Validates that all required Runpod variables are set
3. Starts an activity span (`ai.native.runpod.vllm.chat_completion`) with Runpod-specific tags
4. Constructs the OpenAI-compatible URL (`https://api.runpod.ai/v2/{ENDPOINT_ID}/openai/v1/chat/completions`)
5. Supports custom base URL via `RUNPOD_OPENAI_BASE_URL` environment variable
6. Supports custom prompt via `RUNPOD_PROMPT` environment variable
7. Sends the request with Bearer token auth, captures latency and response
8. Parses token usage and prints the model answer
9. Emits an `OperationalEvent` (`lab06.runpod_vllm_completed` / `lab06.runpod_vllm_failed`)
10. Documents trainer walkthrough topics: endpoint logs, cold start vs warm calls, max workers, GPU type, shutdown controls

## Key Concepts

- **Neocloud inference**: GPU compute provisioned on-demand via Runpod serverless
- **vLLM compatibility**: uses OpenAI-compatible chat completions API
- **Cold start**: first call after endpoint idle triggers worker spin-up (higher latency)
- **Warm calls**: subsequent calls benefit from already-running workers
- **Cost control**: max workers, active workers, timeout, GPU type, endpoint shutdown

## What You Learn

- How to call a Runpod vLLM endpoint using the OpenAI-compatible API
- How to configure Bearer token authentication for neocloud endpoints
- How to measure and compare cold-start vs warm-call latency
- How to inspect Runpod endpoint logs and worker lifecycle
- How to discuss neocloud cost controls: GPU type, worker limits, timeout, shutdown
- How to compare inference runtimes: Azure OpenAI (cloud) vs Foundry Local (on-device) vs Runpod (neocloud)

## Dependencies

- .NET 8
- Active Runpod account with a deployed vLLM endpoint

## Required Environment Variables

| Variable | Description |
|---|---|
| `RUNPOD_ENDPOINT_ID` | Runpod serverless endpoint ID |
| `RUNPOD_API_KEY` | Runpod API key for Bearer auth |
| `RUNPOD_MODEL` | Model name as expected by the vLLM endpoint |
| `RUNPOD_OPENAI_BASE_URL` | (Optional) Custom base URL |
| `RUNPOD_PROMPT` | (Optional) Custom prompt |
| `BATCH_ID`, `STUDENT_ID` | Batch identity |

## Run

```powershell
dotnet run --project src/Lab06RunpodNeocloudClient/Lab06RunpodNeocloudClient.csproj
```
