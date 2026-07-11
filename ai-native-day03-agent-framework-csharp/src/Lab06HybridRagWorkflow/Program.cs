#pragma warning disable MAAI001

using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.AI.Projects;
using Azure.Core;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.AI;
using Pronative.Day03.Shared;

var config = Day03TrainingConfig.Load(args);
var ragConfig = RetrievalGroundedRagConfig.Load(config, args);

Day03Console.PrintAppStart();
Day03Console.PrintHeader(config, "Lab 06 - Retrieval-Grounded RAG for Agentic Workflow");

Console.WriteLine("This lab uses Microsoft Agent Framework and Cosmos DB directly:");
Console.WriteLine("- WorkflowBuilder for explicit RAG orchestration");
Console.WriteLine("- typed Executor<TInput,TOutput> steps for query planning, answering, verification, retry, and finalization");
Console.WriteLine("- AIProjectClient.AsAIAgent(...) for the Foundry-backed answer agent");
Console.WriteLine("- AIFunctionFactory.Create(...) for the retrieval tool boundary");
Console.WriteLine("- Microsoft.Azure.Cosmos.Container query APIs for structured operational data retrieval");
Console.WriteLine("- WorkflowOutputEvent as the typed terminal result");
Console.WriteLine();

Console.WriteLine("Retrieval options covered conceptually");
Console.WriteLine("======================================");
Console.WriteLine("- Azure AI Search: dedicated enterprise search/index service; covered on Day 1; not required in this lab");
Console.WriteLine("- Cosmos DB: hands-on path for structured/semi-structured operational grounding");
Console.WriteLine("- HorizonDB: concept path for PostgreSQL-native vector, full-text, and hybrid retrieval");
Console.WriteLine();

Console.WriteLine("Live Azure dependencies");
Console.WriteLine("=======================");
Console.WriteLine($"Cosmos endpoint: {ragConfig.CosmosEndpoint}");
Console.WriteLine($"Cosmos database: {ragConfig.CosmosDatabase}");
Console.WriteLine($"Cosmos container: {ragConfig.CosmosContainer}");
Console.WriteLine($"Cosmos auth: {(string.IsNullOrWhiteSpace(ragConfig.CosmosKey) ? "AzureCliCredential / RBAC" : "COSMOS_KEY")}");
Console.WriteLine($"Retrieval mode: {ragConfig.RetrievalMode}");
Console.WriteLine($"Top records: {ragConfig.Top}");
Console.WriteLine($"Seed sample data: {ragConfig.SeedSampleData}");
Console.WriteLine();

// Optional: seed Cosmos DB with sample training knowledge records for retrieval grounding
if (ragConfig.SeedSampleData)
{
    Console.WriteLine("Seeding Cosmos DB sample training knowledge...");
    var seedSummary = await CosmosTrainingKnowledgeSeeder.SeedAsync(ragConfig, CancellationToken.None);
    Console.WriteLine(ToJson(seedSummary));
    Console.WriteLine();
}

Console.Write("Enter a grounded question, or press Enter for the default: ");
var question = Console.ReadLine();
if (string.IsNullOrWhiteSpace(question))
{
    question = """
    For Day 3 and Day 4 AI-Native training, what evidence exists about retrieval choices,
    workflow agents, skills, gateway control, and how Cosmos DB differs from Azure AI Search and HorizonDB?
    """;
}

// Workflow orchestrates query planning -> Foundry-backed answer agent -> grounding verification -> retry or finalize
var workflow = RetrievalGroundedRagWorkflowFactory.Build(config, ragConfig);
var input = new RagUserQuestion(
    RequestId: $"rag-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
    BatchId: config.BatchId,
    StudentId: config.StudentId,
    Question: question.Trim(),
    Attempt: 1);

await using var run = await InProcessExecution.RunStreamingAsync(
    workflow,
    input,
    sessionId: $"day03-lab06-{config.StudentId}",
    cancellationToken: CancellationToken.None);

