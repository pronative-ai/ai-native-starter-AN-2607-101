using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;

namespace Pronative.Day05.Shared;

public static class AzureRestClient
{
    private static readonly DefaultAzureCredential Credential = new();

    public static async Task<HttpRequestMessage> CreateJsonRequestAsync(
        HttpMethod method,
        string url,
        string tokenScope,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        var token = await Credential.GetTokenAsync(
            new TokenRequestContext(new[] { tokenScope }),
            cancellationToken);

        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        if (body is not null)
        {
            request.Content = new StringContent(
                JsonSerializer.Serialize(body, OperationalEvent.JsonOptions),
                Encoding.UTF8,
                "application/json");
        }

        return request;
    }
}
