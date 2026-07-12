using System.Text.Json;
using System.Text.Json.Nodes;
using AGUI.Abstractions;
using Pronative.MultiAgentTraining.Shared;

Day04Console.PrintAppStart();

const string threadId = "thread-an2607101-day04-lab04";
const string runId = "run-approval-001";
const string toolCallId = "toolcall-raise-training-exception-001";
const string interruptId = "interrupt-tool-approval-001";

var runInput = new RunAgentInput
{
    ThreadId = threadId,
    RunId = runId,
    Messages =
    [
        new AGUIUserMessage
        {
            Id = "msg-user-001",
            Content = "Prepare a Day 4 lab environment exception for one student and request trainer approval before changing quota."
        }
    ],
    Tools =
    [
        new AGUITool
        {
            Name = "raise_training_environment_exception",
            Description = "Creates a controlled exception request for a student's Azure training environment.",
            Parameters = JsonElementFrom(new
            {
                type = "object",
                required = new[] { "studentId", "reason", "requestedAction" },
                properties = new
                {
                    studentId = new { type = "string" },
                    reason = new { type = "string" },
                    requestedAction = new { type = "string" }
                }
            }),
            Metadata = JsonElementFrom(new
            {
                approval = "required",
                a2uiSurface = "training-exception-approval"
            })
        }
    ],
    State = JsonElementFrom(new
    {
        batchId = "AN-2607-101",
        studentId = "ST-2606-1004",
        currentStep = "needs_trainer_approval",
        risk = "quota-change"
    }),
    Context =
    [
        new AGUIContext
        {
            Description = "Enterprise boundary",
            Value = "The UI may render approval controls, but backend tool execution remains governed by the agent runtime and policy."
        }
    ]
};

var approvalToolCall = new AGUIToolCallInfo
{
    CallId = toolCallId,
    Name = "raise_training_environment_exception",
    Arguments = new Dictionary<string, object?>
    {
        ["studentId"] = "ST-2606-1004",
        ["reason"] = "Student needs a temporary higher request budget for Day 4 multi-agent testing.",
        ["requestedAction"] = "Allow one controlled model-call burst for 30 minutes."
    }
};

var aguiEvents = new BaseEvent[]
{
    new RunStartedEvent
    {
        ThreadId = threadId,
        RunId = runId,
        Input = runInput,
        Timestamp = Now()
    },
    new TextMessageStartEvent
    {
        MessageId = "msg-agent-001",
        Role = "assistant",
        Name = "training-ops-agent",
        Timestamp = Now()
    },
    new TextMessageContentEvent
    {
        MessageId = "msg-agent-001",
        Delta = "I found a quota-sensitive environment change. I will request trainer approval before using the tool.",
        Timestamp = Now()
    },
    new TextMessageEndEvent
    {
        MessageId = "msg-agent-001",
        Timestamp = Now()
    },
    new ToolCallStartEvent
    {
        ParentMessageId = "msg-agent-001",
        ToolCallId = toolCallId,
        ToolCallName = approvalToolCall.Name,
        Timestamp = Now()
    },
    new ToolCallArgsEvent
    {
        ToolCallId = toolCallId,
        Delta = JsonSerializer.Serialize(approvalToolCall.Arguments),
        Timestamp = Now()
    },
    new ToolCallEndEvent
    {
        ToolCallId = toolCallId,
        Timestamp = Now()
    },
    new StateSnapshotEvent
    {
        Snapshot = JsonElementFrom(new
        {
            batchId = "AN-2607-101",
            studentId = "ST-2606-1004",
            agentStep = "approval_requested",
            pendingToolCallId = toolCallId,
            uiSurface = "training-exception-approval"
        }),
        Timestamp = Now()
    },
    new RunFinishedEvent
    {
        ThreadId = threadId,
        RunId = runId,
        Outcome = new RunFinishedInterruptOutcome
        {
            Interrupts =
            [
                new AGUIInterrupt
                {
                    Id = interruptId,
                    Reason = "tool_call",
                    Message = "Approve the training environment exception before the tool executes.",
                    ToolCallId = toolCallId,
                    ResponseSchema = JsonElementFrom(new
                    {
                        type = "object",
                        required = new[] { "approved", "comment" },
                        properties = new
                        {
                            approved = new { type = "boolean" },
                            comment = new { type = "string" }
                        }
                    }),
                    Metadata = JsonElementFrom(new
                    {
                        batchId = "AN-2607-101",
                        studentId = "ST-2606-1004",
                        policy = "trainer-approval-required"
                    })
                }
            ]
        },
        Timestamp = Now()
    }
};

