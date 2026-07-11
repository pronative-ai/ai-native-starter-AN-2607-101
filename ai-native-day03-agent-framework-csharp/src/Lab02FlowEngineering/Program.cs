using System.Text.Json;
using Microsoft.Agents.AI.Workflows;
using Pronative.Day03.Shared;

var config = Day03TrainingConfig.Load(args);
Day03Console.PrintAppStart();
Day03Console.PrintHeader(config, "Lab 02 - Flow Engineering");

Console.WriteLine("This lab uses Microsoft Agent Framework Workflows directly:");
Console.WriteLine("- WorkflowBuilder for an explicit business process graph");
Console.WriteLine("- typed Executor<TInput,TOutput> steps");
Console.WriteLine("- conditional switch routing for branching");
Console.WriteLine("- RequestPort + RequestInfoEvent + ExternalResponse for real HITL approval");
Console.WriteLine("- workflow state with QueueStateUpdateAsync / ReadStateAsync");
Console.WriteLine("- retry loop with a bounded execution attempt");
Console.WriteLine("- WorkflowOutputEvent as the termination contract");
Console.WriteLine();

Console.Write("Enter a training operations request, or press Enter for the default: ");
var requestText = Console.ReadLine();
if (string.IsNullOrWhiteSpace(requestText))
{
    requestText =
        "Prepare Day 3 workflow-agent practicals for batch AN-2607-101 and provision a short-lived shared approval path.";
}

// Workflow built from typed executors with branching, HITL, retry, and state management
var workflow = TrainingOperationsWorkflowFactory.Build(config);
var input = new TrainingRequest(
    RequestId: $"req-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
    BatchId: config.BatchId,
    StudentId: config.StudentId,
    Text: requestText.Trim());

// Streaming execution emits typed events (RequestInfoEvent, ExecutorCompletedEvent, WorkflowOutputEvent, etc.)
await using var run = await InProcessExecution.RunStreamingAsync(
    workflow,
    input,
    sessionId: $"day03-lab02-{config.StudentId}",
    cancellationToken: CancellationToken.None);

FinalFlowResult? finalResult = null;

// Consume workflow events: HITL approval via console, executor progress, output, and error handling
await foreach (var workflowEvent in run.WatchStreamAsync())
{
    switch (workflowEvent)
    {
        case RequestInfoEvent requestInfo:
            // Human-in-the-loop: trainer approves/rejects via console before the workflow proceeds
            var approvalResponse = ReadApprovalFromConsole(requestInfo.Request);
            await run.SendResponseAsync(approvalResponse);
            break;

        case ExecutorCompletedEvent completed:
            Console.WriteLine($"[workflow:event] executor_completed={completed.ExecutorId}");
            if (completed.Data is not null)
            {
                Console.WriteLine(ToJson(completed.Data));
            }
            break;

        case WorkflowOutputEvent output:
            Console.WriteLine("[workflow:event] output");
            Console.WriteLine(ToJson(output.Data ?? "(null workflow output)"));
            if (output.Is<FinalFlowResult>(out var typedResult))
            {
                finalResult = typedResult;
            }
            break;

        case ExecutorFailedEvent failed:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Executor '{failed.ExecutorId}' failed: {failed.Data}");
            Console.ResetColor();
            return;

        case WorkflowErrorEvent error:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(error.Exception?.ToString() ?? "Unknown workflow error.");
            Console.ResetColor();
            return;
    }

    if (finalResult is not null)
    {
        break;
    }
}

Day03Console.PrintLabStart(2);
Console.WriteLine("Final Flow Engineering Result");
Console.WriteLine("=============================");
Console.WriteLine(ToJson(finalResult ?? new FinalFlowResult(
    RequestId: input.RequestId,
    BatchId: input.BatchId,
    StudentId: input.StudentId,
    TerminalStatus: "no_output",
    Route: "unknown",
    Attempts: 0,
    Approval: "not_requested",
    Summary: "The workflow ended without a typed final result.",
    NextAction: "Inspect workflow events and rerun the lab.")));
