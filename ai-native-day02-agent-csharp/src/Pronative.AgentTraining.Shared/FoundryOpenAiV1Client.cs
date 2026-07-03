using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Pronative.AgentTraining.Shared;

public sealed class FoundryOpenAiV1Client
{
    private readonly TrainingConfig _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public FoundryOpenAiV1Client(TrainingConfig config, HttpClient? httpClient = null)
    {
        _config = config;
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<string> ChatAsync(string systemInstruction, string userPrompt, string? groundingContext = null)
    {
        if (!_config.UseLiveFoundry)
        {
            return $"[teaching adapter] {systemInstruction}\n\nUser request: {userPrompt}\n\nContext used: {groundingContext ?? "none"}";
        }

        var messages = new List<object>
        {
            new { role = "system", content = systemInstruction }
        };

        if (!string.IsNullOrWhiteSpace(groundingContext))
        {
            messages.Add(new { role = "system", content = $"Use this trusted context when relevant:\n{groundingContext}" });
        }

        messages.Add(new { role = "user", content = userPrompt });

        var payload = new
        {
            model = _config.ModelDeployment,
            messages,
            temperature = 0.2
        };

        using var response = await SendJsonAsync(HttpMethod.Post, "chat/completions", payload);
        return await ExtractTextAsync(response);
    }

    public async Task<string> InvokeFoundryAgentAsync(string agentName, string userPrompt)
    {
        if (!_config.UseLiveFoundry)
        {
            return $"[teaching adapter] Would invoke Foundry agent '{agentName}' with: {userPrompt}";
        }

        using var conversation = await SendJsonAsync(HttpMethod.Post, "conversations", new { items = Array.Empty<object>() });
        var conversationJson = await conversation.Content.ReadAsStringAsync();
        var conversationId = JsonDocument.Parse(conversationJson).RootElement.GetProperty("id").GetString()
            ?? throw new InvalidOperationException("The conversations API response did not include an id.");

        await SendJsonAsync(
            HttpMethod.Post,
            $"conversations/{conversationId}/items",
            new { items = new[] { new { type = "message", role = "user", content = userPrompt } } });

        using var response = await SendJsonAsync(
            HttpMethod.Post,
            "responses",
            new
            {
                conversation = conversationId,
                input = "",
                agent_reference = new { name = agentName, type = "agent_reference" }
            });

        return await ExtractTextAsync(response);
    }

    public async Task<string> InvokeFoundryWorkflowAsync(string workflowName, string userPrompt)
    {
        if (!_config.UseLiveFoundry)
        {
            return $"[teaching adapter] Would start Foundry workflow '{workflowName}' with: {userPrompt}";
        }

        using var conversation = await SendJsonAsync(HttpMethod.Post, "conversations", new { items = Array.Empty<object>() });
        var conversationJson = await conversation.Content.ReadAsStringAsync();
        var conversationId = JsonDocument.Parse(conversationJson).RootElement.GetProperty("id").GetString()
            ?? throw new InvalidOperationException("The conversations API response did not include an id.");

        using var response = await SendJsonAsync(
            HttpMethod.Post,
            "responses",
            new
            {
                conversation = conversationId,
                input = userPrompt,
                agent_reference = new { name = workflowName, type = "agent_reference" }
            });

        return await ExtractTextAsync(response);
    }

    private async Task<HttpResponseMessage> SendJsonAsync(HttpMethod method, string relativePath, object payload)
    {
        var endpoint = _config.OpenAiV1Endpoint.TrimEnd('/');
        using var request = new HttpRequestMessage(method, $"{endpoint}/{relativePath.TrimStart('/')}");
        request.Content = new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8, "application/json");

        if (!string.IsNullOrWhiteSpace(_config.ApiKey))
        {
            request.Headers.Add("api-key", _config.ApiKey);
        }
        else if (!string.IsNullOrWhiteSpace(_config.BearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config.BearerToken);
        }
        else
        {
            throw new InvalidOperationException("Set AZURE_OPENAI_API_KEY or AZURE_OPENAI_BEARER_TOKEN before enabling live Foundry calls.");
        }

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Foundry call failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
        }

        return response;
    }

    private async Task<string> ExtractTextAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        if (root.TryGetProperty("output_text", out var outputText))
        {
            return outputText.GetString() ?? "";
        }

        if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
        {
            var message = choices[0].GetProperty("message");
            return message.TryGetProperty("content", out var content) ? content.GetString() ?? "" : body;
        }

        if (root.TryGetProperty("output", out var output))
        {
            var parts = new List<string>();
            foreach (var item in output.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out var contentItems))
                {
                    continue;
                }

                foreach (var content in contentItems.EnumerateArray())
                {
                    if (content.TryGetProperty("text", out var text))
                    {
                        parts.Add(text.GetString() ?? "");
                    }
                }
            }

            if (parts.Count > 0)
            {
                return string.Join(Environment.NewLine, parts);
            }
        }

        return body;
    }
}