FinalRetrievalGroundedRagResult? finalResult = null;

// Consume workflow events: executor progress, typed output, and error handling with early break on final result
await foreach (var workflowEvent in run.WatchStreamAsync())
{
    switch (workflowEvent)
    {
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
            if (output.Is<FinalRetrievalGroundedRagResult>(out var typedResult))
            {
                finalResult = typedResult;
            }
            break;

        case ExecutorFailedEvent failed:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Executor '{failed.ExecutorId}' failed.");
            Console.Error.WriteLine(failed.Data?.ToString() ?? "No error data returned.");
            Console.ResetColor();
            return;

        case WorkflowErrorEvent error:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Workflow failed.");
            Console.Error.WriteLine(error.Exception?.ToString() ?? "Unknown workflow error.");
            Console.ResetColor();
            return;
    }

    if (finalResult is not null)
    {
        break;
    }
}

Day03Console.PrintLabStart(6);
Console.WriteLine("Final Retrieval-Grounded RAG Result");
Console.WriteLine("===================================");
Console.WriteLine(ToJson(finalResult ?? FinalRetrievalGroundedRagResult.NoOutput(input, ragConfig)));
Day03Console.PrintLabEnd(6);

if (finalResult is not null)
{
    Directory.CreateDirectory(ragConfig.ArtifactsDirectory);
    var artifactPath = Path.Combine(ragConfig.ArtifactsDirectory, "day03-lab06-retrieval-grounded-rag-result.json");
    await File.WriteAllTextAsync(artifactPath, ToJson(finalResult), CancellationToken.None);
    Console.WriteLine();
    Console.WriteLine($"Evidence artifact: {artifactPath}");
}

Day03Console.PrintAppEnd();

static string ToJson(object value) => JsonSerializer.Serialize(
    value,
    new JsonSerializerOptions { WriteIndented = true });

// RAG workflow factory: query planning -> Foundry answer with retrieval -> verification -> retry or finalize
internal static class RetrievalGroundedRagWorkflowFactory
{
    public static Workflow Build(Day03TrainingConfig config, RetrievalGroundedRagConfig ragConfig)
    {
        var queryPlanner = new QueryPlanningExecutor(ragConfig);
        var answerAgent = new FoundryGroundedAnswerExecutor(config, ragConfig);
        var verifier = new GroundingVerificationExecutor();
        var retryPlanner = new RetrievalRetryPlannerExecutor(ragConfig);
        var finalizer = new FinalRagResultExecutor(ragConfig);

        var builder = new WorkflowBuilder(queryPlanner)
            .WithName("Day 3 Lab 06 - Retrieval-Grounded RAG Workflow")
            .WithDescription("Agentic RAG flow with Cosmos DB structured retrieval, Foundry answer agent, verification, bounded retry, and typed output.");

        builder
            .AddEdge(queryPlanner, answerAgent, "query-plan-to-grounded-answer")
            .AddEdge(answerAgent, verifier, "answer-to-grounding-verification");

        // Bounded retry: if grounding verification fails and attempts remain, expand query and retry
        builder.AddSwitch(
            verifier,
            switchBuilder => switchBuilder
                .AddCase<GroundingVerification>(
                    verification => verification is not null && verification.NeedsRetry && verification.Answer.Attempt < 2,
                    [retryPlanner])
                .WithDefault([finalizer]));

        builder
            .AddEdge(retryPlanner, answerAgent, "retry-query-to-grounded-answer")
            .WithOutputFrom(finalizer);

        return builder.Build();
    }
}

