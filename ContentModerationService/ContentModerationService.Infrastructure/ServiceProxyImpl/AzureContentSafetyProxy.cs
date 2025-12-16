using ContentModerationService.Domain;
using ContentModerationService.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContentModerationService.Infrastructure.ServiceProxyImpl;

public class AzureContentSafetyProxy : IAzureContentSafetyProxy
{
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _options =
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

    public AzureContentSafetyProxy(HttpClient client)
    {
        _client = client;
    }
    async Task<DetectionResult> IAzureContentSafetyProxy.DetectAsync(HttpRequestMessage msg, MediaType mediaType)
    {
        var response = await _client.SendAsync(msg);
        var responseText = await response.Content.ReadAsStringAsync();

        Console.WriteLine(response.StatusCode.ToString());
        foreach (var header in response.Headers)
        {
            Console.WriteLine($"{header.Key}: {string.Join(",", header.Value)}");
        }
        Console.WriteLine(responseText);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Error Code: {response.StatusCode.ToString()}\n" +
                $"Response: {responseText}");
        }

        var result = DeserializeDetectionResult(responseText, mediaType);
        if (result == null)
        {
            throw new InvalidOperationException($"HttpResponse is null. Reponse text is {responseText}");
        }

        return result;
    }

    private DetectionResult? DeserializeDetectionResult(string json, MediaType mediaType)
    {
        switch (mediaType)
        {
            case MediaType.Text:
                return JsonSerializer.Deserialize<TextDetectionResult>(json, _options);
            case MediaType.Image:
            default:
                throw new ArgumentException($"Invalid Media Type {mediaType}");
        }
    }
}