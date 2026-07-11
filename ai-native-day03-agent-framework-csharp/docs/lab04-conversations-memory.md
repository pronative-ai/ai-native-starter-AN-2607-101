# Lab 04 - Conversations, Session, Context, Storage, and Compaction

## Purpose

This lab teaches state and memory using Microsoft Agent Framework conversation primitives rather than a custom memory framework.

The lab covers:

- `AgentSession` as the conversation continuity boundary.
- `AgentSession.StateBag` for serializable task/session metadata.
- `AIContextProvider` for session-scoped context injection and memory extraction.
- `ProviderSessionState<T>` for typed provider state stored inside the session.
- `InMemoryChatHistoryProvider` for framework-managed local chat history.
- `MessageCountingChatReducer` to bound the amount of history sent back into the model call.
- `SerializeSessionAsync(...)` and `DeserializeSessionAsync(...)` for restart-safe checkpoints.
- `CompactionProvider`, `SlidingWindowCompactionStrategy`, `TruncationCompactionStrategy`, and `PipelineCompactionStrategy`.

## Scenario

The learner is preparing a Day 3 Agent Framework environment and wants the agent to remember:

- preferred language: C#
- current issue: Azure AI Search indexing
- current task: prepare Day 3 learner environment

The context provider extracts those facts from user messages, stores them in the session, and injects them into later turns.

## Why The Lab Uses A Local Chat Client

The lab uses a tiny deterministic `IChatClient` only to avoid model cost while demonstrating the conversation pipeline.

It is not a custom agent framework, memory implementation, or storage abstraction. The important capabilities are all official Agent Framework capabilities:

- `ChatClientAgentOptions`
- `AIContextProvider`
- `InMemoryChatHistoryProvider`
- `AgentSession`
- `CompactionProvider`

In production, replace the local chat client with a Foundry-backed chat client or Foundry agent, but keep the same session/provider/storage principles.

## Run

```powershell
dotnet run --project .\src\Lab04ConversationsMemory\Lab04ConversationsMemory.csproj
```

## Walkthrough

### Step 1 - Session

The lab creates one `AgentSession` and stores batch, student, task, and status values in `StateBag`.

This demonstrates the session as a serializable checkpoint boundary. Do not store secrets in this state.

### Step 2 - Context Provider

`TrainingPreferenceContextProvider` uses `ProviderSessionState<PreferenceState>`.

Before a run, it injects remembered facts as context. After a run, it extracts known facts from the latest user messages and saves them back into the session.

The provider does not store session-specific data in its own fields.

### Step 3 - Storage

`InMemoryChatHistoryProvider` stores chat history in the session under `day03-lab04-chat-history`.

`MessageCountingChatReducer(8)` keeps history bounded before retrieval, which is the default cost-control behavior students should understand before moving to external stores such as Cosmos DB.

### Step 4 - Serialization

The session is serialized to:

`bin\Debug\net10.0\artifacts\<student-id>-lab04-session.json`

The lab then deserializes the same session and continues the conversation.

### Step 5 - Compaction

The explicit compaction demo builds a longer history list and runs:

- `SlidingWindowCompactionStrategy`
- `TruncationCompactionStrategy`
- `PipelineCompactionStrategy`

This shows how message groups are reduced while preserving recent context.

## Enterprise Upgrade Path

For the live Azure architecture:

- Keep `AgentSession` as the local conversation/checkpoint boundary.
- Store serialized sessions in Cosmos DB using partition keys such as `BatchId`, `StudentId`, and `TaskId`.
- Use Azure Container Apps with Dapr state management when the agent needs actor-style, long-running task checkpoints.
- Use compaction only for framework-managed in-memory history. For service-managed Foundry Agents, server-side context is managed by the service.

## Trainer Checkpoints

Students should be able to explain:

1. Why session is the unit of continuity.
2. Why context providers store session-specific state in `AgentSession.StateBag`.
3. Difference between local history and service-managed conversation state.
4. Why serialized sessions must be treated as sensitive data.
5. Where compaction helps and where it does not.
