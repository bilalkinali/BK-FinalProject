namespace SalesforceService.Application.Commands;

public interface IEventCommand
{
    Task CreateInboundEventAsync(string topicName, string replayId, Dictionary<string, object?> fields);
    Task CreateOutboundEventAsync(string topicName, string replayId, Dictionary<string, object?> fields);
}