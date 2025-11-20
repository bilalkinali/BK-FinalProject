using ContentModerationService.Domain;
using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Infrastructure;

public interface IAzureContentSafetyProxy
{
    Task<DetectionResult> DetectAsync(HttpRequestMessage msg, MediaType mediaType);
}