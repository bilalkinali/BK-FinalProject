using ContentModerationService.Application.Queries.QueryDto;
using ContentModerationService.Domain.Entity;

namespace ContentModerationService.Infrastructure.Helpers;

public interface IModerationResultMapper
{
    ModerationResultDto MapToDto(ModerationResult moderationResult);
}