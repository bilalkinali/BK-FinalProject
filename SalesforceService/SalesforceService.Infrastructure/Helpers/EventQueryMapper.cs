using SalesforceService.Application.Queries.QueryDto;
using SalesforceService.Domain.Entities;

namespace SalesforceService.Infrastructure.Helpers;

public class EventQueryMapper : IEventQueryMapper
{
    InboundEventDto IEventQueryMapper.MapToInboundEventDto(InboundEvent inboundEvent)
    {
        return new InboundEventDto
        (
            inboundEvent.Id,
            inboundEvent.CorrelationId,
            inboundEvent.SalesforceTopic,
            inboundEvent.ReplayId,
            inboundEvent.RecordId,
            inboundEvent.ObjectType,
            inboundEvent.CreatedAt.ToString("o")
        );
    }

    OutboundEventDto IEventQueryMapper.MapToOutboundEventDto(OutboundEvent outboundEvent)
    {
        return new OutboundEventDto
        (
            outboundEvent.Id,
            outboundEvent.CorrelationId,
            outboundEvent.SalesforceTopic,
            outboundEvent.RecordId,
            outboundEvent.Result,
            outboundEvent.CreatedAt.ToString("o")
        );
    }
}