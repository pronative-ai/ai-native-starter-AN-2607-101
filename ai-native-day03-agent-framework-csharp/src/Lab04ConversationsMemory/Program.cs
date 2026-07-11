#pragma warning disable MAAI001

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Compaction;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Pronative.Day03.Shared;

var config = Day03TrainingConfig.Load(args);

Day03Console.PrintAppStart();
Day03Console.PrintHeader(config, "Lab 04 - Conversations, Session, Context, Storage, and Compaction");

Console.WriteLine("This lab uses Microsoft Agent Framework conversation primitives:");
Console.WriteLine("- AgentSession for conversation continuity");
Console.WriteLine("- AIContextProvider for session-scoped memory/context");
Console.WriteLine("- InMemoryChatHistoryProvider for local history stored in the session");
Console.WriteLine("- session serialization for restart-safe checkpoints");
Console.WriteLine("- optional Cosmos DB persistence for durable session checkpoints");
Console.WriteLine("- CompactionProvider and compaction strategies for context growth control");
Console.WriteLine();

var artifactsDirectory = Path.Combine(AppContext.BaseDirectory, "artifacts");
Directory.CreateDirectory(artifactsDirectory);
var cosmosPersistenceConfig = Lab04CosmosPersistenceConfig.Load(config);

// Custom context provider: stores learner preferences (language, task, issue) in session state
var preferenceProvider = new TrainingPreferenceContextProvider();

// In-memory chat history provider with a reducer that caps conversation history at 8 messages
var historyProvider = new InMemoryChatHistoryProvider(new InMemoryChatHistoryProviderOptions
{
    StateKey = "day03-lab04-chat-history",
    ChatReducer = new MessageCountingChatReducer(targetCount: 8),
    ReducerTriggerEvent = InMemoryChatHistoryProviderOptions.ChatReducerTriggerEvent.BeforeMessagesRetrieval
});

// Compaction pipeline chains strategies: sliding window (drops oldest turns) then truncation (caps total messages)
var compactionPipeline = new PipelineCompactionStrategy(
[
    new SlidingWindowCompactionStrategy(
        trigger: CompactionTriggers.TurnsExceed(3),
        minimumPreservedTurns: 2,
        target: CompactionTriggers.TurnsExceed(2)),
    new TruncationCompactionStrategy(
        trigger: CompactionTriggers.MessagesExceed(10),
        minimumPreservedGroups: 6,
        target: CompactionTriggers.MessagesExceed(8))
]);

// CompactionProvider wraps the pipeline and manages state in AgentSession.StateBag
var compactionProvider = new CompactionProvider(
    compactionPipeline,
    stateKey: "day03-lab04-compaction",
    loggerFactory: NullLoggerFactory.Instance);

using var chatClient = new TrainingEchoChatClient();

// Agent wired with history provider and context providers (preferences + compaction)
// All session-scoped state flows through AgentSession.StateBag
var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
{
    Name = "day03-lab04-conversation-memory-agent",
    Description = "Demonstrates Agent Framework conversations, context providers, storage, and compaction.",
    ChatOptions = new ChatOptions
    {
        Instructions =
            """
            You are a ProNative training support agent.
            Use conversation history and injected context to keep the learner's preferences, task, and constraints visible.
            Keep responses short and operational.
            """
    },
    ChatHistoryProvider = historyProvider,
    AIContextProviders =
    [
        preferenceProvider,
        compactionProvider
    ]
});

Console.WriteLine("Step 1 - Create a session");
Console.WriteLine("=========================");
// AgentSession is the unit of conversation continuity
AgentSession session = await agent.CreateSessionAsync();
// StateBag holds session-scoped metadata visible to context providers
session.StateBag.SetValue("batchId", config.BatchId);
session.StateBag.SetValue("studentId", config.StudentId);
session.StateBag.SetValue("taskId", $"{config.StudentId}-environment-remediation");
session.StateBag.SetValue("taskStatus", "open");
PrintSessionState(session);
Console.WriteLine();

