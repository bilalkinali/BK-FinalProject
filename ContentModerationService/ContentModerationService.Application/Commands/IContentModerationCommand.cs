using ContentModerationService.Domain;
using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Application.Commands;

public interface IContentModerationCommand
{
    Task<Decision> ModerateContentAsync(MediaType mediaType, string content);
}