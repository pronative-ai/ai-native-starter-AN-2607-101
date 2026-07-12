# Day 03 Expanded Student Reference - Agent Framework Labs

Program: ProNative AI-Native Fullstack Engineering  
Day: 03  
Theme: Building controlled, stateful, skill-aware, grounded agentic applications with C# and Microsoft Agent Framework

## How To Use This Reference

Use this file after or during the live session to understand what each lab is trying to teach.

Each lab is explained using the same structure:

1. Concepts and sub-topics
2. Use cases by sub-topic
3. Lab objectives and coverage
4. Flow of the lab from starting point to classes/functions/methods and what each step achieves

The exact class names in your starter repository may differ slightly, but the patterns are the same.

---

# Lab 01 - Agentic AI Reasoning Loop

## 1. Concept

Lab 01 introduces the basic execution lifecycle of an agentic application.

The lifecycle is:

```text
goal
  -> plan
  -> action
  -> observation
  -> reflection
  -> checkpoint
```

This is different from a simple chat completion.

In a simple chat app:

```text
user prompt
  -> model response
```

In an agentic application:

```text
user goal
  -> agent decides what needs to happen
  -> tools may be called
  -> tool results become observations
  -> agent reflects on those observations
  -> state/evidence may be checkpointed
```

The agent is not just producing text. It is participating in a controlled execution loop.

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Goal | The objective the user wants to achieve | "Validate my setup", "Explain this error", "Generate a migration plan" |
| Plan | The agent's internal or explicit strategy | Multi-step troubleshooting, task decomposition, lab guidance |
| Action | A model-selected tool/function/workflow step | Validate input, call API, inspect environment, query data |
| Observation | Evidence returned from an action | Tool output, validation result, API response, file metadata |
| Reflection | Agent checks whether the goal is satisfied | Retry, revise answer, ask for missing input, stop safely |
| Checkpoint | Saved state/evidence from the run | Resume later, audit, debugging, student progress tracking |
| Tool approval | Human or policy gate before sensitive actions | HITL approval, safe command execution, trust boundary |
| Middleware | Cross-cutting logic around model/tool calls | Logging, telemetry, validation, policy enforcement |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- how an agent run differs from a simple model call
- why tool use creates a trust boundary
- how middleware can observe and govern agent behavior
- how reflection lets the agent decide whether more work is needed
- how checkpoint metadata gives the run an audit trail

The lab may cover:

```text
- creating an Agent Framework agent
- defining C# functions as tools
- asking the model to plan and act
- adding approval around tool calls
- logging tool/model activity
- showing goal/plan/action/observation/reflection output
- storing session checkpoint metadata
```

## 4. Lab Flow Walkthrough

### Step 1 - Program Starts

Starting point:

```text
Program.cs
```

The app usually:

```text
1. Reads environment variables.
2. Creates the Foundry/OpenAI client.
3. Creates the agent.
4. Registers tools/functions.
5. Adds middleware or approval behavior.
6. Starts the agent run.
```

What this achieves:

```text
The agent is connected to a model deployment and has access to controlled tools.
```

### Step 2 - Tool Functions Are Registered

Look for methods like:

```csharp
static string ValidateTrainingGoal(string goal)
{
    return $"Goal validated: {goal}";
}

static string InspectSetupBoundary()
{
    return "Setup boundary inspected.";
}
```

These functions become callable tools.

What this achieves:

```text
The model can request a tool call, but the application owns the actual execution.
```

### Step 3 - Agent Receives The Goal

The user prompt or lab-provided prompt becomes the goal:

```text
Validate this training setup and explain the reasoning loop.
```

What this achieves:

```text
The agent now has an objective to plan against.
```

### Step 4 - Agent Plans

The model identifies what steps are needed.

Example:

```text
1. Validate goal.
2. Inspect setup boundary.
3. Explain observations.
4. Reflect on whether the setup is ready.
```

What this achieves:

```text
The agent does not jump directly to a final answer; it structures the work.
```

### Step 5 - Agent Requests Tool Action

The model may request a C# tool call.

The app decides whether the call is allowed, approved, logged, and executed.

What this achieves:

```text
Tool execution happens under application control.
```

### Step 6 - Tool Result Becomes Observation

Example:

```text
Observation:
The setup goal is valid.
The action requires a controlled tool boundary.
```

What this achieves:

```text
The agent grounds the next step in actual returned evidence.
```

### Step 7 - Reflection And Checkpoint

The agent summarizes:

```text
Goal:
Plan:
Action:
Observation:
Reflection:
Checkpoint:
```

What this achieves:

```text
You can inspect the agentic loop instead of treating the answer as a black box.
```

## 5. Recall Summary

```text
Lab 01 teaches the core agentic loop:
goal -> plan -> action -> observation -> reflection -> checkpoint.
```

## 6. References For This Lab