Console.WriteLine("Step 2 - Run multiple turns on the same AgentSession");
Console.WriteLine("===================================================");
// Each RunAsync call preserves history and updates context provider state within the session
await RunTurnAsync(agent, session, "Remember that I prefer C# examples and I am currently fixing Cosmos DB grounding access.");
await RunTurnAsync(agent, session, "My task is to prepare the Day 3 learner environment for Agent Framework labs.");
await RunTurnAsync(agent, session, "What should you remember before giving me the next action?");

Console.WriteLine();
Console.WriteLine("Step 3 - Inspect local in-memory chat history");
Console.WriteLine("============================================");
// In-memory history is stored in AgentSession and keyed by the provider's StateKey
if (session.TryGetInMemoryChatHistory(out List<ChatMessage>? history, stateKey: "day03-lab04-chat-history"))
{
    Console.WriteLine($"History messages stored in AgentSession: {history.Count}");
    foreach (var message in history.TakeLast(6))
    {
        Console.WriteLine($"- {message.Role}: {OneLine(message.Text)}");
    }
}
else
{
    Console.WriteLine("No in-memory history was found. Check the history provider state key.");
}

Console.WriteLine();
Console.WriteLine("Step 4 - Serialize and restore the session");
Console.WriteLine("==========================================");
// SerializeSessionAsync captures full session state (history, state bag, context) into a JSON checkpoint
var serializedSession = await agent.SerializeSessionAsync(session);
var checkpointPath = Path.Combine(artifactsDirectory, $"{config.StudentId}-lab04-session.json");
await File.WriteAllTextAsync(
    checkpointPath,
    JsonSerializer.Serialize(serializedSession, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"Serialized checkpoint written to: {checkpointPath}");

// DeserializeSessionAsync restores a full session from JSON, recreating history, state bag, and context
var restoredJson = JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync(checkpointPath));
AgentSession restoredSession = await agent.DeserializeSessionAsync(restoredJson);
Console.WriteLine("Restored session state:");
PrintSessionState(restoredSession);
Console.WriteLine();

Console.WriteLine("Step 5 - Persist and restore the session checkpoint from Cosmos DB");
Console.WriteLine("=================================================================");
AgentSession continuationSession = restoredSession;
if (cosmosPersistenceConfig.Enabled)
{
    Console.WriteLine($"Cosmos endpoint:  {cosmosPersistenceConfig.Endpoint}");
    Console.WriteLine($"Cosmos database:  {cosmosPersistenceConfig.Database}");
    Console.WriteLine($"Cosmos container: {cosmosPersistenceConfig.Container}");
    Console.WriteLine($"Checkpoint id:    {cosmosPersistenceConfig.CheckpointId}");

    // CosmosSessionCheckpointStore provides durable, partition-keyed persistence for session checkpoints
    var checkpointStore = new CosmosSessionCheckpointStore(cosmosPersistenceConfig);
    var checkpointDocument = AgentSessionCheckpointDocument.Create(
        config,
        cosmosPersistenceConfig,
        serializedSession);

    await checkpointStore.UpsertAsync(checkpointDocument);
    Console.WriteLine("Cosmos checkpoint upserted.");

    var persistedCheckpoint = await checkpointStore.ReadAsync(
        checkpointDocument.Id,
        checkpointDocument.BatchId);
    Console.WriteLine($"Cosmos checkpoint loaded. Created at: {persistedCheckpoint.CreatedAt:O}");

    var persistedSessionJson = JsonSerializer.Deserialize<JsonElement>(
        persistedCheckpoint.SessionPayloadJson);
    continuationSession = await agent.DeserializeSessionAsync(persistedSessionJson);
    Console.WriteLine("Restored session state from Cosmos DB checkpoint:");
    PrintSessionState(continuationSession);
}
else
{
    Console.WriteLine("Cosmos persistence is disabled for this run.");
    Console.WriteLine("Set ENABLE_COSMOS_PERSISTENCE=true to persist the serialized AgentSession checkpoint to Cosmos DB.");
    Console.WriteLine("Optional trainer setup can also set COSMOS_CREATE_IF_NOT_EXISTS=true.");
}
Console.WriteLine();

