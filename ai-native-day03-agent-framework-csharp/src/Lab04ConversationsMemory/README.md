# Lab 04 - Conversations, State, and Memory

## Use Case

This lab demonstrates Microsoft Agent Framework conversation primitives for managing session continuity, context injection, chat history storage, serialization, and compaction.

Key concepts covered:

- **AgentSession** - Unit of conversation continuity
- **AIContextProvider** - Injects session-scoped memory/context before LLM calls
- **InMemoryChatHistoryProvider** - Stores conversation history within the session
- **Session serialization** - Creates restart-safe JSON checkpoints
- **Cosmos DB persistence** - Durable session checkpoint storage (optional)
- **Compaction** - Controls context growth with sliding window and truncation strategies

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- Azure CLI (for optional Cosmos DB persistence)

### Steps

1. **Run local session path:**
   ```powershell
   dotnet run --project .\src\Lab04ConversationsMemory\Lab04ConversationsMemory.csproj
   ```

2. **Optional: Enable Cosmos DB checkpointing:**
   ```powershell
   $env:ENABLE_COSMOS_PERSISTENCE="true"
   $env:COSMOS_SESSION_CONTAINER="agent-session-checkpoints"
   dotnet run --project .\src\Lab04ConversationsMemory\Lab04ConversationsMemory.csproj
   ```

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| Azure CLI | Authentication (for Cosmos DB path only) |
| Cosmos DB | Durable checkpoint storage (optional) |

## Sample Input

No user input required. The lab runs a multi-turn conversation automatically:

```
Turn 1: "Remember that I prefer C# examples..."
Turn 2: "My task is to prepare the Day 3 learner environment..."
Turn 3: "What should you remember before giving me the next action?"
Turn 4: "Continue from the checkpoint..."
```

## Expected Output

```
Step 1 - Create a session
=========================
BatchId:    AN-2607-101
StudentId:  ST-2606-1000
TaskId:     ST-2606-1000-environment-remediation
TaskStatus: open

Step 2 - Run multiple turns on the same AgentSession
===================================================
User:      Remember that I prefer C# examples...
Assistant: I have received the latest request...

Step 3 - Inspect local in-memory chat history
============================================
History messages stored in AgentSession: 6
- User: Remember that I prefer C# examples...
- Assistant: I have received the latest request...

Step 4 - Serialize and restore the session
==========================================
Serialized checkpoint written to: .../artifacts/ST-2606-1000-lab04-session.json

Step 7 - Run explicit compaction over a long message list
========================================================
Before compaction: 13 messages
After compaction:  8 messages
```

## Key Learning Points

1. **Session continuity** - `AgentSession` maintains state across multiple turns
2. **Context providers** - Inject remembered facts into LLM calls via `StateBag`
3. **Chat history** - Stored in session with optional message count reduction
4. **Serialization** - Creates JSON checkpoint for restart/recovery
5. **Cosmos DB** - Stores durable checkpoint; doesn't replace framework primitives
6. **Compaction** - Reduces context growth for long conversations

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Cosmos checkpoint fails | Check `ENABLE_COSMOS_PERSISTENCE`, `COSMOS_SESSION_CONTAINER`, and RBAC |
| No in-memory history found | Verify history provider state key matches |
| Compaction doesn't trigger | Check message count exceeds threshold (10 messages) |

## Reference

- [Agent Framework Conversations](https://learn.microsoft.com/en-us/agent-framework/agents/conversations/?pivots=programming-language-csharp)