Day03Console.PrintLabEnd(2);

Day03Console.PrintAppEnd();

static ExternalResponse ReadApprovalFromConsole(ExternalRequest request)
{
    if (!request.TryGetDataAs<ApprovalRequest>(out var approvalRequest))
    {
        throw new NotSupportedException($"Unsupported external request type: {request.PortInfo.RequestType}");
    }

    Console.WriteLine();
    Console.WriteLine("Human Approval Required");
    Console.WriteLine("=======================");
    Console.WriteLine(ToJson(approvalRequest));
    Console.Write("Trainer approval [approve/reject]: ");

    while (true)
    {
        var decision = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (decision is "approve" or "a" or "yes" or "y")
        {
            Console.Write("Approval note: ");
            var note = Console.ReadLine();
            return request.CreateResponse(new ApprovalDecision(
                approvalRequest.RequestId,
                Approved: true,
                ApprovedBy: Environment.UserName,
                Reason: string.IsNullOrWhiteSpace(note) ? "Approved during Day 3 Lab 02 HITL flow." : note.Trim(),
                ApprovalMode: "console-hitl"));
        }

        if (decision is "reject" or "r" or "no" or "n")
        {
            Console.Write("Rejection reason: ");
            var reason = Console.ReadLine();
            return request.CreateResponse(new ApprovalDecision(
                approvalRequest.RequestId,
                Approved: false,
                ApprovedBy: Environment.UserName,
                Reason: string.IsNullOrWhiteSpace(reason) ? "Rejected during Day 3 Lab 02 HITL flow." : reason.Trim(),
                ApprovalMode: "console-hitl"));
        }

        Console.Write("Please enter approve or reject: ");
    }
}

static string ToJson(object value) => JsonSerializer.Serialize(
    value,
    new JsonSerializerOptions { WriteIndented = true });

// Workflow factory: builds a typed executor graph with switch routing for approval branching and bounded retry
internal static class TrainingOperationsWorkflowFactory
{
    public static Workflow Build(Day03TrainingConfig config)
    {
        var intake = new IntakeExecutor(config);
        var classifier = new ClassifyExecutor();
        var riskRouter = new RiskRouterExecutor();
        var approvalRoute = new ApprovalRouteExecutor();
        var autoRoute = new AutoRouteExecutor();
        // RequestPort enables HITL via ExternalRequest/ExternalResponse pattern
        var approvalPort = RequestPort.Create<ApprovalRequest, ApprovalDecision>("TrainerApproval");
        var approvalToCommand = new ApprovalToCommandExecutor();
        var executor = new ExecuteTrainingOperationExecutor();
        var retryPlanner = new RetryPlannerExecutor();
        var summary = new SummaryExecutor();

        var builder = new WorkflowBuilder(intake)
            .WithName("Day 3 Lab 02 - Training Operations Flow")
            .WithDescription("Typed workflow with branching, real human approval, bounded retry, state, and termination.");

        builder
            .AddEdge(intake, classifier, "intake-to-classify")
            .AddEdge(classifier, riskRouter, "classify-to-risk-router");

        // RiskRouter emits a typed envelope. The switch cases keep the branch
        // explicit and type checked without using a prompt-only routing decision.
        // Switch routing: operational/sensitive requests go through HITL approval; informational auto-routes
        builder.AddSwitch(
            riskRouter,
            switchBuilder => switchBuilder
                .AddCase<RouteEnvelope>(route => route is not null && route.ApprovalRequest is not null, [approvalRoute])
                .WithDefault([autoRoute]));

        builder
            .AddEdge(approvalRoute, approvalPort, "approval-route-to-request-port")
            .AddEdge(autoRoute, executor, "auto-route-to-execution")
            .AddEdge(approvalPort, approvalToCommand, "approval-response-to-command")
            .AddEdge(approvalToCommand, executor, "approved-command-to-execution");

        // Bounded retry: if execution attempt needs retry and has not exceeded max attempts, route through RetryPlanner
        builder.AddSwitch(
            executor,
            switchBuilder => switchBuilder
                .AddCase<ExecutionAttemptResult>(
                    result => result is not null && result.NeedsRetry && result.Attempt < 2,
                    [retryPlanner])
                .WithDefault([summary]));

        builder
            .AddEdge(retryPlanner, executor, "retry-command-to-execution")
            .WithOutputFrom(summary);

        return builder.Build();
    }
}

