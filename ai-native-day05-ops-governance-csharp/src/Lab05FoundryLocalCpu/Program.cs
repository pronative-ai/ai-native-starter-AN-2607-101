using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using DotNetEnv;
using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;

Console.WriteLine("================================================================================");
Console.WriteLine("                     Application 05 Start");
Console.WriteLine("================================================================================");

var config = LocalConfig.Load();
var modelAlias = Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_MODEL_ALIAS") ?? "qwen2.5-0.5b";
using var activitySource = new ActivitySource("Pronative.Day05.Lab05");

Header("Day 5 Lab 05 - Foundry Local CPU Walkthrough");
Row("Model alias", modelAlias);
Row("Batch", config.BatchId);
Row("Student", config.StudentId);

using var activity = activitySource.StartActivity("ai.native.foundry_local.lifecycle", ActivityKind.Internal);
activity?.SetTag("BatchId", config.BatchId);
activity?.SetTag("StudentId", config.StudentId);
activity?.SetTag("foundry.local.model_alias", modelAlias);
activity?.SetTag("foundry.local.runtime", "cpu-or-local-device");

var started = Stopwatch.GetTimestamp();

try
{
    var localConfig = new Configuration
    {
        AppName = "pronative_day05_foundry_local",
        LogLevel = Microsoft.AI.Foundry.Local.LogLevel.Information
    };

    await FoundryLocalManager.CreateAsync(localConfig, NullLogger.Instance);
    var manager = FoundryLocalManager.Instance;

    Header("Execution Providers");
    var executionProviders = manager.DiscoverEps();
    if (executionProviders.Length == 0)
    {
        Console.WriteLine("No execution providers to download or register.");
    }
    else
    {
        foreach (var ep in executionProviders)
        {
            Row(ep.Name, ep.IsRegistered.ToString());
        }

        Console.WriteLine();
        Console.WriteLine("Downloading/registering execution providers when required.");
        await manager.DownloadAndRegisterEpsAsync((epName, percent) =>
        {
            Console.Write($"\r{epName,-32} {percent,6:F1}%");
        });
        Console.WriteLine();
    }

    var catalog = await manager.GetCatalogAsync();
    var model = await catalog.GetModelAsync(modelAlias) ?? throw new InvalidOperationException($"Model alias '{modelAlias}' was not found.");

    Header("Model Lifecycle");
    Row("Model ID", model.Id);

    Console.WriteLine("Downloading model if it is not already cached.");
    await model.DownloadAsync(progress =>
    {
        Console.Write($"\rDownloading model: {progress:F2}%");
        if (progress >= 100f)
        {
            Console.WriteLine();
        }
    });

    Console.Write($"Loading model {model.Id}...");
    await model.LoadAsync();
    Console.WriteLine("done.");

    var chatClient = await model.GetChatClientAsync();
    List<ChatMessage> messages = new()
    {
        new ChatMessage
        {
            Role = "user",
            Content = "In two short bullets, explain why local inference still needs operations controls."
        }
    };

    Header("Streaming Local Response");
    var responseStarted = Stopwatch.GetTimestamp();
    var streamingResponse = chatClient.CompleteChatStreamingAsync(messages, CancellationToken.None);
    await foreach (var chunk in streamingResponse)
    {
        Console.Write(chunk.Choices[0].Message.Content);
        Console.Out.Flush();
    }

    var responseLatency = Stopwatch.GetElapsedTime(responseStarted);
    Console.WriteLine();
    Row("Local response latency", $"{responseLatency.TotalMilliseconds:F0} ms");

    Console.WriteLine("Unloading model.");
    await model.UnloadAsync();

    var totalLatency = Stopwatch.GetElapsedTime(started);
    Header("Operational Event");
    WriteEvent(config, "lab05.foundry_local_completed", modelAlias, totalLatency, true);

Console.WriteLine("================================================================================");
Console.WriteLine("                     Application 05 End");
Console.WriteLine("================================================================================");
}
catch (Exception ex)
{
    var totalLatency = Stopwatch.GetElapsedTime(started);
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

    Header("Operational Event");
    WriteEvent(config, "lab05.foundry_local_failed", modelAlias, totalLatency, false, ex.GetType().Name, ex.Message);

    Console.WriteLine();
    Console.WriteLine("Trainer fallback:");
    Console.WriteLine("- First run can be slow because execution providers and model artifacts are downloaded.");
    Console.WriteLine("- Use this failure to discuss lifecycle, local cache, device capability, and cleanup.");
    Console.WriteLine("- DGX Spark is not part of the alpha-batch hands-on setup.");

Console.WriteLine("================================================================================");
Console.WriteLine("                     Application 05 End");
Console.WriteLine("================================================================================");
}

static void Header(string title)
{
    Console.WriteLine();
    Console.WriteLine(title);
    Console.WriteLine(new string('=', title.Length));
}

static void Row(string name, string value)
{
    Console.WriteLine($"{name,-28} {value}");
}

static void WriteEvent(
    LocalConfig config,
    string eventName,
    string modelAlias,
    TimeSpan latency,
    bool success,
    string? errorType = null,
    string? errorMessage = null)
{
    var json = JsonSerializer.Serialize(new
    {
        EventName = eventName,
        config.BatchId,
        config.StudentId,
        config.EnvironmentId,
        AgentName = "foundry-local-cpu-walkthrough",
        ModelDeployment = modelAlias,
        Backend = "local-cpu",
        LatencyMs = (long)latency.TotalMilliseconds,
        Success = success,
        ErrorType = errorType,
        ErrorMessage = errorMessage
    }, new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    });

    Console.WriteLine(json);
}

internal sealed record LocalConfig(string BatchId, string StudentId, string EnvironmentId)
{
    public static LocalConfig Load()
    {
        var envFile = FindEnvFile();
        if (envFile is not null)
        {
            Env.Load(envFile);
        }

        return new LocalConfig(
            Environment.GetEnvironmentVariable("BATCH_ID") ?? "AN-2607-101",
            Environment.GetEnvironmentVariable("STUDENT_ID") ?? "ST-2606-1000",
            Environment.GetEnvironmentVariable("ENVIRONMENT_ID") ?? "an2607101");
    }

    private static string? FindEnvFile()
    {
        var candidates = new[]
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory(),
            Environment.CurrentDirectory
        };

        foreach (var start in candidates.Distinct())
        {
            var dir = new DirectoryInfo(start);
            while (dir is not null)
            {
                var path = Path.Combine(dir.FullName, ".env");
                if (File.Exists(path))
                    return path;
                dir = dir.Parent;
            }
        }

        return null;
    }
}
