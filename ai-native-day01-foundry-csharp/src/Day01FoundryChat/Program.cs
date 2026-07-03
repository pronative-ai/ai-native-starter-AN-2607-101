using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

string configPath = "appsettings.json";
if (!File.Exists(configPath))
{
    var fallback = Path.Combine("src", "Day01FoundryChat", "appsettings.json");
    if (File.Exists(fallback))
    {
        configPath = fallback;
    }
    else
    {
        fallback = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (File.Exists(fallback))
        {
            configPath = fallback;
        }
    }
}
var config = TrainingConfig.Load(configPath);
var prompt = Args.Get(args, "--ask") ?? "Explain AI-native architecture for enterprise fullstack engineering.";

Telemetry.Trace(config, "Day01.Start", new
{
    Prompt = prompt,
    BatchId = config.Batch.Id,
    StudentId = config.Student.Id,
    config.Student.ResourceGroup
});

using var http = new HttpClient();
var foundry = new FoundryChatClient(http, config);
var search = new SearchGroundingClient(http, config);

// Walkthrough checkpoint:
// First ask without grounding so students can compare raw model behavior with RAG behavior.
var ungrounded = await foundry.AskAsync(prompt, grounding: []);
Console.WriteLine("\n=== Ungrounded model response ===\n");
Console.WriteLine(ungrounded);
Console.WriteLine("================================ Ungrounded call end =======================================");
Console.ReadLine();
// Walkthrough checkpoint:
// Then retrieve grounding snippets from the student's assigned Azure AI Search index.
// If Search is not configured yet, the client returns an empty collection and the app still runs.
var grounding = await search.SearchAsync(prompt);
Telemetry.Trace(config, "Day01.GroundingRetrieved", new
{
    DocumentCount = grounding.Count,
    config.AzureAiSearch.IndexName
});

var groundedPrompt = """
You are helping in the ProNative AI-Native Fullstack Engineering program.
Answer using the grounding context when it is relevant.
If the context is insufficient, say what is missing.
""";

var grounded = await foundry.AskAsync($"{groundedPrompt}\n\nQuestion: {prompt}", grounding);
Console.WriteLine("\n=== Grounded model response ===\n");
Console.WriteLine(grounded);
Console.WriteLine("================================ Grounded call end =======================================");


Telemetry.Trace(config, "Day01.Complete", new
{
    GroundingUsed = grounding.Count > 0
});

Console.WriteLine("================================== DAY 1 End =====================================");


public sealed class FoundryChatClient(HttpClient http, TrainingConfig config)
{
    public async Task<string> AskAsync(string userPrompt, IReadOnlyList<string> grounding)
    {
        if (string.IsNullOrWhiteSpace(config.AzureAiFoundry.Endpoint) ||
            string.IsNullOrWhiteSpace(config.AzureAiFoundry.ChatDeployment))
        {
            return "[Training placeholder] Configure AzureAiFoundry:Endpoint and ChatDeployment to call the live model.";
        }

        // Azure OpenAI-compatible chat completions URL.
        // Trainer note: if your Foundry project endpoint uses a different route, change this in one place.
        var endpoint = config.AzureAiFoundry.Endpoint.TrimEnd('/');
        var deployment = Uri.EscapeDataString(config.AzureAiFoundry.ChatDeployment);
        var url = $"{endpoint}/openai/deployments/{deployment}/chat/completions?api-version={config.AzureAiFoundry.ApiVersion}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        ApplyAuth(request, config.AzureAiFoundry.ApiKey, config.AzureAiFoundry.BearerToken);

        var messages = new List<object>
        {
            new
            {
                role = "system",
                content = "You are a concise enterprise AI engineering assistant. Explain decisions clearly."
            }
        };

        if (grounding.Count > 0)
        {
            messages.Add(new
            {
                role = "system",
                content = "Grounding context:\n" + string.Join("\n---\n", grounding)
            });
        }

        messages.Add(new { role = "user", content = userPrompt });

        var body = JsonSerializer.Serialize(new
        {
            messages,
            max_completion_tokens = 4000
        });

        request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var sw = Stopwatch.StartNew();
        using var response = await http.SendAsync(request);
        var payload = await response.Content.ReadAsStringAsync();
        sw.Stop();

        Telemetry.Trace(config, "Day01.ModelCall", new
        {
            response.StatusCode,
            DurationMs = sw.ElapsedMilliseconds,
            config.AzureAiFoundry.ChatDeployment
        });

        if (!response.IsSuccessStatusCode)
        {
            return $"Model call failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{payload}";
        }

        using var json = JsonDocument.Parse(payload);
        return json.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "";
    }

