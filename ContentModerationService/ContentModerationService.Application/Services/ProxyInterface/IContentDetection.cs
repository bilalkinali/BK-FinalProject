using ContentModerationService.Domain;
using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Application.Services.ProxyInterface
{
    public interface IContentDetection
    {
        Task<DetectionResult> ContentDetectionAsync(MediaType mediaType, string content);
    }
}