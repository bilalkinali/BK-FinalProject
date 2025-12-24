using ContentModerationService.Application.Configuration;
using ContentModerationService.Application.Services;
using ContentModerationService.Application.Services.ProxyInterface;
using ContentModerationService.Domain;
using ContentModerationService.Domain.Entity;
using ContentModerationService.Domain.Enums;
using Action = ContentModerationService.Domain.Enums.Action;

namespace ContentModerationService.Application.Commands;

public class ContentModerationCommand : IContentModerationCommand
{
    private readonly IContentDetection _contentDetection;
    private readonly IDecisionService _decisionService;
    private readonly IRejectionThresholdProvider _rejectionDetectionProvider;
    private readonly IEventHandler _eventHandler;
    private readonly IModerationResultRepository _moderationResultRepository;

    public ContentModerationCommand(
        IContentDetection contentDetection,
        IDecisionService decisionService,
        IRejectionThresholdProvider rejectionDetectionProvider,
        IEventHandler eventHandler,
        IModerationResultRepository moderationResultRepository)
    {
        _contentDetection = contentDetection;
        _decisionService = decisionService;
        _rejectionDetectionProvider = rejectionDetectionProvider;
        _eventHandler = eventHandler;
        _moderationResultRepository = moderationResultRepository;
    }
    async Task IContentModerationCommand.ModerateContentAsync(MediaType mediaType, string correlationId, string content)
    {
        try
        {
            // Detect / analyze content
            var detectionResult = await _contentDetection.ContentDetectionAsync(mediaType, content);

            // Load the rejection thresholds settings
            var rejectionThresholds = _rejectionDetectionProvider.GetRejectionThresholds();

            // Make decision based on detection results and thresholds
            var decisionResult = _decisionService.MakeDecision(detectionResult, rejectionThresholds);

            // Save decision result
            var severities = ConvertToDictionary(detectionResult.CategoriesAnalysis!);

            var moderationResult = ModerationResult.Create(
                correlationId,
                content,
                decisionResult.SuggestedAction,
                severities);

            await _moderationResultRepository.AddModerationResultAsync(moderationResult);

            // Publish decision result
            await _eventHandler.ContentModeratedAsync(correlationId, decisionResult.SuggestedAction);
        }
        catch (HttpRequestException)
        {
            // Publish moderation failed - external service is unavailable
            await _eventHandler.ContentModeratedAsync(correlationId, Action.Failed);
            throw;
        }
    }

    private Dictionary<Category, int?> ConvertToDictionary(List<CategoriesAnalysis> categories)
    {
        var severities = new Dictionary<Category, int?>();
        foreach (var pair in categories)
        {
            severities.Add(Enum.Parse<Category>(pair.Category!), pair.Severity);
        }

        return severities;
    }
}