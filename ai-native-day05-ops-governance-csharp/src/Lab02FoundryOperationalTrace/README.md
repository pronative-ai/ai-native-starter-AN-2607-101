# Lab 02 - Foundry Operational Troubleshooting and Run Evidence

## Use Case

Demonstrate operational troubleshooting for Azure OpenAI / Foundry chat completions. This lab captures latency, token usage, response headers (request IDs, trace IDs, APIM headers), and emits an operational event with success/failure evidence.

## What It Does

1. Loads config and validates authentication credentials
2. Starts an activity span (`ai.native.foundry.operational_trace`) with batch/student tags
3. Sends a chat completion request with a troubleshooting-focused prompt (configurable via `LAB02_PROMPT`)
4. Inspects response headers containing request IDs, trace IDs, and APIM correlation headers
5. Parses token usage from the response and tags the activity
6. Emits an `OperationalEvent` as JSON (`lab02.foundry_chat_completed` / `lab02.foundry_chat_failed`)
7. Documents three troubleshooting modes:
   - Point to a non-existent deployment to observe deployment errors
   - Remove auth to observe 401/403 readiness failures
   - Increase prompt size or run concurrent calls to discuss latency and throttling

## Key Evidence Captured

- **Response headers**: `x-request-id`, `x-ms-request-id`, `apim-request-id`, trace headers
- **Token usage**: prompt, completion, total tokens
- **Performance**: latency in milliseconds
- **Operational event**: structured JSON with batch/student identity, model deployment, backend, latency, success flag, error details
- **Troubleshooting modes**: intentional failure scenarios for trainer walkthrough

## What You Learn

- How to make direct Foundry/Azure OpenAI chat completion calls with proper auth
- How to inspect and surface API response headers for troubleshooting and correlation
- How to extract and report token usage metrics
- How to observe and diagnose common failure modes (wrong deployment, missing auth, throttling)
- How to build operational run evidence for AI model calls

## Dependencies

- .NET 8
- Azure OpenAI deployment

## Required Environment Variables

| Variable | Description |
|---|---|
| `AZURE_OPENAI_ENDPOINT` | Azure OpenAI endpoint URL |
| `AZURE_OPENAI_CHAT_DEPLOYMENT` | Deployment name |
| `AZURE_OPENAI_API_KEY` or `AZURE_OPENAI_BEARER_TOKEN` | Authentication credential |
| `BATCH_ID`, `STUDENT_ID` | Batch identity |
| `LAB02_PROMPT` | (Optional) Custom troubleshooting prompt |

## Run

```powershell
dotnet run --project src/Lab02FoundryOperationalTrace/Lab02FoundryOperationalTrace.csproj
```
