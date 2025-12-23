using Action = ContentModerationService.Domain.Enums.Action;

namespace ContentModerationService.Application.Services;

public interface IEventHandler
{
    Task ContentModeratedAsync(string correlationId, Action result);
    Task ContentModerationFailedAsync(string correlationId, Action result);
}