Console.WriteLine("Step 6 - Continue after restore");
Console.WriteLine("===============================");
// Restored session retains full history and state bag; context providers rehydrate from session state
await RunTurnAsync(agent, continuationSession, "Continue from the checkpoint. What is my preferred language and current task?");

Console.WriteLine();
Console.WriteLine("Step 7 - Run explicit compaction over a long message list");
Console.WriteLine("========================================================");
// CompactionProvider.CompactAsync applies the pipeline strategies to trim oversized history outside a running session
var longHistory = BuildLongConversation();
Console.WriteLine($"Before compaction: {longHistory.Count} messages");

var compacted = (await CompactionProvider.CompactAsync(
        compactionPipeline,
        longHistory,
        NullLogger.Instance,
        CancellationToken.None))
    .ToList();

Console.WriteLine($"After compaction:  {compacted.Count} messages");
foreach (var message in compacted)
{
    Console.WriteLine($"- {message.Role}: {OneLine(message.Text)}");
}

Day03Console.PrintLabStart(4);
Console.WriteLine("Trainer Checkpoint");
Console.WriteLine("==================");
Console.WriteLine("1. Session is the unit of conversation continuity.");
Console.WriteLine("2. Context providers store session-specific memory in AgentSession.StateBag, not in provider fields.");
Console.WriteLine("3. In-memory history is stored in the session and can be reduced with MessageCountingChatReducer.");
Console.WriteLine("4. Serialization creates a restart-safe checkpoint; treat it as sensitive data.");
Console.WriteLine("5. Cosmos DB stores the serialized checkpoint durably; it does not replace Agent Framework memory primitives.");
Console.WriteLine("6. Compaction is for framework-managed in-memory history; service-managed agents already manage server-side context.");
Day03Console.PrintLabEnd(4);

Day03Console.PrintAppEnd();

static async Task RunTurnAsync(AIAgent agent, AgentSession session, string userMessage)
{
    Console.WriteLine($"User:      {userMessage}");
    AgentResponse response = await agent.RunAsync(userMessage, session);
    Console.WriteLine($"Assistant: {OneLine(response.Text)}");
    Console.WriteLine();
}

static void PrintSessionState(AgentSession session)
{
    Console.WriteLine($"BatchId:    {session.StateBag.GetValue<string>("batchId")}");
    Console.WriteLine($"StudentId:  {session.StateBag.GetValue<string>("studentId")}");
    Console.WriteLine($"TaskId:     {session.StateBag.GetValue<string>("taskId")}");
    Console.WriteLine($"TaskStatus: {session.StateBag.GetValue<string>("taskStatus")}");
}

static List<ChatMessage> BuildLongConversation()
{
    List<ChatMessage> messages =
    [
        new(ChatRole.System, "You are a ProNative training support agent.")
    ];

    for (var turn = 1; turn <= 6; turn++)
    {
        messages.Add(new ChatMessage(ChatRole.User, $"Turn {turn}: learner reports progress on Agent Framework lab section {turn}."));
        messages.Add(new ChatMessage(ChatRole.Assistant, $"Turn {turn}: recorded progress and suggested the next safe action."));
    }

    return messages;
}

static string OneLine(string? text)
{
    if (string.IsNullOrWhiteSpace(text))
    {
        return "<empty>";
    }

    return text.ReplaceLineEndings(" ").Trim();
}

// Custom AIContextProvider: reads/writes learner preference facts from AgentSession.StateBag
// Demonstrates the two-phase lifecycle: ProvideAIContextAsync (inject context before LLM call)
// and StoreAIContextAsync (persist extracted facts after LLM call)
internal sealed class TrainingPreferenceContextProvider : AIContextProvider
{
    private readonly ProviderSessionState<PreferenceState> _sessionState = new(
        _ => new PreferenceState(),
        "day03-lab04-preferences",
        jsonSerializerOptions: null);

