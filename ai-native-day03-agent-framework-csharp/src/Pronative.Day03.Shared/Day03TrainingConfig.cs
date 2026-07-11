using System.Text.Json;
using DotNetEnv;

namespace Pronative.Day03.Shared;

// Central configuration record loaded from JSON file + .env + environment variable overrides
public sealed record Day03TrainingConfig
{
    public string BatchId { get; init; } = "AN-2607-101";
    public string StudentId { get; init; } = "ST-2606-1000";
    public string ProjectEndpoint { get; init; } = "https://proj-an2607101-default-resource.services.ai.azure.com/api/projects/proj-an2607101-default";
    public string ModelDeployment { get; init; } = "gpt-5-mini";
    public string AgentName { get; init; } = "day03-agentic-reasoning-loop";

    // Load chain: JSON file (from args or appsettings.json) -> .env file -> environment variable overrides
    public static Day03TrainingConfig Load(string[] args)
    {
        LoadEnvFile();

        var filePath = args.FirstOrDefault(a => a.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            ?? "appsettings.json";

        var fromFile = File.Exists(filePath)
            ? JsonSerializer.Deserialize<Day03TrainingConfig>(
                File.ReadAllText(filePath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : new Day03TrainingConfig();

        return (fromFile ?? new Day03TrainingConfig()).WithEnvironmentOverrides();
    }

    // Walks up the directory tree to find and load the nearest .env file
    private static void LoadEnvFile()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir is not null)
        {
            var envFile = Path.Combine(dir, ".env");
            if (File.Exists(envFile))
            {
                Env.Load(envFile);
                return;
            }
            dir = Path.GetDirectoryName(dir);
        }
    }

    // Environment variables take precedence over file-based config values
    private Day03TrainingConfig WithEnvironmentOverrides() => this with
    {
        BatchId = Environment.GetEnvironmentVariable("BATCH_ID") ?? BatchId,
        StudentId = Environment.GetEnvironmentVariable("STUDENT_ID") ?? StudentId,
        ProjectEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT")
            ?? Environment.GetEnvironmentVariable("PROJECT_ENDPOINT")
            ?? ProjectEndpoint,
        ModelDeployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_DEPLOYMENT") ?? ModelDeployment,
        AgentName = Environment.GetEnvironmentVariable("FOUNDRY_AGENT_NAME") ?? AgentName
    };
}

public static class Day03Console
{
    public static void PrintHeader(Day03TrainingConfig config, string labName)
    {
        Console.WriteLine($"ProNative AI-Native Fullstack Engineering - Day 3 - {labName}");
        Console.WriteLine($"Batch: {config.BatchId} | Student: {config.StudentId}");
        Console.WriteLine($"Foundry project endpoint: {config.ProjectEndpoint}");
        Console.WriteLine($"Model deployment: {config.ModelDeployment}");
        Console.WriteLine();
    }

    public static void PrintAppStart()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("========================================================================");
        Console.WriteLine("                          Application Started                           ");
        Console.WriteLine("========================================================================");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintAppEnd()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("========================================================================");
        Console.WriteLine("                         Application Completed                          ");
        Console.WriteLine("========================================================================");
        Console.ResetColor();
    }

    public static void PrintLabStart(int labNumber)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"================== Lab {labNumber} Result Start =================");
        Console.ResetColor();
    }

    public static void PrintLabEnd(int labNumber)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"================== Lab {labNumber} Result End ====================");
        Console.ResetColor();
    }
}