internal sealed class QueryPlanningExecutor(RetrievalGroundedRagConfig ragConfig)
    : Executor<RagUserQuestion, RagRetrievalPlan>("PlanRetrieval")
{
    public override async ValueTask<RagRetrievalPlan> HandleAsync(
        RagUserQuestion message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var plan = new RagRetrievalPlan(
            message.RequestId,
            message.BatchId,
            message.StudentId,
            message.Question,
            RetrievalQuery: NormalizeQuestionForRetrieval(message.Question),
            RetrievalMode: ragConfig.RetrievalMode,
            RetrievalProvider: "cosmos-db-structured",
            CosmosEndpoint: ragConfig.CosmosEndpoint,
            CosmosDatabase: ragConfig.CosmosDatabase,
            CosmosContainer: ragConfig.CosmosContainer,
            Top: ragConfig.Top,
            Attempt: message.Attempt,
            PlanningNote: "Use Cosmos DB operational knowledge records first; answer only with cited retrieved evidence.");

        await context.QueueStateUpdateAsync("rag.plan", plan, SharedState.Scope, cancellationToken);
        await context.QueueStateUpdateAsync("rag.requestId", message.RequestId, SharedState.Scope, cancellationToken);
        await context.QueueStateUpdateAsync("rag.studentId", message.StudentId, SharedState.Scope, cancellationToken);

        return plan;
    }

    private static string NormalizeQuestionForRetrieval(string question)
    {
        var trimmed = question.Trim();
        return trimmed.Length <= 260 ? trimmed : trimmed[..260];
    }
}