// IntakeExecutor: receives the raw TrainingRequest, creates an IntakeRecord, and persists it to workflow state
internal sealed class IntakeExecutor(Day03TrainingConfig config)
    : Executor<TrainingRequest, IntakeRecord>("Intake")
{
    public override async ValueTask<IntakeRecord> HandleAsync(
        TrainingRequest message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var record = new IntakeRecord(
            message.RequestId,
            message.BatchId,
            message.StudentId,
            message.Text,
            ReceivedAtUtc: DateTimeOffset.UtcNow,
            TrainerProjectEndpoint: config.ProjectEndpoint);

        // QueueStateUpdateAsync persists data into workflow-scoped shared state for downstream executors
        await context.QueueStateUpdateAsync("request", record, SharedState.Scope, cancellationToken);
        await context.QueueStateUpdateAsync("batchId", message.BatchId, SharedState.Scope, cancellationToken);
        await context.QueueStateUpdateAsync("studentId", message.StudentId, SharedState.Scope, cancellationToken);

        return record;
    }
}

internal sealed class ClassifyExecutor()
    : Executor<IntakeRecord, ClassifiedRequest>("Classify")
{
    public override async ValueTask<ClassifiedRequest> HandleAsync(
        IntakeRecord message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var normalized = message.Text.ToLowerInvariant();

        var category = normalized.Contains("secret", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains("key", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains("credential", StringComparison.OrdinalIgnoreCase)
            ? RequestCategory.Sensitive
            : normalized.Contains("provision", StringComparison.OrdinalIgnoreCase)
                || normalized.Contains("deploy", StringComparison.OrdinalIgnoreCase)
                || normalized.Contains("delete", StringComparison.OrdinalIgnoreCase)
                || normalized.Contains("scale", StringComparison.OrdinalIgnoreCase)
                    ? RequestCategory.Operational
                    : RequestCategory.Informational;

        var classification = new ClassifiedRequest(
            message.RequestId,
            message.BatchId,
            message.StudentId,
            message.Text,
            category,
            category switch
            {
                RequestCategory.Sensitive => "Request mentions credentials/secrets or high-control access.",
                RequestCategory.Operational => "Request changes live training infrastructure or deployment state.",
                _ => "Request is informational and has no direct infrastructure side effect."
            });

        await context.QueueStateUpdateAsync("classification", classification, SharedState.Scope, cancellationToken);
        return classification;
    }
}

internal sealed class RiskRouterExecutor()
    : Executor<ClassifiedRequest, RouteEnvelope>("RiskRouter")
{
    public override async ValueTask<RouteEnvelope> HandleAsync(
        ClassifiedRequest message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var requiresApproval = message.Category is RequestCategory.Operational or RequestCategory.Sensitive;
        await context.QueueStateUpdateAsync("requiresApproval", requiresApproval, SharedState.Scope, cancellationToken);

        if (requiresApproval)
        {
            return new RouteEnvelope(
                ApprovalRequest: new ApprovalRequest(
                    message.RequestId,
                    message.BatchId,
                    message.StudentId,
                    message.Text,
                    RiskLevel: message.Category.ToString(),
                    Reason: message.Reason,
                    SuggestedAction: "Approve only if the operation is scoped to the current training batch and has a cleanup path."),
                ExecutionCommand: null);
        }

        return new RouteEnvelope(
            ApprovalRequest: null,
            ExecutionCommand: ExecutionCommand.FromAutoApproved(message));
    }
}

internal sealed class ApprovalRouteExecutor()
    : Executor<RouteEnvelope, ApprovalRequest>("ApprovalRoute")
{
    public override ValueTask<ApprovalRequest> HandleAsync(
        RouteEnvelope message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(message.ApprovalRequest
            ?? throw new InvalidOperationException("Approval route selected without an approval request."));
    }
}

internal sealed class AutoRouteExecutor()
    : Executor<RouteEnvelope, ExecutionCommand>("AutoRoute")
{
    public override ValueTask<ExecutionCommand> HandleAsync(
        RouteEnvelope message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(message.ExecutionCommand
            ?? throw new InvalidOperationException("Auto route selected without an execution command."));
    }
}

internal sealed class ApprovalToCommandExecutor()
    : Executor<ApprovalDecision, ExecutionCommand>("ApprovalToCommand")
{
    public override async ValueTask<ExecutionCommand> HandleAsync(
        ApprovalDecision message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var request = await context.ReadStateAsync<IntakeRecord>("request", SharedState.Scope, cancellationToken)
            ?? throw new InvalidOperationException("Workflow state 'request' was not found.");

        var classification = await context.ReadStateAsync<ClassifiedRequest>("classification", SharedState.Scope, cancellationToken)
            ?? throw new InvalidOperationException("Workflow state 'classification' was not found.");

        await context.QueueStateUpdateAsync("approval", message, SharedState.Scope, cancellationToken);

        return new ExecutionCommand(
            request.RequestId,
            request.BatchId,
            request.StudentId,
            request.Text,
            classification.Category,
            Approved: message.Approved,
            ApprovalMode: message.ApprovalMode,
            Attempt: 1,
            ApprovalReason: message.Reason);
    }
}

internal sealed class ExecuteTrainingOperationExecutor()
    : Executor<ExecutionCommand, ExecutionAttemptResult>("ExecuteTrainingOperation")
{
    public override async ValueTask<ExecutionAttemptResult> HandleAsync(
        ExecutionCommand message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var priorAttemptCount = await context.ReadStateAsync<int>("executionAttempts", SharedState.Scope, cancellationToken);
        await context.QueueStateUpdateAsync("executionAttempts", priorAttemptCount + 1, SharedState.Scope, cancellationToken);

        if (!message.Approved)
        {
            return new ExecutionAttemptResult(
                message.RequestId,
                message.BatchId,
                message.StudentId,
                message.Category,
                message.Attempt,
                Status: "denied",
                NeedsRetry: false,
                ApprovalMode: message.ApprovalMode,
                Evidence: "Human approval rejected the requested operation.",
                NextAction: "Stop the workflow and ask the requester to revise the request scope.");
        }

// First operational attempt is deliberately failed to demonstrate retry as explicit workflow control flow
        {
            return new ExecutionAttemptResult(
                message.RequestId,
                message.BatchId,
                message.StudentId,
                message.Category,
                message.Attempt,
                Status: "transient_policy_check_failed",
                NeedsRetry: true,
                ApprovalMode: message.ApprovalMode,
                Evidence: "First pass detected that weekend cleanup tags were not attached to the operation.",
                NextAction: "Retry after adding BatchId, StudentId, Environment, and DeleteAfter tags.");
        }

        return new ExecutionAttemptResult(
            message.RequestId,
            message.BatchId,
            message.StudentId,
            message.Category,
            message.Attempt,
            Status: "completed",
            NeedsRetry: false,
            ApprovalMode: message.ApprovalMode,
            Evidence: message.Category switch
            {
                RequestCategory.Informational => "Response prepared without infrastructure side effects.",
                RequestCategory.Sensitive => "Sensitive action completed only after approval boundary.",
                _ => "Operational action completed with required training-batch controls."
            },
            NextAction: "Emit the final workflow result.");
    }
}

internal sealed class RetryPlannerExecutor()
    : Executor<ExecutionAttemptResult, ExecutionCommand>("RetryPlanner")
{
    public override async ValueTask<ExecutionCommand> HandleAsync(
        ExecutionAttemptResult message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var request = await context.ReadStateAsync<IntakeRecord>("request", SharedState.Scope, cancellationToken)
            ?? throw new InvalidOperationException("Workflow state 'request' was not found.");

        var approval = await context.ReadStateAsync<ApprovalDecision>("approval", SharedState.Scope, cancellationToken);

        await context.QueueStateUpdateAsync(
            "retryReason",
            "Added required cost/control tags before retrying the operation.",
            SharedState.Scope,
            cancellationToken);

        return new ExecutionCommand(
            request.RequestId,
            request.BatchId,
            request.StudentId,
            request.Text,
            message.Category,
            Approved: true,
            ApprovalMode: approval?.ApprovalMode ?? "auto-no-hitl",
            Attempt: message.Attempt + 1,
            ApprovalReason: approval?.Reason ?? "No HITL required.");
    }
}

internal sealed class SummaryExecutor()
    : Executor<ExecutionAttemptResult, FinalFlowResult>("Summary")
{
    public override async ValueTask<FinalFlowResult> HandleAsync(
        ExecutionAttemptResult message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var classification = await context.ReadStateAsync<ClassifiedRequest>("classification", SharedState.Scope, cancellationToken);
        var requiresApproval = await context.ReadStateAsync<bool>("requiresApproval", SharedState.Scope, cancellationToken);
        var retryReason = await context.ReadStateAsync<string>("retryReason", SharedState.Scope, cancellationToken);

        return new FinalFlowResult(
            message.RequestId,
            message.BatchId,
            message.StudentId,
            TerminalStatus: message.Status,
            Route: requiresApproval ? "hitl-approval-route" : "auto-route",
            Attempts: message.Attempt,
            Approval: message.ApprovalMode,
            Summary: $"{classification?.Category.ToString() ?? "Unknown"} request ended with status '{message.Status}'. Evidence: {message.Evidence}",
            NextAction: retryReason is null
                ? message.NextAction
                : $"{message.NextAction} Retry evidence: {retryReason}");
    }
}

internal static class SharedState
{
    public const string Scope = "Day03Lab02Flow";
}

internal enum RequestCategory
{
    Informational,
    Operational,
    Sensitive
}

internal sealed record TrainingRequest(
    string RequestId,
    string BatchId,
    string StudentId,
    string Text);

internal sealed record IntakeRecord(
    string RequestId,
    string BatchId,
    string StudentId,
    string Text,
    DateTimeOffset ReceivedAtUtc,
    string TrainerProjectEndpoint);

internal sealed record ClassifiedRequest(
    string RequestId,
    string BatchId,
    string StudentId,
    string Text,
    RequestCategory Category,
    string Reason);

internal sealed record ApprovalRequest(
    string RequestId,
    string BatchId,
    string StudentId,
    string RequestText,
    string RiskLevel,
    string Reason,
    string SuggestedAction);

internal sealed record RouteEnvelope(
    ApprovalRequest? ApprovalRequest,
    ExecutionCommand? ExecutionCommand);

internal sealed record ApprovalDecision(
    string RequestId,
    bool Approved,
    string ApprovedBy,
    string Reason,
    string ApprovalMode);

internal sealed record ExecutionCommand(
    string RequestId,
    string BatchId,
    string StudentId,
    string RequestText,
    RequestCategory Category,
    bool Approved,
    string ApprovalMode,
    int Attempt,
    string ApprovalReason)
{
    public static ExecutionCommand FromAutoApproved(ClassifiedRequest request) => new(
        request.RequestId,
        request.BatchId,
        request.StudentId,
        request.Text,
        request.Category,
        Approved: true,
        ApprovalMode: "auto-no-hitl",
        Attempt: 1,
        ApprovalReason: "Informational request does not need HITL approval.");
}

internal sealed record ExecutionAttemptResult(
    string RequestId,
    string BatchId,
    string StudentId,
    RequestCategory Category,
    int Attempt,
    string Status,
    bool NeedsRetry,
    string ApprovalMode,
    string Evidence,
    string NextAction);

internal sealed record FinalFlowResult(
    string RequestId,
    string BatchId,
    string StudentId,
    string TerminalStatus,
    string Route,
    int Attempts,
    string Approval,
    string Summary,
    string NextAction);
