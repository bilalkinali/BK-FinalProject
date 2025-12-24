using SalesforceService.Application.Queries.QueryDto;

namespace SalesforceService.Application.Queries;

public interface IEventQuery
{
    Task<InboundEventDto?> GetInboundEventAsync(int inboundEventId);
    Task<IReadOnlyList<InboundEventDto>> GetInboundEventsAsync();

    Task<OutboundEventDto?> GetOutboundEventAsync(int outboundEventId);
    Task<IReadOnlyList<OutboundEventDto>> GetOutboundEventsAsync();
}