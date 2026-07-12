# Day 4 Lab 04 - AG-UI / A2UI Agent-User Interaction Boundary

This lab shows how an enterprise agent communicates with a user-facing app without letting the agent generate arbitrary frontend code.

The lab is intentionally compact:

- AG-UI is used through the official .NET protocol package.
- A2UI is represented as declarative protocol JSON, because the starter pack does not include an official .NET A2UI SDK package.
- The lab does not build a full frontend. It teaches the contract that a frontend would consume.

## Component Contract

- Official capability: AG-UI event stream and interrupt/resume flow for agent-user interaction.
- Package: `AGUI.Abstractions` 0.0.3.
- Required classes/methods:
  - `RunAgentInput`
  - `AGUIUserMessage`
  - `AGUITool`
  - `RunStartedEvent`
  - `TextMessageStartEvent`
  - `TextMessageContentEvent`
  - `TextMessageEndEvent`
  - `ToolCallStartEvent`
  - `ToolCallArgsEvent`
  - `ToolCallEndEvent`
  - `StateSnapshotEvent`
  - `RunFinishedEvent`
  - `RunFinishedInterruptOutcome`
  - `AGUIInterrupt`
  - `AGUIResume`
  - `AGUIToolApprovalResumePayload`
  - `ToolCallResultEvent`
  - `RunFinishedSuccessOutcome`
  - `AGUIJsonSerializerContext`
- Required code evidence:
  - `PackageReference Include="AGUI.Abstractions" Version="0.0.3"`
  - `RunFinishedEvent` with `RunFinishedInterruptOutcome`
  - `AGUIResume` carrying `AGUIToolApprovalResumePayload`
  - A2UI `createSurface`, `updateComponents`, `updateDataModel`, and `action` payloads.
- Forbidden substitutes:
  - No custom AG-UI event records as the main path.
  - No homemade interrupt/resume model as the main path.
  - No generated executable frontend code.
  - No claim that A2UI has a .NET SDK in this starter pack.
- Build acceptance: `dotnet build src/Lab04AgentUserInteractionBoundary/Lab04AgentUserInteractionBoundary.csproj`.

## What Students Should Learn

| Concept | Meaning |
|---|---|
| AG-UI | Event transport between agent runtime and user interface |
| A2UI | Declarative UI payload that asks the app to render approved components |
| Interrupt | Agent pauses because a human or application decision is required |
| Resume | The UI sends an approval/rejection result back to the agent runtime |
| Trust boundary | The agent sends UI intent and data, while the app controls what can render and what tool action is allowed |

## Run

```powershell
cd outputs\starter-repositories\ai-native-day04-multi-agent-csharp
dotnet run --project .\src\Lab04AgentUserInteractionBoundary\Lab04AgentUserInteractionBoundary.csproj
```

## Walkthrough

1. Review the `RunAgentInput`.
   - It contains the user message, available tool metadata, state, and context.
   - The tool metadata marks the environment exception tool as approval-gated.

2. Review the AG-UI event stream.
   - `RunStartedEvent` starts the run.
   - `TextMessage*Event` streams assistant text to the UI.
   - `ToolCall*Event` shows the tool request.
   - `StateSnapshotEvent` lets the UI understand the current state.
   - `RunFinishedEvent` ends with `RunFinishedInterruptOutcome`, not success, because approval is required.

3. Review the A2UI payloads.
   - `createSurface` creates a UI surface for approval.
   - `updateComponents` requests a safe approval card made from approved component names.
   - `updateDataModel` supplies the data the approved UI components render.
   - `action` represents the user clicking Approve.

4. Review the resume flow.
   - `AGUIResume` carries the approval result back to the runtime.
   - `AGUIToolApprovalResumePayload` connects the approval decision to the original tool call.
   - The resumed run emits `ToolCallResultEvent` and then `RunFinishedSuccessOutcome`.

## Trainer Notes

- Use this lab before discussing frontend-heavy agent experiences.
- Emphasize that AG-UI is about event transport and interaction lifecycle.
- Emphasize that A2UI is about safe declarative UI rendering.
- Connect this to Day 3 human-in-the-loop approval: approval should remain a backend policy decision, not only a frontend button.
- Connect this to Day 4 AgentGateway: gateway logs and policies should include the same `BatchId`, `StudentId`, `ToolCallId`, and correlation identifiers shown here.

## References

- AG-UI overview: https://docs.ag-ui.com/introduction
- AG-UI .NET SDK abstractions: https://docs.ag-ui.com/sdk/dotnet/abstractions/overview
- AG-UI events: https://docs.ag-ui.com/concepts/events
- AG-UI interrupts: https://docs.ag-ui.com/concepts/interrupts
- A2UI overview: https://a2ui.org/
- A2UI protocol v1.0: https://a2ui.org/specification/v1.0-a2ui/