// Foundry-backed answer executor: creates a Cosmos DB retrieval tool, a Foundry agent with retrieve_training_context, and runs the RAG prompt
internal sealed class FoundryGroundedAnswerExecutor(Day03TrainingConfig config, RetrievalGroundedRagConfig ragConfig)
    : Executor<RagRetrievalPlan, GroundedRagAnswer>("AnswerWithFoundryAgent")
{
    public override async ValueTask<GroundedRagAnswer> HandleAsync(
        RagRetrievalPlan message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var retrievalTool = new CosmosDbRetrievalTool(ragConfig);
        var retrieveTrainingContext = AIFunctionFactory.Create(
            (Func<string, string>)retrievalTool.RetrieveTrainingContext,
            name: "retrieve_training_context",
            description: "Retrieve grounding evidence from Cosmos DB training knowledge records. Input is the retrieval query.");

        var projectClient = new AIProjectClient(new Uri(config.ProjectEndpoint), new AzureCliCredential());

        var agent = projectClient.AsAIAgent(
            model: config.ModelDeployment,
            name: $"day03-lab06-rag-{SanitizeAgentName(config.StudentId)}",
            description: "Day 3 Lab 06 Foundry-backed database-grounded RAG answer agent",
            instructions:
                """
                You are a ProNative AI grounded-answer agent running inside a Microsoft Agent Framework workflow.

                You must:
                - Call retrieve_training_context before answering.
                - Use only evidence returned by retrieve_training_context.
                - Cite each factual claim with citation markers in the form [doc:<id>].
                - If no retrieved evidence is relevant, say that the answer is not grounded and ask for better source data.
                - Do not invent document names, Azure resources, training policies, or platform decisions.
                """,
            tools: [retrieveTrainingContext]);

        var session = await agent.CreateSessionAsync();
        session.StateBag.SetValue("BatchId", message.BatchId);
        session.StateBag.SetValue("StudentId", message.StudentId);
        session.StateBag.SetValue("RetrievalProvider", message.RetrievalProvider);
        session.StateBag.SetValue("CosmosDatabase", message.CosmosDatabase);
        session.StateBag.SetValue("CosmosContainer", message.CosmosContainer);
        session.StateBag.SetValue("RetrievalMode", message.RetrievalMode);

        var prompt =
            $"""
            BatchId: {message.BatchId}
            StudentId: {message.StudentId}
            RetrievalProvider: {message.RetrievalProvider}
            RetrievalMode: {message.RetrievalMode}
            CosmosDatabase: {message.CosmosDatabase}
            CosmosContainer: {message.CosmosContainer}
            Attempt: {message.Attempt}

            Question:
            {message.Question}

            Retrieval query to use:
            {message.RetrievalQuery}

            Answer with these headings exactly:
            Answer
            Citations
            Grounding Notes

            Every factual claim must include [doc:<id>] citation markers from the retrieved records.
            """;

        var run = await agent.RunAsync(prompt, session);
        var responseText = run.ToString();
        var retrievalResult = retrievalTool.LastResult ?? RetrievalResult.Empty(
            message.RequestId,
            message.StudentId,
            message.RetrievalQuery,
            ragConfig);

        var answer = new GroundedRagAnswer(
            message.RequestId,
            message.BatchId,
            message.StudentId,
            message.Question,
            message.RetrievalQuery,
            message.RetrievalMode,
            message.RetrievalProvider,
            message.Attempt,
            responseText,
            retrievalResult.Documents,
            retrievalResult.Diagnostics,
            DateTimeOffset.UtcNow);

        await context.QueueStateUpdateAsync("rag.lastAnswer", answer, SharedState.Scope, cancellationToken);
        await context.QueueStateUpdateAsync("rag.lastDocumentCount", retrievalResult.Documents.Count, SharedState.Scope, cancellationToken);

        return answer;
    }

    private static string SanitizeAgentName(string studentId)
    {
        var safe = new string(studentId.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        return string.IsNullOrWhiteSpace(safe) ? "student" : safe;
    }
}

// Grounding verification: checks if the answer cites retrieved documents with [doc:<id>] markers or explicitly states it is not grounded
internal sealed class GroundingVerificationExecutor()
    : Executor<GroundedRagAnswer, GroundingVerification>("VerifyGrounding")
{
    public override async ValueTask<GroundingVerification> HandleAsync(
        GroundedRagAnswer message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var citedDocumentIds = message.RetrievedDocuments
            .Where(doc => message.AnswerText.Contains($"[doc:{doc.Id}]", StringComparison.OrdinalIgnoreCase))
            .Select(doc => doc.Id)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var hasRetrievedEvidence = message.RetrievedDocuments.Count > 0;
        var hasCitations = citedDocumentIds.Length > 0;
        var saysNotGrounded = message.AnswerText.Contains("not grounded", StringComparison.OrdinalIgnoreCase);
        var passes = hasRetrievedEvidence && (hasCitations || saysNotGrounded);

        var verification = new GroundingVerification(
            message.RequestId,
            message.BatchId,
            message.StudentId,
            message,
            Passes: passes,
            NeedsRetry: !passes,
            CitedDocumentIds: citedDocumentIds,
            Findings:
            [
                hasRetrievedEvidence
                    ? $"Retrieved {message.RetrievedDocuments.Count} record(s) from Cosmos DB."
                    : "No records were retrieved from Cosmos DB.",
                hasCitations
                    ? $"Answer cited {citedDocumentIds.Length} retrieved record(s)."
                    : "Answer did not cite retrieved records with [doc:<id>] markers.",
                saysNotGrounded
                    ? "Answer explicitly stated that the response is not grounded."
                    : "Answer did not explicitly identify lack of grounding."
            ]);

        await context.QueueStateUpdateAsync("rag.verification", verification, SharedState.Scope, cancellationToken);
        return verification;
    }
}

internal sealed class RetrievalRetryPlannerExecutor(RetrievalGroundedRagConfig ragConfig)
    : Executor<GroundingVerification, RagRetrievalPlan>("PlanRetrievalRetry")
{
    public override async ValueTask<RagRetrievalPlan> HandleAsync(
        GroundingVerification message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var retryQuery =
            $"{message.Answer.Question} ProNative AI-Native Day 3 Day 4 Cosmos DB HorizonDB Azure AI Search workflow agent grounding evidence";

        var retryPlan = new RagRetrievalPlan(
            message.RequestId,
            message.BatchId,
            message.StudentId,
            message.Answer.Question,
            RetrievalQuery: retryQuery,
            RetrievalMode: ragConfig.RetrievalMode,
            RetrievalProvider: "cosmos-db-structured",
            CosmosEndpoint: ragConfig.CosmosEndpoint,
            CosmosDatabase: ragConfig.CosmosDatabase,
            CosmosContainer: ragConfig.CosmosContainer,
            Top: Math.Max(ragConfig.Top, 5),
            Attempt: message.Answer.Attempt + 1,
            PlanningNote: "Retry with an expanded query because the first answer did not pass grounding verification.");

        await context.QueueStateUpdateAsync("rag.retryPlan", retryPlan, SharedState.Scope, cancellationToken);
        return retryPlan;
    }
}

internal sealed class FinalRagResultExecutor(RetrievalGroundedRagConfig ragConfig)
    : Executor<GroundingVerification, FinalRetrievalGroundedRagResult>("FinalizeRagResult")
{
    public override ValueTask<FinalRetrievalGroundedRagResult> HandleAsync(
        GroundingVerification message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var terminalStatus = message.Passes ? "grounded" : "not_grounded";
        var nextAction = message.Passes
            ? "Use the cited answer as the grounded response and inspect the evidence artifact."
            : "Check Cosmos DB training knowledge records, seed sample data, improve the query, or add missing operational evidence.";

        var result = new FinalRetrievalGroundedRagResult(
            message.RequestId,
            message.BatchId,
            message.StudentId,
            terminalStatus,
            message.Answer.Question,
            message.Answer.AnswerText,
            message.Answer.RetrievalMode,
            message.Answer.RetrievalProvider,
            ragConfig.CosmosEndpoint,
            ragConfig.CosmosDatabase,
            ragConfig.CosmosContainer,
            message.Answer.RetrievedDocuments,
            message.CitedDocumentIds,
            message.Findings,
            message.Answer.Attempt,
            nextAction,
            DateTimeOffset.UtcNow);

        return ValueTask.FromResult(result);
    }
}

// Cosmos DB retrieval tool: queries structured training knowledge records and scores them against the input query
internal sealed class CosmosDbRetrievalTool(RetrievalGroundedRagConfig config)
{
    public RetrievalResult? LastResult { get; private set; }

    public string RetrieveTrainingContext(string query)
    {
        LastResult = RetrieveTrainingContextAsync(query, CancellationToken.None).GetAwaiter().GetResult();
        return JsonSerializer.Serialize(LastResult, new JsonSerializerOptions { WriteIndented = true });
    }

    private async Task<RetrievalResult> RetrieveTrainingContextAsync(string query, CancellationToken cancellationToken)
    {
        using var client = CosmosClientFactory.Create(config);
        var container = client.GetContainer(config.CosmosDatabase, config.CosmosContainer);

        var queryDefinition = new QueryDefinition(
                """
                SELECT * FROM c
                WHERE c.batchId = @batchId
                AND c.documentType = @documentType
                AND (c.studentId = @studentId OR c.studentId = 'shared')
                """)
            .WithParameter("@batchId", config.BatchId)
            .WithParameter("@documentType", "training-knowledge")
            .WithParameter("@studentId", config.StudentId);

        using var iterator = container.GetItemQueryIterator<TrainingKnowledgeRecord>(
            queryDefinition,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(config.BatchId),
                MaxItemCount = 50
            });

        var allRecords = new List<TrainingKnowledgeRecord>();
        var requestCharge = 0.0;
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            requestCharge += response.RequestCharge;
            allRecords.AddRange(response);
        }

        var rankedDocuments = allRecords
            .Select(record => new
            {
                Record = record,
                Score = ScoreRecord(record, query)
            })
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.Record.Category)
            .ThenBy(item => item.Record.Title)
            .Take(config.Top)
            .Select(item => ToRetrievedDocument(item.Record, item.Score))
            .ToArray();

        return new RetrievalResult(
            RequestId: $"cosmos-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}",
            StudentId: config.StudentId,
            Query: query,
            RetrievalProvider: "cosmos-db-structured",
            CosmosEndpoint: config.CosmosEndpoint,
            CosmosDatabase: config.CosmosDatabase,
            CosmosContainer: config.CosmosContainer,
            RetrievalMode: config.RetrievalMode,
            Documents: rankedDocuments,
            Diagnostics: new RetrievalDiagnostics(
                CandidateCount: allRecords.Count,
                ReturnedCount: rankedDocuments.Length,
                RequestCharge: requestCharge,
                AuthMode: string.IsNullOrWhiteSpace(config.CosmosKey) ? "AzureCliCredential/RBAC" : "COSMOS_KEY",
                Note: rankedDocuments.Length == 0
                    ? "Cosmos DB returned zero records. Seed sample data or check batch/student filters."
                    : "Cosmos DB returned structured operational grounding records."));
    }

    private static RetrievedDocument ToRetrievedDocument(TrainingKnowledgeRecord record, double score) =>
        new(
            Id: record.Id,
            Title: record.Title,
            Content: record.Content,
            Source: record.Source,
            Category: record.Category,
            Tags: record.Tags,
            Score: score);

    private static double ScoreRecord(TrainingKnowledgeRecord record, string query)
    {
        var tokens = Tokenize(query);
        if (tokens.Length == 0)
        {
            return record.Priority;
        }

        var searchable = string.Join(
            ' ',
            record.Title,
            record.Category,
            record.Content,
            record.RelevanceHint,
            string.Join(' ', record.Tags)).ToLowerInvariant();

        var tokenMatches = tokens.Count(searchable.Contains);
        var categoryBoost = record.Category.Contains("retrieval", StringComparison.OrdinalIgnoreCase) ? 2.0 : 0.0;
        return record.Priority + tokenMatches + categoryBoost;
    }

    private static string[] Tokenize(string query) =>
        query
            .ToLowerInvariant()
            .Split([' ', ',', '.', ';', ':', '?', '!', '\r', '\n', '\t', '-', '/', '(', ')'], StringSplitOptions.RemoveEmptyEntries)
            .Where(token => token.Length >= 4)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}

