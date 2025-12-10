namespace SalesforceService.Application.Services.Interfaces;

public interface IPublisherService
{
    Task PublishAsync<T>(string topic, T data);
}