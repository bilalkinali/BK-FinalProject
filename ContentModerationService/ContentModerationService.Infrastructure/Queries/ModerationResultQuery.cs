using ContentModerationService.Application.Queries;
using ContentModerationService.Application.Queries.QueryDto;
using ContentModerationService.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ContentModerationService.Infrastructure.Queries;

public class ModerationResultQuery : IModerationResultQuery
{
    private readonly ContentModerationContext _db;
    private readonly IModerationResultMapper _mapper;

    public ModerationResultQuery(ContentModerationContext db, IModerationResultMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    async Task<ModerationResultDto?> IModerationResultQuery.GetModerationResultAsync(int moderationId)
    {
        var moderationResult = await _db.ModerationResults
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == moderationId);

        return moderationResult is null ? null : _mapper.MapToDto(moderationResult);
    }

    async Task<IReadOnlyList<ModerationResultDto>> IModerationResultQuery.GetModerationResultsAsync()
    {
        var moderationResults = await _db.ModerationResults
            .AsNoTracking()
            .ToListAsync();

        return moderationResults.Select(_mapper.MapToDto).ToList();
    }
}