var a2uiCreateSurface = new
{
    jsonrpc = "2.0",
    id = "a2ui-001",
    method = "createSurface",
    @params = new
    {
        surfaceId = "training-exception-approval",
        version = "v1.0",
        catalogId = "pronative-training-ui-catalog",
        title = "Training Exception Approval"
    }
};

var a2uiUpdateComponents = new
{
    jsonrpc = "2.0",
    id = "a2ui-002",
    method = "updateComponents",
    @params = new
    {
        surfaceId = "training-exception-approval",
        components = new object[]
        {
            new
            {
                id = "approval-card",
                type = "Card",
                props = new
                {
                    title = "Approve quota-sensitive action",
                    tone = "warning"
                },
                children = new object[]
                {
                    new
                    {
                        id = "student-summary",
                        type = "Text",
                        props = new
                        {
                            value = "Student ST-2606-1004 requested a temporary controlled model-call burst."
                        }
                    },
                    new
                    {
                        id = "approve-button",
                        type = "Button",
                        props = new
                        {
                            label = "Approve",
                            action = "trainingException.approve"
                        }
                    },
                    new
                    {
                        id = "reject-button",
                        type = "Button",
                        props = new
                        {
                            label = "Reject",
                            action = "trainingException.reject"
                        }
                    }
                }
            }
        }
    }
};

var a2uiUpdateDataModel = new
{
    jsonrpc = "2.0",
    id = "a2ui-003",
    method = "updateDataModel",
    @params = new
    {
        surfaceId = "training-exception-approval",
        data = new
        {
            batchId = "AN-2607-101",
            studentId = "ST-2606-1004",
            toolCallId,
            requestedAction = "Allow one controlled model-call burst for 30 minutes.",
            approvalStatus = "pending"
        }
    }
};

var a2uiAction = new
{
    jsonrpc = "2.0",
    id = "a2ui-004",
    method = "action",
    @params = new
    {
        surfaceId = "training-exception-approval",
        action = "trainingException.approve",
        payload = new
        {
            approved = true,
            comment = "Approved for 30 minutes during trainer-led Day 4 testing."
        }
    }
};

var resumeInput = new RunAgentInput
{
    ThreadId = threadId,
    RunId = "run-approval-002",
    ParentRunId = runId,
    Resume =
    [
        new AGUIResume
        {
            InterruptId = interruptId,
            Status = "approved",
            Payload = JsonElementFrom(new AGUIToolApprovalResumePayload
            {
                Approved = true,
                ToolCall = approvalToolCall,
                Result = "Trainer approved the controlled exception for 30 minutes."
            })
        }
    ]
};

var resumedEvents = new BaseEvent[]
{
    new RunStartedEvent
    {
        ThreadId = threadId,
        RunId = resumeInput.RunId,
        ParentRunId = runId,
        Input = resumeInput,
        Timestamp = Now()
    },
    new ToolCallResultEvent
    {
        MessageId = "msg-tool-001",
        ToolCallId = toolCallId,
        Role = "tool",
        Content = "Exception recorded with policy tag trainer-approved and expiry 30 minutes.",
        Timestamp = Now()
    },
    new RunFinishedEvent
    {
        ThreadId = threadId,
        RunId = resumeInput.RunId,
        Outcome = new RunFinishedSuccessOutcome(),
        Result = JsonElementFrom(new
        {
            status = "completed",
            policyTag = "trainer-approved",
            expiresInMinutes = 30
        }),
        Timestamp = Now()
    }
};

