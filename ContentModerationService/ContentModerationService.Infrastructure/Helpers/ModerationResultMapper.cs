using ContentModerationService.Application.Queries.QueryDto;
using ContentModerationService.Domain.Entity;

namespace ContentModerationService.Infrastructure.Helpers;

public class ModerationResultMapper : IModerationResultMapper
{
    ModerationResultDto IModerationResultMapper.MapToDto(ModerationResult moderationResult)
    {
        return new ModerationResultDto
        (
            moderationResult.Id,
            moderationResult.CorrelationId,
            moderationResult.Content,
            moderationResult.SuggestedAction.ToString(),
            moderationResult.Hate ?? 0,
            moderationResult.SelfHarm ?? 0,
            moderationResult.Sexual ?? 0,
            moderationResult.Violence ?? 0,
            moderationResult.CreatedAt.ToString("o")
        );
    }
}