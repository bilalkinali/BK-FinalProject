using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Application.Commands;

public interface IContentModerationCommand
{
    Task ModerateContentAsync(MediaType mediaType, string correlationId, string content);
}