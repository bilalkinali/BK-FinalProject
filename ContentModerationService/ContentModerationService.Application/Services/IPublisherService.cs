namespace ContentModerationService.Application.Services;

public interface IPublisherService
{
    Task PublishAsync<T>(string topic, T data);
}