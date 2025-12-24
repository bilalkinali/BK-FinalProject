using SalesforceService.Application.Queries.QueryDto;
using SalesforceService.Domain.Entities;

namespace SalesforceService.Infrastructure.Helpers;

public interface IEventQueryMapper
{
    InboundEventDto MapToInboundEventDto(InboundEvent inboundEvent);
    OutboundEventDto MapToOutboundEventDto(OutboundEvent outboundEvent);
}