WriteSection("1. AG-UI event stream from agent runtime to UI");
foreach (var aguiEvent in aguiEvents)
{
    WriteProtocolJson(aguiEvent.Type, SerializeAgUiEvent(aguiEvent));
}

WriteSection("2. A2UI declarative UI payloads carried to the frontend");
WriteProtocolObject("createSurface", a2uiCreateSurface);
WriteProtocolObject("updateComponents", a2uiUpdateComponents);
WriteProtocolObject("updateDataModel", a2uiUpdateDataModel);

WriteSection("3. User action becomes AG-UI resume input");
WriteProtocolObject("A2UI action", a2uiAction);
WriteAgUiObject("AG-UI RunAgentInput.Resume", resumeInput, AGUIJsonSerializerContext.Default.RunAgentInput);

WriteSection("4. Agent runtime continues after approval");
foreach (var aguiEvent in resumedEvents)
{
    WriteProtocolJson(aguiEvent.Type, SerializeAgUiEvent(aguiEvent));
}

Day04Console.PrintLabStart(4);

WriteSection("Trainer Notes");
Console.WriteLine("AG-UI is the event transport between the agent runtime and the user-facing app.");
Console.WriteLine("A2UI is the declarative UI payload: the agent requests approved components, not arbitrary frontend code.");
Console.WriteLine("The approval response crosses back as AGUIResume, so backend tool execution stays governed by policy.");

Day04Console.PrintLabEnd(4);

Day04Console.PrintAppEnd();

static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

static JsonElement JsonElementFrom<T>(T value)
{
    return JsonSerializer.SerializeToElement(value);
}

static void WriteSection(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', title.Length));
    Console.WriteLine(title);
    Console.WriteLine(new string('=', title.Length));
}

static void WriteProtocolObject<T>(string label, T value)
{
    Console.WriteLine();
    Console.WriteLine($"--- {label} ---");
    Console.WriteLine(JsonSerializer.Serialize(value, DisplayJson()));
}

static void WriteProtocolJson(string label, string json)
{
    Console.WriteLine();
    Console.WriteLine($"--- {label} ---");
    Console.WriteLine(json);
}

static void WriteAgUiObject<T>(string label, T value, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo)
{
    Console.WriteLine();
    Console.WriteLine($"--- {label} ---");
    var json = JsonSerializer.Serialize(value, typeInfo);
    Console.WriteLine(Pretty(json));
}

static string SerializeAgUiEvent(BaseEvent aguiEvent)
{
    var json = aguiEvent switch
    {
        RunStartedEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.RunStartedEvent),
        TextMessageStartEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.TextMessageStartEvent),
        TextMessageContentEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.TextMessageContentEvent),
        TextMessageEndEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.TextMessageEndEvent),
        ToolCallStartEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.ToolCallStartEvent),
        ToolCallArgsEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.ToolCallArgsEvent),
        ToolCallEndEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.ToolCallEndEvent),
        ToolCallResultEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.ToolCallResultEvent),
        StateSnapshotEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.StateSnapshotEvent),
        RunFinishedEvent value => JsonSerializer.Serialize(value, AGUIJsonSerializerContext.Default.RunFinishedEvent),
        _ => JsonSerializer.Serialize(aguiEvent, AGUIJsonSerializerContext.Default.BaseEvent)
    };

    return Pretty(json);
}

static string Pretty(string json)
{
    var node = JsonNode.Parse(json);
    return JsonSerializer.Serialize(node, DisplayJson());
}

static JsonSerializerOptions DisplayJson() => new()
{
    WriteIndented = true
};
