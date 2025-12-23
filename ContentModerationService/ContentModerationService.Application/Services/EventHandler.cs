using Action = ContentModerationService.Domain.Enums.Action;

namespace ContentModerationService.Application.Services;

public class EventHandler : IEventHandler
{
    private readonly IPublisherService _publisherService;

    public EventHandler(
        IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }
    async Task IEventHandler.ContentModeratedAsync(string correlationId, Action result)
    {
        var contentModeratedDto = new ContentModeratedDto(correlationId, result);

        await _publisherService.PublishAsync("content-moderated", contentModeratedDto);
    }

    async Task IEventHandler.ContentModerationFailedAsync(string correlationId, Action result)
    {
        var contentModeratedDto = new ContentModeratedDto(correlationId, result);
        await _publisherService.PublishAsync("content-moderation-failed", contentModeratedDto);
    }
}

internal record ContentModeratedDto(string CorrelationId, Action SuggestedAction);