// Seeds Cosmos DB with training knowledge records from a JSON file; handles database/container creation if configured
internal static class CosmosTrainingKnowledgeSeeder
{
    public static async Task<SeedSummary> SeedAsync(
        RetrievalGroundedRagConfig config,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(config.SeedDataPath))
        {
            throw new FileNotFoundException("Cosmos seed data file was not found.", config.SeedDataPath);
        }

        var seedJson = await File.ReadAllTextAsync(config.SeedDataPath, cancellationToken);
        var records = JsonSerializer.Deserialize<List<TrainingKnowledgeRecord>>(
            seedJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        using var client = CosmosClientFactory.Create(config);
        Database database;
        Container container;

        if (config.CreateCosmosIfNotExists)
        {
            database = await client.CreateDatabaseIfNotExistsAsync(config.CosmosDatabase, cancellationToken: cancellationToken);
            container = await database.CreateContainerIfNotExistsAsync(
                id: config.CosmosContainer,
                partitionKeyPath: "/batchId",
                throughput: null,
                cancellationToken: cancellationToken);
        }
        else
        {
            database = client.GetDatabase(config.CosmosDatabase);
            container = database.GetContainer(config.CosmosContainer);
        }

        var upserted = 0;
        foreach (var record in records)
        {
            var normalized = record with
            {
                BatchId = string.IsNullOrWhiteSpace(record.BatchId) ? config.BatchId : record.BatchId,
                StudentId = string.Equals(record.StudentId, "{studentId}", StringComparison.OrdinalIgnoreCase)
                    ? config.StudentId
                    : record.StudentId,
                DocumentType = "training-knowledge"
            };

            await container.UpsertItemAsync(
                normalized,
                new PartitionKey(normalized.BatchId),
                cancellationToken: cancellationToken);
            upserted++;
        }

        return new SeedSummary(
            config.CosmosDatabase,
            config.CosmosContainer,
            config.SeedDataPath,
            upserted,
            config.CreateCosmosIfNotExists);
    }
}