    private static void ApplyAuth(HttpRequestMessage request, string apiKey, string bearerToken)
    {
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            request.Headers.Add("api-key", apiKey);
            return;
        }

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
    }
}

public sealed class SearchGroundingClient(HttpClient http, TrainingConfig config)
{
    public async Task<IReadOnlyList<string>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(config.AzureAiSearch.Endpoint) ||
            string.IsNullOrWhiteSpace(config.AzureAiSearch.IndexName))
        {
            return [];
        }

        var endpoint = config.AzureAiSearch.Endpoint.TrimEnd('/');
        var index = Uri.EscapeDataString(config.AzureAiSearch.IndexName);
        var url = $"{endpoint}/indexes/{index}/docs/search?api-version={config.AzureAiSearch.ApiVersion}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        if (!string.IsNullOrWhiteSpace(config.AzureAiSearch.ApiKey))
        {
            request.Headers.Add("api-key", config.AzureAiSearch.ApiKey);
        }
        else if (!string.IsNullOrWhiteSpace(config.AzureAiSearch.BearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.AzureAiSearch.BearerToken);
        }

        var body = JsonSerializer.Serialize(new
        {
            search = query,
            top = 3
        });

        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await http.SendAsync(request);
        var payload = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Telemetry.Trace(config, "Day01.SearchFailed", new { response.StatusCode, payload });
            return [];
        }

        using var json = JsonDocument.Parse(payload);
        var results = new List<string>();

        foreach (var item in json.RootElement.GetProperty("value").EnumerateArray())
        {
            // This is deliberately schema-flexible for training.
            // Students can inspect their index and map the real content field.
            if (item.TryGetProperty("content", out var content))
            {
                results.Add(content.GetString() ?? "");
            }
            else
            {
                results.Add(item.ToString());
            }
        }

        return results;
    }
}

public sealed record TrainingConfig(
    BatchConfig Batch,
    StudentConfig Student,
    FoundryConfig AzureAiFoundry,
    SearchConfig AzureAiSearch,
    TelemetryConfig Telemetry)
{
    public static TrainingConfig Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Missing config file: {path}. Copy samples/appsettings.sample.json first.");
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<TrainingConfig>(json, JsonOptions) ??
               throw new InvalidOperationException("Unable to parse appsettings.json.");
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };
}

public sealed record BatchConfig(string Id);
public sealed record StudentConfig(string Id, string ResourceGroup, string ManagedIdentityName);
public sealed record FoundryConfig(string Endpoint, string ChatDeployment, string ApiVersion, string BearerToken, string ApiKey);
public sealed record SearchConfig(string Endpoint, string IndexName, string ApiVersion, string BearerToken, string ApiKey);
public sealed record TelemetryConfig(string ApplicationInsightsConnectionString, bool EnableConsoleTrace);

public static class Telemetry
{
    public static void Trace(TrainingConfig config, string eventName, object data)
    {
        if (!config.Telemetry.EnableConsoleTrace)
        {
            return;
        }

        // Day 1 uses console trace plus Foundry visibility.
        // In Azure Container Apps or later project days, this event shape maps cleanly to App Insights custom events.
        var envelope = new
        {
            TimestampUtc = DateTimeOffset.UtcNow,
            EventName = eventName,
            BatchId = config.Batch.Id,
            StudentId = config.Student.Id,
            Data = data
        };

        Console.Error.WriteLine(JsonSerializer.Serialize(envelope));
    }
}

public static class Args
{
    public static string? Get(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }
}
