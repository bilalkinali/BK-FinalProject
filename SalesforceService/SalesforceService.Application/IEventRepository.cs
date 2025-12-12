using SalesforceService.Domain.Entities;

namespace SalesforceService.Application;

public interface IEventRepository
{
    Task AddInboundEventAsync(InboundEvent inboundEvent);
}