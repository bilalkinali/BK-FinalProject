using ContentModerationService.Application.Services.ProxyInterface;
using ContentModerationService.Domain;
using ContentModerationService.Domain.Enums;
using ContentModerationService.Infrastructure.Helpers;

namespace ContentModerationService.Infrastructure.Services;

public class ContentDetection : IContentDetection
{
    private readonly IAzureContentSafetyProxy _azureContentSafetyProxy;
    private readonly IRequestBuilder _requestBuilder;

    public ContentDetection(IAzureContentSafetyProxy azureContentSafetyProxy, IRequestBuilder requestBuilder)
    {
        _azureContentSafetyProxy = azureContentSafetyProxy;
        _requestBuilder = requestBuilder;
    }

    async Task<DetectionResult> IContentDetection.ContentDetectionAsync(MediaType mediaType, string content)
    {
        var message = _requestBuilder.BuildHttpRequestMessage(mediaType, content);
        var result = await _azureContentSafetyProxy.DetectAsync(message, mediaType);

        return result;
    }
}