using ContentModerationService.Application.Queries.QueryDto;

namespace ContentModerationService.Application.Queries;

public interface IModerationResultQuery
{
    Task<ModerationResultDto?> GetModerationResultAsync(int moderationId);
    Task<IReadOnlyList<ModerationResultDto>> GetModerationResultsAsync();
}