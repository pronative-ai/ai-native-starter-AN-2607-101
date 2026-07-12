# Lab 05 - MCP vs UTCP Tool Boundary

## Use Case

This lab compares MCP (Model Context Protocol) and UTCP (Universal Tool Calling Protocol) for integrating tools with AI agents.

Key concepts:

- **MCP** - Tool server protocol; tools live behind a server boundary
- **UTCP** - Direct API/tool calling style; existing APIs invoked directly
- **Decision Matrix** - When to use MCP vs UTCP vs either through AgentGateway

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- No Azure authentication required (local simulation)

### Steps

1. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab05ProtocolToolBoundary\Lab05ProtocolToolBoundary.csproj
   ```

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| ModelContextProtocol NuGet | MCP SDK for tool metadata |

## Sample Input

No user input required. The lab demonstrates MCP and UTCP tool shapes.

## Expected Output

```
1. MCP boundary - tool is exposed through an MCP server
======================================================
Official SDK evidence in this lab:
- PackageReference Include="ModelContextProtocol" Version="1.4.0"
- [McpServerToolType] and [McpServerTool] annotate the tool class and method.
- McpServerTool.Create(...) creates the MCP server tool metadata.
- McpServerTool.ProtocolTool exposes the protocol-level Tool shape.

MCP protocol tool metadata produced by the official SDK:
{
  "Name": "raise_training_exception",
  "Description": "Creates a controlled Day 4 training exception request...",
  "InputSchema": {...}
}

MCP JSON-RPC call shape a client would send to an MCP server:
{
  "jsonrpc": "2.0",
  "method": "tools/call",
  "params": {
    "name": "raise_training_exception",
    "arguments": {"studentId": "ST-2606-1004", ...}
  }
}

2. UTCP boundary - tool is an existing native endpoint
======================================================
UTCP manual exposed by the existing API owner:
{
  "manual_version": "1.0.0",
  "tools": [{
    "name": "raise_training_exception",
    "tool_call_template": {
      "call_template_type": "http",
      "url": "https://training-ops.pronative.ai/api/training-exceptions",
      "http_method": "POST"
    }
  }]
}

Native HTTP request generated from the UTCP manual:
{
  "Method": "POST",
  "Url": "https://training-ops.pronative.ai/api/training-exceptions",
  "Headers": {"x-api-key": "${PRONATIVE_TRAINING_OPS_API_KEY}"},
  "Body": {"studentId": "ST-2606-1004", ...}
}

3. Enterprise decision matrix
=============================
Scenario: GitHub, Azure, enterprise systems already exposed as MCP servers
Prefer:   MCP
Why:      Use standardized tool discovery, host approval, and tool-server governance.

Scenario: Existing internal REST API with mature auth, logging, and billing
Prefer:   UTCP
Why:      Keep native endpoint, auth, audit, and latency profile.

Scenario: Tool requires server-side aggregation, secrets, or multi-step normalization
Prefer:   MCP
Why:      A tool server can own secret handling and compose multiple backend calls safely.

Scenario: Simple direct API call already safe for automation
Prefer:   UTCP
Why:      The agent can call the API directly using the existing protocol and security model.
```

## Key Learning Points

1. **MCP** - Tool server boundary; agent discovers tools from server; server owns secrets
2. **UTCP** - Direct API call; existing endpoint; no wrapper server needed
3. **Decision factors** - Existing infrastructure, secrets, aggregation needs, governance
4. **AgentGateway** - Both MCP and UTCP calls can route through gateway for policy/logs

## When to Use Each Protocol

| Protocol | Best When |
|----------|-----------|
| MCP | Tools already exposed as MCP servers; need standardized discovery; secrets/composition required |
| UTCP | Existing REST API with mature auth/logging; simple direct calls; no wrapper needed |
| Either (via Gateway) | Need rate limits, tags, observability, cost attribution |

## Troubleshooting

| Problem | Solution |
|---------|----------|
| MCP tool not found | Verify `[McpServerTool]` attribute on method |
| Build error | Check ModelContextProtocol package version |

## Reference

- [MCP Documentation](https://modelcontextprotocol.io/docs/getting-started/intro)