internal static class CosmosClientFactory
{
    public static CosmosClient Create(RetrievalGroundedRagConfig config)
    {
        var options = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };

        return string.IsNullOrWhiteSpace(config.CosmosKey)
            ? new CosmosClient(config.CosmosEndpoint, new AzureCliCredential(), options)
            : new CosmosClient(config.CosmosEndpoint, config.CosmosKey, options);
    }
}

internal static class SharedState
{
    public const string Scope = "Day03Lab06RetrievalGroundedRag";
}

internal sealed record RetrievalGroundedRagConfig(
    string BatchId,
    string StudentId,
    string CosmosEndpoint,
    string CosmosDatabase,
    string CosmosContainer,
    string? CosmosKey,
    bool CreateCosmosIfNotExists,
    bool SeedSampleData,
    string SeedDataPath,
    string RetrievalMode,
    int Top,
    string ArtifactsDirectory)
{
    public static RetrievalGroundedRagConfig Load(Day03TrainingConfig config, string[] args)
    {
        var seedRequested = args.Any(arg => string.Equals(arg, "--seed", StringComparison.OrdinalIgnoreCase))
            || IsTruthy(Environment.GetEnvironmentVariable("COSMOS_SEED_SAMPLE_DATA"));

        return new RetrievalGroundedRagConfig(
            config.BatchId,
            config.StudentId,
            Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")
                ?? Environment.GetEnvironmentVariable("PN_COSMOS_ENDPOINT")
                ?? "https://cosmos-an2607101.documents.azure.com:443/",
            Environment.GetEnvironmentVariable("COSMOS_DATABASE")
                ?? Environment.GetEnvironmentVariable("PN_COSMOS_DATABASE")
                ?? "db-an2607101-training",
            Environment.GetEnvironmentVariable("COSMOS_CONTAINER")
                ?? Environment.GetEnvironmentVariable("PN_COSMOS_KNOWLEDGE_CONTAINER")
                ?? "training-knowledge",
            Environment.GetEnvironmentVariable("COSMOS_KEY"),
            IsTruthy(Environment.GetEnvironmentVariable("COSMOS_CREATE_IF_NOT_EXISTS")),
            seedRequested,
            Environment.GetEnvironmentVariable("COSMOS_SEED_DATA_PATH")
                ?? Path.Combine(AppContext.BaseDirectory, "data", "lab06-cosmos-training-knowledge.json"),
            Environment.GetEnvironmentVariable("RETRIEVAL_RAG_MODE")
                ?? "cosmos-structured",
            int.TryParse(Environment.GetEnvironmentVariable("RETRIEVAL_RAG_TOP"), out var top) && top > 0
                ? top
                : 5,
            Path.Combine(AppContext.BaseDirectory, "artifacts", config.StudentId));
    }

    private static bool IsTruthy(string? value) =>
        value is not null
        && (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase));
}

