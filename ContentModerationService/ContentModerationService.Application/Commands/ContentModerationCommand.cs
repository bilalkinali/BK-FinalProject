using ContentModerationService.Application.Configuration;
using ContentModerationService.Application.Services;
using ContentModerationService.Application.Services.ProxyInterface;
using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Application.Commands;

public class ContentModerationCommand : IContentModerationCommand
{
    private readonly IContentDetection _contentDetection;
    private readonly IDecisionService _decisionService;
    private readonly IRejectionThresholdProvider _rejectionDetectionProvider;
    private readonly IEventHandler _eventHandler;

    public ContentModerationCommand(
        IContentDetection contentDetection,
        IDecisionService decisionService,
        IRejectionThresholdProvider rejectionDetectionProvider,
        IEventHandler eventHandler)
    {
        _contentDetection = contentDetection;
        _decisionService = decisionService;
        _rejectionDetectionProvider = rejectionDetectionProvider;
        _eventHandler = eventHandler;
    }
    async Task IContentModerationCommand.ModerateContentAsync(MediaType mediaType, string correlationId, string content)
    {
        // Detect / analyze content
        var detectionResult = await _contentDetection.ContentDetectionAsync(mediaType, content);

        // Load the rejection thresholds settings
        var rejectionThresholds = _rejectionDetectionProvider.GetRejectionThresholds();

        // Make decision based on detection results and thresholds
        var decisionResult = _decisionService.MakeDecision(detectionResult, rejectionThresholds);

        // Save decision result

        // Publish decision result
        await _eventHandler.ContentModeratedAsync(correlationId, decisionResult.SuggestedAction);
    }
}