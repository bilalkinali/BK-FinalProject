using ContentModerationService.Application;
using ContentModerationService.Domain.Entity;

namespace ContentModerationService.Infrastructure.Repositories;

public class ModerationResultRepository : IModerationResultRepository
{
    private readonly ContentModerationContext _db;

    public ModerationResultRepository(ContentModerationContext db)
    {
        _db = db;
    }

    async Task IModerationResultRepository.AddModerationResultAsync(ModerationResult moderationResult)
    {
        await _db.ModerationResults.AddAsync(moderationResult);
        await _db.SaveChangesAsync();
    }
}