# Lab 01 - LLMOps and GenAIOps Observability Baseline

## Use Case

Establish an operational observability baseline for AI agent calls to Azure OpenAI. This lab instruments a live Foundry chat completion request with OpenTelemetry spans, attaches batch/student/trace metadata as custom tags, and exports the telemetry to both the console and Azure Monitor (Application Insights).

## What It Does

1. Loads environment config (batch ID, student ID, Azure OpenAI endpoint, API key)
2. Configures an OpenTelemetry `TracerProvider` with Console exporter and optional Azure Monitor exporter
3. Starts a root activity span (`ai.native.llmops.genaiops.live_foundry_request`) with AI-specific tags (`ai.system`, `ai.operation.type`, `ai.model.deployment`)
4. Sends a chat completion request to Azure OpenAI's `/chat/completions` endpoint
5. Creates a child span (`foundry.chat.completions`) capturing server address, model deployment, HTTP status, latency, and token usage
6. Parses the response to extract prompt/completion/total token counts and tags them on both spans
7. Prints the model answer and emits an `OperationalEvent` JSON record
8. On failure, captures error details on the spans and emits a failed operational event

## Key Telemetry Captured

- **Service identity**: `service.name = day05-lab01-llmops-genaiops-observability`
- **Trace context**: `traceparent` header for distributed tracing correlation
- **Custom dimensions**: `BatchId`, `StudentId`, `EnvironmentId`, `CostCenter`, `Owner`
- **AI-specific tags**: `ai.system`, `ai.operation.type`, `ai.model.deployment`, `ai.prompt.tokens`, `ai.completion.tokens`, `ai.total.tokens`
- **Performance**: `http.response.status_code`, `ai.latency_ms`
- **Operational event**: `lab01.live_foundry_observability_completed` / `lab01.live_foundry_observability_failed`

## What You Learn

- How to instrument a .NET application with OpenTelemetry for AI workloads
- How to configure OpenTelemetry to export traces to both console and Azure Monitor
- How to attach custom semantic conventions (batch/student identity, cost center, AI model metadata) to activities
- How to propagate trace context via HTTP headers for end-to-end correlation
- How to extract token usage from OpenAI-compatible responses and record it as telemetry
- How to verify evidence in Application Insights and Log Analytics using KQL queries

## Dependencies

- .NET 8
- `Azure.Monitor.OpenTelemetry.Exporter`
- `OpenTelemetry`, `OpenTelemetry.Extensions.Hosting`
- Azure OpenAI deployment (`gpt-5-mini` or equivalent)

## Required Environment Variables

| Variable | Description |
|---|---|
| `AZURE_OPENAI_ENDPOINT` | Azure OpenAI endpoint URL |
| `AZURE_OPENAI_CHAT_DEPLOYMENT` | Deployment name (e.g., `gpt-5-mini`) |
| `AZURE_OPENAI_API_KEY` or `AZURE_OPENAI_BEARER_TOKEN` | Authentication credential |
| `BATCH_ID`, `STUDENT_ID`, `ENVIRONMENT_ID` | Batch identity for trace correlation |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | (Optional) Application Insights ingestion endpoint |

## Run

```powershell
dotnet run --project src/Lab01ObservabilityTelemetry/Lab01ObservabilityTelemetry.csproj
```
