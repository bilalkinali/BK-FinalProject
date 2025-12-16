using ContentModerationService.Domain.Entity;

namespace ContentModerationService.Application;

public interface IModerationDecisionRepository
{
    Task AddModerationDecisionAsync(ModerationDecision moderationDecision);
}