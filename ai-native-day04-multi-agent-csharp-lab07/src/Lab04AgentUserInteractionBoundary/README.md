# Lab 04 - AG-UI / A2UI Agent-User Interaction Boundary

## Use Case

This lab demonstrates agent-user interaction boundaries using AG-UI (event-driven runtime) and A2UI (declarative UI payloads).

Key concepts:

- **AG-UI Events** - Event stream from agent runtime to UI (RunStarted, ToolCall, Interrupt, Resume)
- **A2UI Payloads** - Declarative UI components (createSurface, updateComponents, updateDataModel)
- **Interrupt/Resume** - Approval workflow where agent pauses for user decision, then continues

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- No Azure authentication required (local simulation)

### Steps

1. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab04AgentUserInteractionBoundary\Lab04AgentUserInteractionBoundary.csproj
   ```

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |

## Sample Input

No user input required. The lab simulates a training environment exception approval workflow.

## Expected Output

```
1. AG-UI event stream from agent runtime to UI
================================================
--- RunStartedEvent ---
{"type":"run_started","threadId":"thread-an2607101-day04-lab04","runId":"run-approval-001",...}

--- TextMessageStartEvent ---
{"type":"text_message_start","messageId":"msg-agent-001","role":"assistant",...}

--- TextMessageContentEvent ---
{"type":"text_message_content","delta":"I found a quota-sensitive environment change...",...}

--- ToolCallStartEvent ---
{"type":"tool_call_start","toolCallId":"toolcall-raise-training-exception-001",...}

--- RunFinishedEvent (Interrupt) ---
{"type":"run_finished","outcome":{"type":"interrupt","interrupts":[{"id":"interrupt-tool-approval-001",...}]}}

2. A2UI declarative UI payloads carried to the frontend
=======================================================
--- createSurface ---
{"jsonrpc":"2.0","method":"createSurface","params":{"surfaceId":"training-exception-approval",...}}

--- updateComponents ---
{"jsonrpc":"2.0","method":"updateComponents","params":{"components":[{"type":"Card","props":{"title":"Approve quota-sensitive action"}}]}}

--- updateDataModel ---
{"jsonrpc":"2.0","method":"updateDataModel","params":{"data":{"approvalStatus":"pending"}}}

3. User action becomes AG-UI resume input
=========================================
--- AG-UI RunAgentInput.Resume ---
{"resume":[{"interruptId":"interrupt-tool-approval-001","status":"approved","payload":{...}}]}

4. Agent runtime continues after approval
=========================================
--- ToolCallResultEvent ---
{"type":"tool_call_result","content":"Exception recorded with policy tag trainer-approved..."}

--- RunFinishedEvent ---
{"type":"run_finished","outcome":{"type":"success"},...}
```

## Key Learning Points

1. **AG-UI** - Event transport between agent runtime and UI
2. **A2UI** - Declarative UI payload; agent requests approved components, not arbitrary code
3. **Interrupt** - Agent pauses execution for user approval
4. **Resume** - User decision crosses back as `AGUIResume`; backend tool execution stays governed
5. **State Snapshot** - UI can render approval controls based on agent state

## Comparison: A2A vs AG-UI vs A2UI

| Boundary | Focus |
|----------|-------|
| A2A | Agent-to-agent discovery and messaging |
| AG-UI | Event-driven agent-to-UI runtime interaction |
| A2UI | Declarative/adaptive agent-to-UI payload concept |

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Build error | Ensure AGUI NuGet package is restored |
| Events not printing | Check `AGUIJsonSerializerContext` source generation |

## Reference

- [AG-UI Documentation](https://docs.ag-ui.com/introduction)
