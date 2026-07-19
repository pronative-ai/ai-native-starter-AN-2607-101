# AgentGateway

Local Docker Compose stack for running [agentgateway](https://agentgateway.dev) with [Langfuse](https://langfuse.com) observability and shared backing services.

## Services

| Service | Port | Purpose |
|---------|------|---------|
| HTTP bind | `:80` | Health check / direct response endpoint |
| MCP gateway | `:3000` | MCP proxy (Microsoft Learn MCP target) |
| LLM gateway | `:4000` | OpenAI-compatible chat completions proxy (Azure OpenAI backend) |
| Admin UI | `:15000` | Gateway config, cost dashboard, request logs |
| Langfuse | `:3001` | LLM tracing, cost tracking, prompt management |
| PostgreSQL | `:5432` | Shared database for Langfuse + agentgateway access logs |
| ClickHouse | `:8123` | Langfuse analytics engine |
| Redis | `:6379` | Langfuse job queue |
| MinIO | `:9090` | Langfuse object storage (console at `:9091`) |

## Quick start

```powershell
cd agentgateway
Copy-Item .env.example .env
# Edit .env and set at minimum AZURE_OPENAI_API_KEY and AGENTGATEWAY_API_KEY
docker compose up -d
```

## Gateway ports

- **`http://localhost:80`** — HTTP health check (returns "Agent Gateway proxy is working!")
- **`http://localhost:4000/v1/chat/completions`** — LLM chat completions (requires `Authorization: Bearer <AGENTGATEWAY_API_KEY>`)
- **`http://localhost:15000/ui`** — Admin UI
- **`http://localhost:3000`** — MCP gateway (Microsoft Learn)

## First-time Langfuse setup

1. Open `http://localhost:3001`
2. Create an admin account
3. Create a project
4. Copy the **Public Key** and **Secret Key** into a `.env` file or use the Langfuse UI

## Routing Lab 03 traffic through the gateway

Set these environment variables for Lab 03:

```powershell
$env:AGENTGATEWAY_ENDPOINT="http://localhost:4000"
$env:AGENTGATEWAY_ROUTE="/v1/chat/completions"
$env:AGENTGATEWAY_BEARER_TOKEN="<your-AGENTGATEWAY_API_KEY-value>"
```

Then run Lab 03:

```powershell
dotnet run --project ..\src\Lab03AgentGatewayOperationalClient\Lab03AgentGatewayOperationalClient.csproj
```

Requests will appear in:
- **agentgateway admin UI** (`http://localhost:15000/ui`) — request logs, cost dashboard
- **Langfuse** (`http://localhost:3001`) — traces with latency, token usage, cost attribution

## Stopping

```powershell
docker compose down
docker compose down -v  # remove volumes (data loss)
```
