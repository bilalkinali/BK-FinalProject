namespace SalesforceService.Application.Queries.QueryDto;

public record InboundEventDto
(
    int InboundEventId,
    string CorrelationId,
    string SalesforceTopic,
    string ReplayId,
    string RecordId,
    string ObjectType,
    string CreatedAt
);