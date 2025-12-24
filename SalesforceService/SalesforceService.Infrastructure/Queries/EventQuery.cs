using Microsoft.EntityFrameworkCore;
using SalesforceService.Application.Queries;
using SalesforceService.Application.Queries.QueryDto;
using SalesforceService.Infrastructure.Helpers;

namespace SalesforceService.Infrastructure.Queries;

public class EventQuery : IEventQuery
{
    private readonly SalesforceContext _db;
    private readonly IEventQueryMapper _mapper;

    public EventQuery(SalesforceContext db, IEventQueryMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    async Task<InboundEventDto?> IEventQuery.GetInboundEventAsync(int inboundEventId)
    {
        var inboundEvent = await _db.InboundEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(ie => ie.Id == inboundEventId);

        return inboundEvent is null ? null : _mapper.MapToInboundEventDto(inboundEvent);
    }

    async Task<IReadOnlyList<InboundEventDto>> IEventQuery.GetInboundEventsAsync()
    {
        var indboundEvents = await _db.InboundEvents
            .AsNoTracking()
            .ToListAsync();

        return indboundEvents.Select(_mapper.MapToInboundEventDto).ToList();
    }

    async Task<OutboundEventDto?> IEventQuery.GetOutboundEventAsync(int outboundEventId)
    {
        var outboundEvent = await _db.OutboundEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(oe => oe.Id == outboundEventId);

        return outboundEvent is null ? null : _mapper.MapToOutboundEventDto(outboundEvent);
    }

    async Task<IReadOnlyList<OutboundEventDto>> IEventQuery.GetOutboundEventsAsync()
    {
        var outboundEvents = await _db.OutboundEvents
            .AsNoTracking()
            .ToListAsync();

        return outboundEvents.Select(_mapper.MapToOutboundEventDto).ToList();
    }
}