    public override IReadOnlyList<string> StateKeys => [_sessionState.StateKey];

    protected override ValueTask<AIContext> ProvideAIContextAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        var state = _sessionState.GetOrInitializeState(context.Session);
        if (state.Facts.Count == 0)
        {
            return new ValueTask<AIContext>(new AIContext());
        }

        var facts = string.Join("; ", state.Facts.Select(f => $"{f.Key}: {f.Value}"));
        return new ValueTask<AIContext>(new AIContext
        {
            Messages =
            [
                new ChatMessage(
                    ChatRole.User,
                    $"Session memory from TrainingPreferenceContextProvider: {facts}")
            ]
        });
    }

    protected override ValueTask StoreAIContextAsync(
        InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        var state = _sessionState.GetOrInitializeState(context.Session);
        foreach (var message in context.RequestMessages)
        {
            CaptureKnownFact(state, message.Text);
        }

        _sessionState.SaveState(context.Session, state);
        return default;
    }

    private static void CaptureKnownFact(PreferenceState state, string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (text.Contains("prefer C#", StringComparison.OrdinalIgnoreCase))
        {
            state.Facts["preferred-language"] = "C#";
        }

        if (text.Contains("Cosmos DB", StringComparison.OrdinalIgnoreCase))
        {
            state.Facts["current-issue"] = "Cosmos DB grounding access";
        }

        if (text.Contains("Day 3 learner environment", StringComparison.OrdinalIgnoreCase))
        {
            state.Facts["current-task"] = "prepare Day 3 learner environment for Agent Framework labs";
        }
    }

    internal sealed class PreferenceState
    {
        [JsonPropertyName("facts")]
        public Dictionary<string, string> Facts { get; set; } = [];
    }
}

// Configuration record for Cosmos DB checkpoint persistence; reads from environment variables with fallback defaults
internal sealed record Lab04CosmosPersistenceConfig(
    bool Enabled,
    bool CreateIfNotExists,
    string Endpoint,
    string Database,
    string Container,
    string? Key,
    string CheckpointId)
{
    public static Lab04CosmosPersistenceConfig Load(Day03TrainingConfig config)
    {
        var enabled = IsEnabled("ENABLE_COSMOS_PERSISTENCE")
            || IsEnabled("LAB04_ENABLE_COSMOS_PERSISTENCE");

        var container = Environment.GetEnvironmentVariable("COSMOS_SESSION_CONTAINER")
            ?? Environment.GetEnvironmentVariable("COSMOS_CHECKPOINT_CONTAINER")
            ?? "agent-session-checkpoints";

        return new Lab04CosmosPersistenceConfig(
            enabled,
            IsEnabled("COSMOS_CREATE_IF_NOT_EXISTS"),
            Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")
                ?? Environment.GetEnvironmentVariable("PN_COSMOS_ENDPOINT")
                ?? "https://cosmos-an2607101.documents.azure.com:443/",
            Environment.GetEnvironmentVariable("COSMOS_DATABASE")
                ?? Environment.GetEnvironmentVariable("PN_COSMOS_DATABASE")
                ?? "db-an2607101-training",
            container,
            Environment.GetEnvironmentVariable("COSMOS_KEY")
                ?? Environment.GetEnvironmentVariable("PN_COSMOS_KEY"),
            Environment.GetEnvironmentVariable("COSMOS_SESSION_CHECKPOINT_ID")
                ?? $"{config.StudentId.ToLowerInvariant()}-day03-lab04-session");
    }

    private static bool IsEnabled(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return bool.TryParse(value, out var enabled) && enabled;
    }
}

// Durable checkpoint store backed by Cosmos DB; uses /batchId as partition key for tenant isolation
internal sealed class CosmosSessionCheckpointStore(Lab04CosmosPersistenceConfig config)
{
    public async Task UpsertAsync(AgentSessionCheckpointDocument checkpoint)
    {
        using var client = CreateClient();
        var container = await GetContainerAsync(client);
        await container.UpsertItemAsync(checkpoint, new PartitionKey(checkpoint.BatchId));
    }

