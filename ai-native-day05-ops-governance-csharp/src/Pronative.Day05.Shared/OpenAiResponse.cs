using System.Text.Json;

namespace Pronative.Day05.Shared;

public sealed record OpenAiUsage(int? PromptTokens, int? CompletionTokens, int? TotalTokens);

public static class OpenAiResponse
{
    public static OpenAiUsage ExtractUsage(string json)
    {
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("usage", out var usage))
        {
            return new OpenAiUsage(null, null, null);
        }

        return new OpenAiUsage(
            TryGetInt(usage, "prompt_tokens"),
            TryGetInt(usage, "completion_tokens"),
            TryGetInt(usage, "total_tokens"));
    }

    public static string ExtractChatAnswerOrRawJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("choices", out var choices) &&
            choices.ValueKind == JsonValueKind.Array &&
            choices.GetArrayLength() > 0 &&
            choices[0].TryGetProperty("message", out var message) &&
            message.TryGetProperty("content", out var content))
        {
            return content.GetString() ?? "";
        }

        return JsonSerializer.Serialize(doc.RootElement, OperationalEvent.JsonOptions);
    }

    public static string TryPrettyJson(string value)
    {
        try
        {
            using var doc = JsonDocument.Parse(value);
            return JsonSerializer.Serialize(doc.RootElement, OperationalEvent.JsonOptions);
        }
        catch
        {
            return value;
        }
    }

    private static int? TryGetInt(JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value) && value.TryGetInt32(out var result) ? result : null;
    }
}