internal sealed record RagUserQuestion(
    string RequestId,
    string BatchId,
    string StudentId,
    string Question,
    int Attempt);

internal sealed record RagRetrievalPlan(
    string RequestId,
    string BatchId,
    string StudentId,
    string Question,
    string RetrievalQuery,
    string RetrievalMode,
    string RetrievalProvider,
    string CosmosEndpoint,
    string CosmosDatabase,
    string CosmosContainer,
    int Top,
    int Attempt,
    string PlanningNote);

internal sealed record RetrievedDocument(
    string Id,
    string Title,
    string Content,
    string Source,
    string Category,
    IReadOnlyList<string> Tags,
    double? Score);

internal sealed record RetrievalDiagnostics(
    int CandidateCount,
    int ReturnedCount,
    double RequestCharge,
    string AuthMode,
    string Note);

internal sealed record RetrievalResult(
    string RequestId,
    string StudentId,
    string Query,
    string RetrievalProvider,
    string CosmosEndpoint,
    string CosmosDatabase,
    string CosmosContainer,
    string RetrievalMode,
    IReadOnlyList<RetrievedDocument> Documents,
    RetrievalDiagnostics Diagnostics)
{
    public static RetrievalResult Empty(
        string requestId,
        string studentId,
        string query,
        RetrievalGroundedRagConfig config) =>
        new(
            requestId,
            studentId,
            query,
            "cosmos-db-structured",
            config.CosmosEndpoint,
            config.CosmosDatabase,
            config.CosmosContainer,
            config.RetrievalMode,
            [],
            new RetrievalDiagnostics(
                CandidateCount: 0,
                ReturnedCount: 0,
                RequestCharge: 0,
                AuthMode: string.IsNullOrWhiteSpace(config.CosmosKey) ? "AzureCliCredential/RBAC" : "COSMOS_KEY",
                Note: "The retrieval tool was not called or returned no records."));
}

