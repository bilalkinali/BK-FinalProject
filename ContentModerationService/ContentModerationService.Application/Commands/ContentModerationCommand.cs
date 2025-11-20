using ContentModerationService.Application.Configuration;
using ContentModerationService.Application.Services;
using ContentModerationService.Application.Services.ProxyInterface;
using ContentModerationService.Domain;
using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Application.Commands;

public class ContentModerationCommand : IContentModerationCommand
{
    private readonly IContentDetection _contentDetection;
    private readonly IDecisionService _decisionService;
    private readonly IRejectionThresholdProvider _rejectionDetectionProvider;

    public ContentModerationCommand(
        IContentDetection contentDetection,
        IDecisionService decisionService,
        IRejectionThresholdProvider rejectionDetectionProvider)
    {
        _contentDetection = contentDetection;
        _decisionService = decisionService;
        _rejectionDetectionProvider = rejectionDetectionProvider;
    }
    async Task<Decision> IContentModerationCommand.ModerateContentAsync(MediaType mediaType, string content)
    {
        // Detect / analyze content
        var detectionResult = await _contentDetection.ContentDetectionAsync(mediaType, content);

        // Load the rejection thresholds settings
        var rejectionThresholds = _rejectionDetectionProvider.GetRejectionThresholds();

        // Make decision based on detection results and thresholds
        var decisionResult = _decisionService.MakeDecision(detectionResult, rejectionThresholds);

        // Publish decision result?

        return decisionResult;
    }
}