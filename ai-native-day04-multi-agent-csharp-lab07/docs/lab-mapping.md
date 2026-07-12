# Day 4 Lab Mapping

## Lab 01 - Multi-Agent Architecture Mapping

Project: `src/Lab01MultiAgentArchitecture`

Students compare native Microsoft Agent Framework sequential, concurrent, handoff, and group-chat workflows for the same ProNative enterprise training operations request.

## Lab 02 - Magentic-Style Coordinator-Worker Orchestration

Project: `src/Lab02CoordinatorWorkerAgents`

Students run a native Microsoft Agent Framework Magentic workflow. A manager agent plans and coordinates curriculum, protocol, gateway, and validation worker agents.

## Lab 03 - A2A Awareness / Agent Exposure

Project: `src/Lab03A2AAgentExposure`

Students expose an Agent Framework agent through the official A2A ASP.NET Core integration, inspect its Agent Card, and invoke the A2A message stream endpoint.

## Lab 04 - AG-UI and A2UI Concept Check

Project: `src/Lab04AgentUserInteractionBoundary`

Students inspect an official AG-UI .NET event stream, compare it with A2UI declarative UI payloads, and trace how a frontend approval becomes `AGUIResume` for backend-controlled tool execution.

## Lab 05 - MCP vs UTCP Tool Boundary

Project: `src/Lab05ProtocolToolBoundary`

Students compare an MCP tool-server boundary using the official `ModelContextProtocol` C# package with a UTCP direct native HTTP endpoint boundary represented by an official UTCP manual shape.

## Lab 06 - AgentGateway Baseline

Project: `src/Lab06AgentGatewayClient`

Students inspect an official AgentGateway standalone configuration and use a C# verification client to prepare or send model, MCP, and A2A requests through the gateway. The lab covers listeners, routes, backends, request header attribution, local rate limiting, managed identity backend auth, and tracing.

## Lab 07 - Gateway Observability and Control

Project: `src/Lab07GatewayObservabilityControl`

Students generate controlled gateway traffic, capture request/correlation/trace identifiers, exercise rate-limit behavior when the stricter Lab 07 route is deployed, and inspect gateway evidence through Azure Container Apps logs, Azure Monitor, and App Insights/Log Analytics KQL.