- [Agent Framework middleware](https://learn.microsoft.com/en-us/agent-framework/agents/middleware/)
- [Microsoft.Agents.AI API reference](https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| Middleware | Middleware lets the application observe, log, validate, or govern model/tool activity around an agent run |
| Tool boundary | Tool calls are not just model text; they are application-controlled execution points |
| Agent APIs | Types such as `AIAgent`, `AgentResponse`, `AgentRunOptions`, and session-related types form the core programming model |
| Approval and control | Approval and middleware are how the application keeps the reasoning loop reviewable and safe |

Practical recall:

```text
The model reasons.
The application controls tools, middleware, approval, and checkpoints.
```

---

# Lab 02 - Flow Engineering

## 1. Concept

Lab 02 teaches how to make agentic work more controlled by putting it inside a structured flow.

Lab 01 showed the reasoning loop. Lab 02 adds explicit workflow control:

```text
start
  -> validate input
  -> branch
  -> approve if needed
  -> retry if needed
  -> terminate with typed result
```

The important shift:

```text
The model can reason, but the application controls the workflow path.
```

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Flow step | A named unit of work | Validate request, classify issue, retrieve data, produce output |
| Branching | Choosing a path based on state/result | Simple vs complex request, safe vs risky action |
| Approval | Human/policy gate before continuation | HITL approval, deployment approval, data export approval |
| Retry | Repeat a failed/weak step with changed input | Fix invalid output, retry transient API failure, improve answer |
| Termination | Controlled end condition | Done, rejected, expired, failed, needs human |
| Typed result | Structured output instead of loose text | Status object, validation report, workflow decision |
| Flow state | Data passed between steps | Current goal, validation result, approval result, retry count |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- why enterprise agents need workflow control
- how explicit steps make behavior easier to test
- how approval creates a trust boundary
- why retry should be bounded
- why typed termination is safer than open-ended completion

The lab may cover:

```text
- defining workflow state
- validating input
- branching based on result
- pausing for approval
- retrying a step
- returning a typed final result
```

## 4. Lab Flow Walkthrough

### Step 1 - Program Starts

Starting point:

```text
Program.cs
```

The app usually:

```text
1. Builds the flow state.
2. Creates the agent/model client.
3. Defines workflow steps.
4. Runs the flow from the first step.
```

What this achieves:

```text
The lab moves from a free-form agent run to a controlled workflow.
```

### Step 2 - Request Is Validated

Look for code shaped like:

```csharp
var validation = ValidateRequest(userGoal);

if (!validation.IsValid)
{
    return WorkflowResult.Rejected(validation.Reason);
}
```

What this achieves:

```text
The workflow does not blindly send every request to the model.
```

### Step 3 - Flow Branches

Example:

```csharp
if (request.RequiresApproval)
{
    approval = await RequestApprovalAsync(request);
}
else
{
    approval = ApprovalResult.NotRequired;
}
```

What this achieves:

```text
The application decides when the agent can continue.
```

### Step 4 - Approval Boundary

The lab may use console approval:

```csharp
Console.Write("Approve this action? Y/N: ");
var approved = Console.ReadLine();
```

The enterprise version can use email, Teams, or workflow approval.

What this achieves:

```text
The model cannot cross a sensitive boundary without authorization.
```

### Step 5 - Retry Or Continue

Example:

```csharp
for (var attempt = 1; attempt <= maxAttempts; attempt++)
{
    var result = await RunStepAsync(state);

    if (result.IsAcceptable)
    {
        break;
    }
}
```

What this achieves:

```text
Retry is bounded and visible instead of uncontrolled looping.
```

### Step 6 - Typed Termination

Example:

```csharp
public sealed record WorkflowResult(
    string Status,
    string Summary,
    IReadOnlyList<string> Evidence);
```

What this achieves:

```text
The workflow ends with a structured result that can be logged, tested, or displayed.
```

## 5. Recall Summary

```text
Lab 02 teaches that agentic work should move through controlled flow steps, not uncontrolled improvisation.
```

## 6. References For This Lab

- [Workflow orchestrations in Agent Framework](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/)
- [Microsoft.Agents.AI API reference](https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| Workflow orchestration | Agentic work can be organized as explicit steps instead of one open-ended prompt |
| Branching | A workflow can choose different paths based on validation, model output, or external state |
| Approval | Human or policy approval is a workflow boundary, not only a console prompt |
| Retry and termination | A production flow needs bounded retry and clear stop conditions |
| Typed outputs | Structured results are easier to test, log, and pass to the next step |

Practical recall:

```text
Flow engineering moves control from "the model decides everything" to "the application owns the path."
```

---

# Lab 03 - Agent Skills

## 1. Concept

Lab 03 explains how agents get reusable capabilities.

An agent skill is a portable capability package that can include:

- instructions
- references
- assets
- executable helpers
- domain rules

The official Agent Framework skills documentation describes skills as capability packages that can be loaded progressively. Reference: [Agent Skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp).

The important idea:

```text
Skills give the agent a catalog of reusable capabilities.
```

## 2. Skill Structure

The official skill structure is:

```text
skill-folder/
├── SKILL.md
├── scripts/
├── references/
└── assets/
```

Reference: [Skill structure](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#skill-structure).

### `SKILL.md`

`SKILL.md` is the required entry point.

It contains:

- skill name
- description
- routing guidance
- instructions
- examples

Use cases:

| Use case | Why it helps |
|---|---|
| Lab setup guidance | Keeps approved setup instructions together |
| Policy guidance | Gives consistent policy explanation |
| Troubleshooting | Gives known failure modes and resolution steps |
| Domain workflow | Gives repeatable task process |

### `references/`

References are detailed supporting docs.

Use cases:

| Use case | Why it helps |
|---|---|
| Large FAQ | Loaded only when needed |
| API notes | Keeps detailed docs out of main instructions |
| Error catalog | Agent can look up specific failures |
| Architecture explanation | Supports deeper student questions |

### `assets/`

Assets are reusable templates, sample files, or static artifacts.

Use cases:

| Use case | Why it helps |
|---|---|
| Report template | Agent can fill a known structure |
| Expected output sample | Agent can compare actual result |
| Checklist | Agent can guide validation |
| Starter snippet | Agent can reuse standard scaffolding |

### `scripts/`

Scripts are executable helpers. In this Day 03 reference, examples should be .NET/C# oriented.

Examples:

```text
scripts/
├── ValidateEnvironment.csx
├── SummarizeBuildLog.csx
└── GenerateSetupReport.csx
```

Use cases:

| Use case | Why it helps |
|---|---|
| Environment validation | Check .NET version and required variables |
| Build log parsing | Extract warnings/errors |
| Report generation | Produce structured summary |
| Data normalization | Convert raw output to known shape |

Script execution should be treated as a trust boundary and may require approval. Reference: [Tool approval](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#tool-approval).

## 3. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Progressive disclosure | Load only what is needed | Keep agent context smaller |
| File-based skill | Skill from folder with `SKILL.md` | Starter pack capabilities |
| Code-defined skill | Skill built in C# code | Dynamic context or environment-specific capability |
| Class-based skill | Skill packaged as C# class | Reusable enterprise/NuGet-style skill |
| MCP-based skill | Skill discovered from MCP server | Shared remote skill registry |
| AgentSkillsProvider | Exposes skills to agent | Makes skill catalog available |
| AgentSkillsProviderBuilder | Combines skill sources | Mix file, inline, class, MCP skills |

References:

- [Progressive disclosure](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#progressive-disclosure)
- [File-based skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#file-based-skills)
- [Code-defined skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#code-defined-skills)
- [Class-based skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#class-based-skills)
- [MCP-based skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#mcp-based-skills)

## 4. Lab Objectives And Coverage

This lab helps you understand:

- how skills are structured
- how skills are discovered
- how the agent decides when to load a skill
- how resources and assets support the skill
- how scripts/functions add executable behavior
- why skill execution should be governed

The lab may cover:

```text
- file-based skills
- inline/code-defined skills
- skill provider registration
- resource reading
- script or function execution
- approval around sensitive skill actions
```

## 5. Lab Flow Walkthrough

### Step 1 - Program Starts

Starting point:

```text
Program.cs
```

What happens:

```text
1. Load environment variables.
2. Create model/agent client.
3. Locate skill folder or define skills in code.
4. Build AgentSkillsProvider.
5. Attach provider to agent context.
6. Run the agent with a prompt that requires a skill.
```

### Step 2 - Skills Are Advertised

The agent sees skill names and descriptions.

What this achieves:

```text
The model can decide which skill is relevant without loading every file.
```

### Step 3 - Skill Is Loaded

When relevant, the agent loads `SKILL.md`.

What this achieves:

```text
The agent gets detailed instructions for this specific capability.
```

### Step 4 - Resource Or Asset Is Read

If needed, the agent reads:

```text
references/<file>
assets/<file>
```

What this achieves:

```text
The agent gets deeper context only when required.
```

### Step 5 - Executable Helper Runs

If the task needs computation or validation, a C# helper or script runs.

Example shape:

```csharp
public static string SummarizeSetup(string dotnetVersion, bool azureLoggedIn)
{
    return JsonSerializer.Serialize(new
    {
        dotnetVersion,
        azureLoggedIn,
        ready = dotnetVersion.StartsWith("10.") && azureLoggedIn
    });
}
```

What this achieves:

```text
The agent response can be grounded in actual computed evidence.
```

### Step 6 - Result Becomes Observation

The agent receives the skill output.

What this achieves:

```text
The final answer is based on skill-specific instructions and evidence.
```

## 6. Recall Summary

```text
Lab 03 gives the agent a reusable capability catalog.
A skill is more than a tool: it packages instructions, resources, assets, and optional executable helpers.
```

## 7. References For This Lab

- [Agent Skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp)
- [Skill structure](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#skill-structure)
- [Progressive disclosure](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#progressive-disclosure)
- [File-based skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#file-based-skills)
- [Code-defined skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#code-defined-skills)
- [Class-based skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#class-based-skills)
- [MCP-based skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#mcp-based-skills)
- [Tool approval for skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#tool-approval)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| Skill package | A skill can package instructions, references, assets, and executable helpers |
| `SKILL.md` | `SKILL.md` is the required entry point for file-based skills |
| `references/` | Detailed documents can be loaded only when needed |
| `assets/` | Templates, examples, and static files can support repeatable output |
| `scripts/` or executable helpers | Computation or validation helpers should be treated as controlled execution |
| Progressive disclosure | The agent first sees skill metadata, then loads deeper content only when relevant |
| Provider construction | `AgentSkillsProvider` and builders expose skills to the agent |
| Approval | Loading/reading/running skill capabilities can be governed by approval rules |

Practical recall:

```text
Skills are a capability catalog.
Progressive disclosure keeps the agent from loading every detail up front.
```

---

# Lab 04 - Conversations, State, And Memory

## 1. Concept

Lab 04 teaches how agentic applications manage conversational continuity.

The core distinction:

```text
Conversation history:
What was said in this conversation.

Session state:
Runtime state associated with the current agent interaction.

Memory:
Stored information that can be reused later.

Context provider:
External facts injected into the current run.

Checkpoint:
Persisted state that allows restore/resume.
```

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| AgentSession | Current unit of interaction/state | Track current conversation, metadata, checkpoint |
| Conversation ID | Key that separates one chat from another | New chat, resume old chat, switch topics |
| Chat history | Prior user/assistant turns | Follow-up answers, recall prior question |
| State bag | Structured runtime metadata | Student ID, lab step, retry count, setup status |
| Context provider | External current facts | Current lab, repo path, student profile, policy |
| Serialization | Convert session/state to stored form | Save to disk/Cosmos |
| Cosmos checkpoint | Durable persisted state | Resume after process restart |
| Compaction | Summarize long history | Reduce context size while preserving key facts |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- how to keep conversation histories separate
- how to start a new conversation
- how to resume an old conversation
- how state differs from message history
- how context providers differ from history
- how checkpointing enables restart-safe state

The lab may cover:

```text
- creating an AgentSession
- storing session metadata
- adding messages to history
- switching conversation IDs
- using in-memory history
- optionally using Cosmos-backed history/checkpointing
- serializing/deserializing session state
- custom context provider for lab-specific facts
```

## 4. Conversation Switching

The code equivalent of ChatGPT's "New chat" is:

```csharp
var conversationId = Guid.NewGuid().ToString("n");
```

Same ID:

```text
continue same conversation
```

New ID:

```text
fresh conversation; previous history is not included
```

Old ID:

```text
resume previous conversation if stored
```

Example:

```csharp
var conversationId = $"{batchId}/{studentId}/day03-lab04/{conversationName}";
```

## 5. Custom Context Provider

A new conversation clears history, but it should not erase current app context.

Example:

```text
New conversation:
No prior messages.

Context provider:
Still knows current lab, batch, student ID, repo, region, policy.
```

Use cases:

| Context type | Use case |
|---|---|
| Training context | Current lab, current topic, batch ID |
| Student context | Student ID, progress, blockers |
| Environment context | .NET version, repo path, Cosmos mode |
| Governance context | Allowed/restricted actions |
| App context | Current page, selected account, active workflow |

## 6. Lab Flow Walkthrough

### Step 1 - Program Starts

The app usually:

```text
1. Reads batch/student/lab variables.
2. Creates a default conversation identity.
3. Creates history store.
4. Creates context provider.
5. Creates agent/session.
6. Starts console loop.
```

### Step 2 - Conversation Is Selected

Console commands may look like:

```text
/new setup-help
/use setup-help
/list
/history
/context
```

What this achieves:

```text
The user controls which conversation history is active.
```

### Step 3 - History Is Loaded

Example:

```csharp
var history = await historyStore.LoadAsync(conversationId, cancellationToken);
```

What this achieves:

```text
Only messages for this conversation are included.
```

### Step 4 - Context Is Built

Example:

```csharp
var context = await trainingLabContextProvider.GetContextAsync(
    conversationIdentity,
    cancellationToken);
```

What this achieves:

```text
The agent gets current app facts that are independent of chat history.
```

### Step 5 - Agent Runs

The agent receives:

```text
- current user input
- selected conversation history
- session state
- custom context
```

What this achieves:

```text
The agent can answer follow-ups and still know current lab facts.
```

### Step 6 - Messages And Checkpoint Are Saved

Example:

```csharp
await historyStore.AppendAsync(conversationId, "user", userInput);
await historyStore.AppendAsync(conversationId, "assistant", responseText);
```

What this achieves:

```text
Future turns in the same conversation can load this history.
```

## 7. Recall Summary

```text
Lab 04 teaches that history, state, context, and checkpointing are separate controls.
New conversation means new history scope; context can still describe the current app situation.
```

## 8. References For This Lab

- [Microsoft.Agents.AI API reference](https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest)
- [Agent Framework skills and context concepts](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp)
- [Cosmos DB .NET SDK v3](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| Agent session APIs | Session-related types represent state associated with an agent run or conversation |
| Context provider concepts | Context is extra current information supplied to a run; it is not the same as prior messages |
| Chat history concepts | History is the set of previous user/assistant turns for a selected conversation |
| Cosmos DB SDK | Cosmos DB can persist session checkpoints or conversation history so it survives process restart |
| Conversation identity | A new conversation is a new history key; an old key resumes old history |

Practical recall:

```text
History answers: what was said before?
Context answers: what does the app know right now?
Session answers: what state belongs to this run/conversation?
Checkpoint answers: what can be restored later?
```

---

# Lab 05 - Harness Engineering

## 1. Concept

Lab 05 teaches that an enterprise agent needs a runtime harness around the model.

The model is only one part.

The harness provides:

```text
tools
approval
file/memory support
evidence capture
loop limits
telemetry
logging
mode control
```

The harness makes model-powered work operational.

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Tool boundary | Controlled functions the agent can call | Build validation, data lookup, file inspection |
| Approval | Human/policy gate before tool execution | Delete, deploy, send email, modify data |
| File access provider | Controlled file read/write surface | Inspect lab files, save report, read logs |
| Memory provider | Store useful facts | Long-running tasks, preferences, state |
| Todo provider | Track sub-tasks | Multi-step coding, lab troubleshooting |
| Mode provider | Control behavior mode | Plan-only, execute, review, safe mode |
| Loop evaluator | Decide whether agent should continue | Stop on DONE, retry if incomplete |
| Evidence capture | Record why answer was produced | Auditing, debugging, grading |
| Telemetry/logging | Observe execution | Cost, latency, tool use, failures |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- why a harness is needed around a model
- how tools are exposed safely
- how approvals reduce risk
- how evidence makes agent work reviewable
- how loop evaluators prevent uncontrolled execution
- how file/memory/todo providers support long-running tasks

The lab may cover:

```text
- creating a harness-like runtime
- giving the agent controlled tools
- requiring approval for sensitive actions
- capturing evidence
- stopping when a completion marker is reached
- showing logs or telemetry
```

## 4. Lab Flow Walkthrough

### Step 1 - Program Starts

The app usually:

```text
1. Creates the agent.
2. Registers tool providers.
3. Registers file/memory/todo support.
4. Adds approval rules.
5. Adds loop evaluator.
6. Runs a task through the harness.
```

What this achieves:

```text
The agent is operating inside a controlled runtime instead of raw model access.
```

### Step 2 - Harness Registers Tools

Example:

```csharp
static string InspectBuildOutput(string output)
{
    return output.Contains("error", StringComparison.OrdinalIgnoreCase)
        ? "Build output contains errors."
        : "No build errors detected.";
}
```

What this achieves:

```text
The model can request a task-specific capability without owning execution.
```

### Step 3 - Approval Is Applied

Example:

```csharp
if (toolCall.RequiresApproval)
{
    var approved = await approvalGate.RequestAsync(toolCall);
}
```

What this achieves:

```text
Sensitive operations are gated.
```

### Step 4 - Evidence Is Captured

Example:

```csharp
evidence.Add(new EvidenceItem(
    Source: "InspectBuildOutput",
    Summary: "Build output contains errors."));
```

What this achieves:

```text
The final answer can show what it was based on.
```

### Step 5 - Loop Evaluator Decides Continue/Stop

Example:

```csharp
if (response.Contains("DONE", StringComparison.OrdinalIgnoreCase))
{
    return LoopDecision.Stop;
}
```

What this achieves:

```text
The agent cannot loop forever without a stopping rule.
```

### Step 6 - Final Output Shows Harness Value

The final response should show:

```text
- work performed
- evidence collected
- approval decisions
- final state
- whether the task is complete
```

## 5. Recall Summary

```text
Lab 05 teaches that production agents need a runtime harness: tools, evidence, approval, memory, telemetry, and stop rules.
```

## 6. References For This Lab

- [Agent Framework middleware](https://learn.microsoft.com/en-us/agent-framework/agents/middleware/)
- [Agent Framework observability](https://learn.microsoft.com/en-us/agent-framework/agents/observability?pivots=programming-language-csharp)
- [Microsoft.Agents.AI API reference](https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| Middleware | Cross-cutting behavior can be added around model and tool activity |
| Observability | Agent runs should expose logs, traces, and runtime signals for debugging and operations |
| Tool approval | Sensitive tool execution should be controlled by the application |
| Harness-adjacent APIs | File access, memory, todo, mode, loop, and evaluation concepts support long-running agent work |
| Evidence | The harness should capture what happened so answers are reviewable |

Practical recall:

```text
A harness is the runtime around the model: tools, approval, evidence, memory, loop control, and telemetry.
```

---

# Lab 06 - Retrieval-Grounded RAG For Agentic Workflow

## 1. Concept

Lab 06 teaches how an agent grounds its answer in retrieved evidence.

RAG means:

```text
Retrieval-Augmented Generation
```

The agent does not answer only from general model knowledge. It first retrieves relevant data, then uses that data as grounding context.

In this lab:

```text
Cosmos DB is the retrieval store.
The retrieval component plays the TextSearchProvider role.
The agent uses retrieved structured data as evidence.
```

Say this carefully:

```text
Lab 06 demonstrates the TextSearchProvider pattern for agentic RAG, implemented over Cosmos DB.
```

Do not say the lab uses the official `TextSearchProvider` API unless the source code actually uses that type.

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Retrieval | Query relevant records | Search policy, lab docs, product data |
| Structured filter | Filter by fields like labId/topic/batch | Limit to current lab or student cohort |
| Grounding evidence | Data inserted into prompt/context | Avoid unsupported answer |
| Verification | Check whether answer cites evidence | Reduce hallucination |
| Retry | Retrieve again if evidence is weak | Improve recall or broaden query |
| Citation | Show source fields/IDs | Audit, student confidence, traceability |
| Cosmos DB query | Query JSON documents | Structured training knowledge |
| Vector search | Similarity over embeddings if enabled | Semantic retrieval |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- how an agent retrieves evidence before answering
- how structured data can ground a response
- how Cosmos DB documents can represent training knowledge
- how filters keep retrieval scoped
- how retrieved records become context
- how citations/evidence make answers reviewable

The lab may cover:

```text
- seeding or reading training knowledge in Cosmos DB
- querying by lab/topic/tags
- optionally using vector-like retrieval
- returning top relevant records
- injecting retrieved evidence into the agent prompt
- asking the agent to answer only from evidence
- showing citations or source IDs
```

## 4. Example Structured Document

Example Cosmos DB item:

```json
{
  "id": "lab06-rag-001",
  "batchId": "AN-2607-101",
  "labId": "day03-lab06",
  "topic": "retrieval-grounded workflow",
  "tags": ["rag", "cosmos", "grounding"],
  "title": "Why grounding matters",
  "content": "The agent should use retrieved evidence before answering.",
  "source": "student-playbook-day03-day04",
  "createdAtUtc": "2026-07-12T00:00:00Z"
}
```

## 5. Lab Flow Walkthrough

### Step 1 - Program Starts

The app usually:

```text
1. Reads Cosmos DB environment variables.
2. Creates CosmosClient.
3. Opens database/container.
4. Creates retrieval service.
5. Creates agent.
6. Runs user question through retrieval-grounded flow.
```

### Step 2 - User Asks A Question

Example:

```text
Why do we use Cosmos DB for this lab instead of only model memory?
```

What this achieves:

```text
The question becomes the retrieval query and the agent goal.
```

### Step 3 - Retrieval Service Queries Cosmos DB

Example shape:

```csharp
var query = new QueryDefinition("""
    SELECT TOP 5 c.id, c.title, c.content, c.source
    FROM c
    WHERE c.batchId = @batchId
    AND c.labId = @labId
    AND ARRAY_CONTAINS(c.tags, @tag)
    """)
    .WithParameter("@batchId", batchId)
    .WithParameter("@labId", "day03-lab06")
    .WithParameter("@tag", "rag");
```

What this achieves:

```text
The app retrieves only relevant structured records.
```

### Step 4 - Evidence Is Formatted

Example:

```csharp
var evidenceText = string.Join(
    Environment.NewLine,
    results.Select(r => $"- [{r.Id}] {r.Title}: {r.Content}"));
```

What this achieves:

```text
The agent receives concise grounding context instead of raw database output.
```

### Step 5 - Agent Answers With Grounding

The prompt/context should instruct:

```text
Use only the retrieved evidence.
If evidence is insufficient, say what is missing.
Include source IDs.
```

What this achieves:

```text
The response is tied to retrieved evidence.
```

### Step 6 - Verification And Retry

The app may check:

```text
Did the response cite a source?
Was enough evidence retrieved?
Should retrieval be broadened?
```

What this achieves:

```text
The agentic workflow can improve grounding before final answer.
```

## 6. Recall Summary

```text
Lab 06 teaches agentic RAG: retrieve relevant evidence first, then answer with grounding and citations.
Cosmos DB is the retrieval store; the retrieval component plays the TextSearchProvider role.
```

Useful references:

- [RAG in Azure AI Search](https://learn.microsoft.com/en-us/azure/search/retrieval-augmented-generation-overview)
- [Vector search in Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/vector-search)
- [Cosmos DB NoSQL queries](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/query/)
- [Cosmos DB parameterized queries](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/query/parameterized-queries)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| RAG overview | RAG grounds generation in retrieved enterprise or application data |
| Retrieval step | The application retrieves relevant evidence before asking the model to answer |
| Structured queries | Cosmos DB can filter by fields such as `batchId`, `labId`, `topic`, `tags`, or `source` |
| Parameterized queries | Query parameters avoid unsafe string-built query text and make queries reusable |
| Vector search | Cosmos DB can store embeddings and use vector similarity when semantic retrieval is required |
| TextSearchProvider pattern | Even if the official type is not used, the Cosmos-backed retriever plays the retrieval-provider role |

Practical recall:

```text
Cosmos DB SDK explains how to query data.
RAG references explain how retrieved data becomes grounding evidence.
```

---

# Lab 07 - Workflow Agent

## 1. Concept

Lab 07 combines earlier Day 03 ideas into a structured workflow agent.

Earlier labs introduced:

```text
Lab 01 -> reasoning loop
Lab 02 -> flow engineering
Lab 03 -> skills
Lab 04 -> state and memory
Lab 05 -> harness and evidence
Lab 06 -> retrieval-grounded workflow
Lab 07 -> workflow agent
```

A workflow agent is:

```text
An agentic system where model-powered reasoning is embedded inside a structured, observable, and controllable workflow.
```

The agent is part of the workflow. The workflow is the control system.

## 2. Sub-Topics And Use Cases

| Sub-topic | What it means | Practical use cases |
|---|---|---|
| Workflow stage | Named step in the process | Intake, retrieve, analyze, validate, summarize |
| Specialist role | Focused responsibility | Planner, retriever, reviewer, validator |
| Shared state | Data passed between stages | Request, evidence, draft answer, validation result |
| Evidence handoff | Retrieved data passed forward | Grounded analysis, audit trail |
| Validation step | Check output before final | Citation check, policy check, completeness check |
| Final response | User-facing answer | Student guidance, report, recommendation |
| Checkpoint | Save workflow state | Resume, debug, audit |

## 3. Lab Objectives And Coverage

This lab helps you understand:

- how agentic work becomes a workflow
- how specialist roles divide responsibility
- how state/evidence moves between steps
- how validation improves reliability
- how the final answer is produced after controlled stages

The lab may cover:

```text
- defining workflow state
- creating specialist prompts/agents/functions
- running steps in order
- retrieving or preparing evidence
- validating output
- returning final response
- checkpointing workflow state
```

## 4. Example Workflow

Suppose the user asks:

```text
Review this lab answer and suggest what I should improve.
```

Workflow:

```text
1. Intake step
   Understand the user request.

2. Context step
   Load lab and student context.

3. Retrieval step
   Find relevant playbook/lab content.

4. Analysis step
   Compare student answer to expected concepts.

5. Validation step
   Check whether feedback is grounded.

6. Summary step
   Produce final student-friendly answer.
```

## 5. Lab Flow Walkthrough

### Step 1 - Program Starts

The app usually:

```text
1. Creates workflow state.
2. Creates model/agent clients.
3. Registers tools/skills/retrievers.
4. Defines workflow steps.
5. Starts workflow execution.
```

### Step 2 - Intake Step

Example:

```csharp
var request = WorkflowRequest.FromUserInput(userInput);
```

What this achieves:

```text
The workflow converts raw user input into structured state.
```

### Step 3 - Planning Or Routing Step

Example:

```csharp
var route = await planner.DecideNextStepAsync(request, cancellationToken);
```

What this achieves:

```text
The workflow decides which specialist step is needed.
```

### Step 4 - Retrieval Or Evidence Step

Example:

```csharp
var evidence = await retriever.SearchAsync(request.Topic, cancellationToken);
```

What this achieves:

```text
The workflow gathers grounding data before analysis.
```

### Step 5 - Specialist Analysis Step

Example:

```csharp
var draft = await analyzer.AnalyzeAsync(request, evidence, cancellationToken);
```

What this achieves:

```text
A focused role performs the reasoning task.
```

### Step 6 - Validation Step

Example:

```csharp
var validation = await validator.ValidateAsync(draft, evidence, cancellationToken);

if (!validation.IsValid)
{
    draft = await analyzer.ReviseAsync(draft, validation, cancellationToken);
}
```

What this achieves:

```text
The workflow checks output before showing it to the user.
```

### Step 7 - Final Response And Checkpoint

Example:

```csharp
var final = await summarizer.CreateFinalAnswerAsync(draft, cancellationToken);
await checkpointStore.SaveAsync(workflowState, cancellationToken);
```

What this achieves:

```text
The user receives a controlled final answer and the workflow state is saved.
```

## 6. Recall Summary

```text
Lab 07 shows that enterprise agents are workflows around model calls.
Work moves through stages, evidence moves with state, and validation happens before final output.
```

## 7. References For This Lab

- [Workflow orchestrations in Agent Framework](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/)
- [Magentic orchestration in Agent Framework](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/magentic?pivots=programming-language-csharp)
- [Microsoft.Agents.AI API reference](https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest)

What to take from these references:

| Reference area | What it helps you recall |
|---|---|
| Workflow orchestration | A workflow agent organizes model-powered work into explicit stages |
| Specialist roles | Planner, retriever, analyzer, validator, and summarizer responsibilities can be separated |
| Magentic pattern | Coordinator-style orchestration shows how planning, delegation, evaluation, and re-planning work |
| State handoff | Workflow stages need shared state so evidence and decisions move forward |
| Validation | Final output should be checked before being shown to the user |

Practical recall:

```text
The model is inside the workflow.
The workflow is the control system.
```

---

# Day 03 Overall Map

| Lab | Main idea | What to remember |
|---|---|---|
| Lab 01 | Agentic reasoning loop | Agent work follows goal, plan, action, observation, reflection, checkpoint |
| Lab 02 | Flow engineering | Application controls branches, approval, retry, and termination |
| Lab 03 | Skills | Skills package reusable capabilities |
| Lab 04 | State and memory | History, session, context, and checkpoint are separate controls |
| Lab 05 | Harness | Production agents need runtime controls around the model |
| Lab 06 | RAG | Retrieve evidence before answering |
| Lab 07 | Workflow agent | Agentic systems become structured workflows |

# How Day 03 Concepts Progress Across Labs

Day 03 is not seven separate ideas. It is a progressive build-up from a simple agent loop into a controlled, grounded, observable workflow agent.

## 1. From Reasoning Loop To Flow Control

Lab 01 starts with the agent execution lifecycle:

```text
goal -> plan -> action -> observation -> reflection -> checkpoint
```

This gives you the basic mental model of how an agent works. The agent does not only generate text; it repeatedly decides what to do next, performs an action, observes the result, and updates its next step.

Lab 02 adds flow engineering on top of that lifecycle. Instead of allowing every action to run automatically, the application can control:

| Control | Why it matters |
|---|---|
| Branching | Different inputs need different paths |
| Retry | Failed or weak steps can be attempted again |
| Human approval | Sensitive actions should wait for permission |
| Termination | The agent should stop when the work is complete |

Progression to remember:

```text
Lab 01 explains how the agent thinks and acts.
Lab 02 explains how the application controls that thinking and acting.
```

## 2. From Tools To Skills

Lab 01 and Lab 02 introduce tools as actions the agent can call. A tool is usually a focused capability, such as checking inventory, sending a request, searching records, or validating a setup.

Lab 03 expands this into skills. A skill is a packaged capability area that can contain instructions, C# code, scripts, references, and assets. This matters because real agent systems need organized capability packages rather than a long unstructured list of tools.

Progression to remember:

| Earlier labs | Lab 03 evolution |
|---|---|
| Tool call | Reusable skill |
| Single action | Capability package |
| Direct function | Instructions, assets, references, and implementation together |
| Always visible capability list | Progressive disclosure of only relevant capability information |

Practical example:

```text
A refund tool might process one refund action.
A support-resolution skill can include refund rules, escalation guidance, account lookup tools, policy references, and approval instructions.
```

## 3. From Chat History To State, Context, And Checkpoints

Lab 04 clarifies that memory is not one thing.

| Concept | What it means |
|---|---|
| Conversation history | Messages exchanged in one session |
| Session | A boundary around one conversation or task thread |
| State | Data the application tracks while work is happening |
| Context | Relevant facts supplied to the agent for the current run |
| Checkpoint | A saved point that allows continuation or recovery |

This is where students learn how to start a new agent session when they want a fresh conversation history. They also learn that custom context providers can supply domain-specific or app-specific context that the model would not know from chat history alone.

Progression to remember:

```text
Lab 01 has checkpoints as part of the lifecycle.
Lab 04 explains what is actually being remembered, separated, restored, or supplied as context.
```

## 4. From Working Demo To Runtime Harness

Lab 05 brings production thinking into the agent design. A runtime harness wraps the model and agent calls with controls that make the system safer and easier to operate.

The harness can manage:

| Harness concern | Practical purpose |
|---|---|
| Tool approval | Prevent unsafe or unintended actions |
| Evidence capture | Keep track of why the agent answered a certain way |
| Telemetry | Understand what happened during execution |
| Loop limits | Stop runaway execution |
| Error handling | Recover or fail cleanly |
| Policy checks | Apply application rules around model behavior |

Progression to remember:

```text
Lab 01 shows the lifecycle.
Lab 05 makes the lifecycle operationally controlled.
```

## 5. From Model Knowledge To Grounded Answers

Lab 06 adds retrieval-augmented generation. The important shift is that the agent should not answer only from model knowledge when business-specific or current data is required.

Instead, the flow becomes:

```text
user question -> retrieve relevant evidence -> pass evidence to agent -> generate grounded answer
```

For structured data, Cosmos DB can provide records, filters, metadata, and domain entities. For text-heavy knowledge, search and vector retrieval can provide relevant passages. The agent uses the retrieved data as grounding context before producing the final response.

Progression to remember:

```text
Lab 04 supplies context.
Lab 06 retrieves external evidence and uses it as grounding context.
```

## 6. From Agent Call To Workflow Agent

Lab 07 combines the earlier ideas into a structured workflow. A workflow agent separates the work into stages such as planning, retrieval, analysis, validation, and final response.

The earlier labs now reappear inside a larger control structure:

| Earlier concept | How it appears in Lab 07 |
|---|---|
| Lifecycle | Each workflow stage may include planning, action, observation, and reflection |
| Flow engineering | The workflow branches, retries, and stops intentionally |
| Skills | Specialist capabilities are invoked where needed |
| State and memory | Workflow state carries evidence and decisions between stages |
| Harness | The workflow is monitored and controlled |
| RAG | Retrieval stages ground later analysis |

End-to-end mental model:

```text
An enterprise agent is not only a prompt.
It is a controlled workflow that can use skills, manage state, retrieve evidence, ask for approval, and produce observable outcomes.
```

# References

- [Agent Framework overview](https://learn.microsoft.com/en-us/agent-framework/)
- [Microsoft.Agents.AI API reference](https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai?view=agent-framework-dotnet-latest)
- [Agent Framework Middleware](https://learn.microsoft.com/en-us/agent-framework/agents/middleware/)
- [Agent Framework Skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp)
- [Skill structure](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#skill-structure)
- [Progressive disclosure for skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp#progressive-disclosure)
- [Workflow orchestrations](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/)
- [Magentic orchestration](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/magentic?pivots=programming-language-csharp)
- [Agent observability](https://learn.microsoft.com/en-us/agent-framework/agents/observability?pivots=programming-language-csharp)
- [RAG in Azure AI Search](https://learn.microsoft.com/en-us/azure/search/retrieval-augmented-generation-overview)
- [Cosmos DB .NET SDK v3](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3)
- [Cosmos DB NoSQL queries](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/query/)
- [Parameterized queries in Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/query/parameterized-queries)
- [Vector search in Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/vector-search)
