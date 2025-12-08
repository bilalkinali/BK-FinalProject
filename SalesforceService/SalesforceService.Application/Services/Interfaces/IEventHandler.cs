namespace SalesforceService.Application.Services.Interfaces;

public interface IEventHandler
{
    Task HandleAsync(string topicName, string eventId, Dictionary<string, object?> fields);
}