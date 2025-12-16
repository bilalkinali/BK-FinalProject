using ContentModerationService.Application;
using ContentModerationService.Domain.Entity;

namespace ContentModerationService.Infrastructure.Repositories;

public class ModerationDecisionRepository : IModerationDecisionRepository
{
    private readonly ContentModerationContext _db;

    public ModerationDecisionRepository(ContentModerationContext db)
    {
        _db = db;
    }

    async Task IModerationDecisionRepository.AddModerationDecisionAsync(ModerationDecision moderationDecision)
    {
        await _db.ModerationDecisions.AddAsync(moderationDecision);
        await _db.SaveChangesAsync();
    }
}