using SalesforceService.Domain.Entities;

namespace SalesforceService.Application.Services.Interfaces;

public interface IEventHandler
{
    Task PublishInboundEventAsync(string salesforceTopic, string correlationId, Dictionary<string, object?> fields);
    Task PublishOutboundEventAsync(string internalTopic, string recordId, string result);
}