    public async Task<AgentSessionCheckpointDocument> ReadAsync(string id, string batchId)
    {
        using var client = CreateClient();
        var container = await GetContainerAsync(client);
        var response = await container.ReadItemAsync<AgentSessionCheckpointDocument>(
            id,
            new PartitionKey(batchId));
        return response.Resource;
    }

    private CosmosClient CreateClient()
    {
        var options = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };

        return string.IsNullOrWhiteSpace(config.Key)
            ? new CosmosClient(config.Endpoint, new AzureCliCredential(), options)
            : new CosmosClient(config.Endpoint, config.Key, options);
    }

    private async Task<Container> GetContainerAsync(CosmosClient client)
    {
        if (!config.CreateIfNotExists)
        {
            return client.GetContainer(config.Database, config.Container);
        }

        var database = await client.CreateDatabaseIfNotExistsAsync(config.Database);
        var container = await database.Database.CreateContainerIfNotExistsAsync(
            config.Container,
            "/batchId");

        return container.Container;
    }
}

// Document model for Cosmos DB checkpoint; carries serialized AgentSession payload alongside metadata
internal sealed class AgentSessionCheckpointDocument
{
    [Newtonsoft.Json.JsonProperty("id")]
    public string Id { get; init; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("batchId")]
    public string BatchId { get; init; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("studentId")]
    public string StudentId { get; init; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("labId")]
    public string LabId { get; init; } = "day03-lab04";

    [Newtonsoft.Json.JsonProperty("checkpointType")]
    public string CheckpointType { get; init; } = "agent-session";

    [Newtonsoft.Json.JsonProperty("createdAt")]
    public DateTimeOffset CreatedAt { get; init; }

    [Newtonsoft.Json.JsonProperty("sessionPayloadJson")]
    public string SessionPayloadJson { get; init; } = "{}";

    [Newtonsoft.Json.JsonProperty("notes")]
    public string Notes { get; init; } =
        "Serialized AgentSession checkpoint. Treat as sensitive; do not store secrets in session state.";

    public static AgentSessionCheckpointDocument Create(
        Day03TrainingConfig config,
        Lab04CosmosPersistenceConfig cosmosConfig,
        JsonElement serializedSession)
    {
        return new AgentSessionCheckpointDocument
        {
            Id = cosmosConfig.CheckpointId,
            BatchId = config.BatchId,
            StudentId = config.StudentId,
            CreatedAt = DateTimeOffset.UtcNow,
            SessionPayloadJson = JsonSerializer.Serialize(
                serializedSession,
                new JsonSerializerOptions { WriteIndented = false })
        };
    }
}

// Echo IChatClient for training: injects context provider memory into responses instead of calling a real LLM
internal sealed class TrainingEchoChatClient : IChatClient
{
    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var messageList = messages.ToList();
        var memoryMessage = messageList.LastOrDefault(m => m.Text.Contains("Session memory from TrainingPreferenceContextProvider", StringComparison.OrdinalIgnoreCase));
        var latestUserMessage = messageList.LastOrDefault(m => m.Role == ChatRole.User);

        var responseText =
            memoryMessage is not null
                ? $"I will use this session context: {memoryMessage.Text.Replace("Session memory from TrainingPreferenceContextProvider:", string.Empty, StringComparison.OrdinalIgnoreCase).Trim()}"
                : $"I have received the latest request: {latestUserMessage?.Text ?? "no user message"}";

        return Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, responseText)));
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await GetResponseAsync(messages, options, cancellationToken);
        yield return new ChatResponseUpdate(ChatRole.Assistant, response.Text);
    }

    public object? GetService(Type serviceType, object? serviceKey = null) =>
        serviceType.IsInstanceOfType(this) ? this : null;

    public void Dispose()
    {
    }
}
