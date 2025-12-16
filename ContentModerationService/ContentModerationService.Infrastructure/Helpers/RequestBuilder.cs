using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContentModerationService.Domain;
using ContentModerationService.Domain.Enums;
using ContentModerationService.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace ContentModerationService.Infrastructure.Helpers;

public class RequestBuilder : IRequestBuilder
{
    private readonly string _apiVersion;
    private readonly string _endpoint;
    private readonly string _subscriptionKey;

    private readonly JsonSerializerOptions _options =
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

    public RequestBuilder(IOptions<ContentModerationSettings> settings)
    {
        _apiVersion = settings.Value.ApiVersion;
        _endpoint = settings.Value.Endpoint;
        _subscriptionKey = settings.Value.SubscriptionKey;
    }

    HttpRequestMessage IRequestBuilder.BuildHttpRequestMessage(MediaType mediaType, string content)
    {
        var url = BuildUrl(mediaType);
        var requestBody = BuildRequestBody(mediaType, content);
        var payload = JsonSerializer.Serialize(requestBody,requestBody.GetType(), _options);
        var msg = new HttpRequestMessage(HttpMethod.Post, url);

        msg.Content = new StringContent(payload, Encoding.UTF8, "application/json");
        msg.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

        return msg;
    }


    private string BuildUrl(MediaType mediaType)
    {
        switch (mediaType)
        {
            case MediaType.Text:
                return $"{_endpoint}/contentsafety/text:analyze?api-version={_apiVersion}";
            case MediaType.Image:
                return $"{_endpoint}/contentsafety/image:analyze?api-version={_apiVersion}";
            default:
                throw new ArgumentException($"Invalid Media Type {mediaType}");
        }
    }

    private object BuildRequestBody(MediaType mediaType, string content)
    {
        switch (mediaType)
        {
            case MediaType.Text:
                return new TextDetectionRequest(content);
            default:
                throw new ArgumentException($"Invalid Media Type {mediaType}");
        }
    }
}