using ContentModerationService.Domain.Entity;

namespace ContentModerationService.Application;

public interface IModerationResultRepository
{
    Task AddModerationResultAsync(ModerationResult moderationResult);
}