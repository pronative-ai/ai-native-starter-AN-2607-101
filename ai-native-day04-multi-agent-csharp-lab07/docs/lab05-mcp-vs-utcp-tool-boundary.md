# Day 4 Lab 05 - MCP vs UTCP Tool Boundary

This lab helps students decide when a tool should be exposed through an MCP server and when an agent can call an existing API directly through UTCP.

The lab is intentionally not another deep MCP build, because Day 2 already covered MCP integration. Day 4 focuses on the enterprise architecture decision.

## Component Contract

- Official capability: MCP server tool boundary and UTCP direct native endpoint boundary.
- MCP package: `ModelContextProtocol` 1.4.0.
- MCP required classes/methods:
  - `[McpServerToolType]`
  - `[McpServerTool]`
  - `McpServerTool.Create(...)`
  - `McpServerToolCreateOptions`
  - `McpServerTool.ProtocolTool`
  - `Tool.Name`
  - `Tool.Description`
  - `Tool.InputSchema`
- MCP hosting APIs shown for real server deployment:
  - `AddMcpServer()`
  - `WithStdioServerTransport()`
  - `WithToolsFromAssembly()`
- MCP client APIs referenced for real client use:
  - `McpClient.CreateAsync(...)`
  - `ListToolsAsync()`
  - `CallToolAsync(...)`
- UTCP required protocol evidence:
  - `manual_version`
  - `utcp_version`
  - `tools`
  - `inputs`
  - `tool_call_template`
  - `call_template_type: http`
  - `url`
  - `http_method`
  - `auth`
- Required code evidence:
  - `PackageReference Include="ModelContextProtocol" Version="1.4.0"`
  - MCP tool metadata generated through `McpServerTool.Create(...)`
  - UTCP manual-generated native `HttpRequestMessage`
- Forbidden substitutes:
  - No custom MCP-like protocol.
  - No invented UTCP C# SDK.
  - No fake MCP event/request names.
  - No wrapper server for the UTCP path.
- Build acceptance: `dotnet build src/Lab05ProtocolToolBoundary/Lab05ProtocolToolBoundary.csproj`.

## What Students Should Learn

| Concept | Meaning |
|---|---|
| MCP | A tool-server boundary. The agent/host discovers and calls tools through an MCP server. |
| UTCP | A direct native endpoint boundary. A manual tells the agent how to call the existing API. |
| Wrapper server | MCP is useful when tool logic, normalization, secrets, or governance should sit behind a server. |
| Native API reuse | UTCP is useful when the API is already secure, observable, and automation-safe. |
| Gateway control | Both MCP and UTCP traffic can still be routed through AgentGateway for rate limits, attribution, and policy. |

## Run

```powershell
cd outputs\starter-repositories\ai-native-day04-multi-agent-csharp
dotnet run --project .\src\Lab05ProtocolToolBoundary\Lab05ProtocolToolBoundary.csproj
```

## Walkthrough

1. Review the MCP tool class.
   - `TrainingEnvironmentMcpTools` is annotated with `[McpServerToolType]`.
   - `RaiseTrainingException(...)` is annotated with `[McpServerTool]`.
   - Parameter descriptions become part of the generated tool schema.

2. Review the MCP metadata.
   - `McpServerTool.Create(...)` generates protocol-level tool metadata.
   - `ProtocolTool.Name`, `ProtocolTool.Description`, and `ProtocolTool.InputSchema` are the shape a host can discover.

3. Review the MCP call shape.
   - The lab prints a `tools/call` JSON-RPC request.
   - The call goes to an MCP server, not directly to the business API.

4. Review the UTCP manual.
   - The manual describes an existing API.
   - `tool_call_template` contains the native HTTP route and method.
   - `auth` explains where the existing API credential belongs.

5. Review the native HTTP request.
   - The lab builds an `HttpRequestMessage` from the UTCP manual.
   - No tool wrapper server is introduced.
   - Existing API auth, logging, and gateway policy can still apply.

## Trainer Notes

- Ask students: "Where should the control point live?"
- If the answer is "in a tool server," MCP is usually a better fit.
- If the answer is "in the existing API and gateway," UTCP is usually a better fit.
- For ProNative training, MCP is a good fit for shared tool servers and curated tool catalogs.
- UTCP is a good fit for existing Azure/internal REST APIs where direct HTTP with Entra/API-key auth is already governed.
- AgentGateway remains relevant to both paths because it can apply route policies, rate limits, cost tags, and trace correlation.

## Enterprise Decision Heuristic

| Scenario | Prefer | Reason |
|---|---|---|
| Existing MCP server already available | MCP | Reuse standard discovery and host approval. |
| Existing REST API already production-ready | UTCP | Avoid wrapper tax and preserve native auth/audit. |
| Tool needs secret handling and backend composition | MCP | Keep secrets and composition server-side. |
| Tool is a simple safe direct API call | UTCP | Keep direct endpoint semantics. |
| Shared training environment requires governance | Either through AgentGateway | Apply route, identity, rate, and cost controls consistently. |

## References

- MCP C# SDK overview: https://csharp.sdk.modelcontextprotocol.io/
- MCP C# SDK getting started: https://csharp.sdk.modelcontextprotocol.io/concepts/getting-started.html
- ModelContextProtocol C# SDK repository: https://github.com/modelcontextprotocol/csharp-sdk
- UTCP introduction: https://www.utcp.io/
- UTCP GitHub organization: https://github.com/universal-tool-calling-protocol
