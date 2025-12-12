using SalesforceService.Application;
using SalesforceService.Domain.Entities;

namespace SalesforceService.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly SalesforceContext _db;

    public EventRepository(SalesforceContext db)
    {
        _db = db;
    }

    async Task IEventRepository.AddInboundEventAsync(InboundEvent inboundEvent)
    {
        await _db.InboundEvents.AddAsync(inboundEvent);
    }
}