internal sealed record GroundedRagAnswer(
    string RequestId,
    string BatchId,
    string StudentId,
    string Question,
    string RetrievalQuery,
    string RetrievalMode,
    string RetrievalProvider,
    int Attempt,
    string AnswerText,
    IReadOnlyList<RetrievedDocument> RetrievedDocuments,
    RetrievalDiagnostics RetrievalDiagnostics,
    DateTimeOffset AnsweredAtUtc);

internal sealed record GroundingVerification(
    string RequestId,
    string BatchId,
    string StudentId,
    GroundedRagAnswer Answer,
    bool Passes,
    bool NeedsRetry,
    IReadOnlyList<string> CitedDocumentIds,
    IReadOnlyList<string> Findings);

internal sealed record FinalRetrievalGroundedRagResult(
    string RequestId,
    string BatchId,
    string StudentId,
    string TerminalStatus,
    string Question,
    string Answer,
    string RetrievalMode,
    string RetrievalProvider,
    string CosmosEndpoint,
    string CosmosDatabase,
    string CosmosContainer,
    IReadOnlyList<RetrievedDocument> RetrievedDocuments,
    IReadOnlyList<string> CitedDocumentIds,
    IReadOnlyList<string> VerificationFindings,
    int Attempts,
    string NextAction,
    DateTimeOffset CompletedAtUtc)
{
    public static FinalRetrievalGroundedRagResult NoOutput(RagUserQuestion input, RetrievalGroundedRagConfig config) =>
        new(
            input.RequestId,
            input.BatchId,
            input.StudentId,
            "no_output",
            input.Question,
            "The workflow ended without a typed final result.",
            config.RetrievalMode,
            "cosmos-db-structured",
            config.CosmosEndpoint,
            config.CosmosDatabase,
            config.CosmosContainer,
            [],
            [],
            ["No WorkflowOutputEvent carrying FinalRetrievalGroundedRagResult was observed."],
            input.Attempt,
            "Inspect workflow events and rerun the lab.",
            DateTimeOffset.UtcNow);
}

internal sealed record TrainingKnowledgeRecord
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    public string BatchId { get; init; } = "AN-2607-101";
    public string StudentId { get; init; } = "shared";
    public string DocumentType { get; init; } = "training-knowledge";
    public string Title { get; init; } = "";
    public string Category { get; init; } = "";
    public string Content { get; init; } = "";
    public string Source { get; init; } = "";
    public string RelevanceHint { get; init; } = "";
    public int Priority { get; init; } = 1;
    public IReadOnlyList<string> Tags { get; init; } = [];
}

internal sealed record SeedSummary(
    string CosmosDatabase,
    string CosmosContainer,
    string SeedDataPath,
    int UpsertedRecords,
    bool CreatedDatabaseOrContainerIfMissing);
