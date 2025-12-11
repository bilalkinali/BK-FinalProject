using Dapr.Client;
using SalesforceService.Application.Services.Interfaces;

namespace SalesforceService.Infrastructure.Services;

public class DaprPublisherService(DaprClient daprClient) : IPublisherService
{
    private const string PubSub = "pubsub";
    async Task IPublisherService.PublishAsync<T>(string topic, T data)
    {
        await daprClient.PublishEventAsync(PubSub, topic, data);
    }
}