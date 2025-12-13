namespace SalesforceService.Application.Commands;

public interface IEventCommand
{
    Task CreateInboundEventAsync(string salesforceTopic, string replayId, Dictionary<string, object?> fields);
    Task CreateOutboundEventAsync(string topic, string correlationId, string result);
}