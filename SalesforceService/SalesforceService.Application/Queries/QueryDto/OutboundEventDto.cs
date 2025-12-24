namespace SalesforceService.Application.Queries.QueryDto;

public record OutboundEventDto
(
    int OutboundEventId,
    string CorrelationId,
    string SalesforceTopic,
    string RecordId,
    string Result,
    string CreatedAt
);