namespace SalesforceService.Application.Services.Interfaces;

public interface ISalesforcePublisherService
{
    Task PublishAsync(string salesforceTopic, Dictionary<string, Object?> payload);
}