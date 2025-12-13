namespace SalesforceService.Application.Services.Interfaces;

public interface IModerationResultHandler
{
    Task HandleModerationResultAsync(string topic, ContentModeratedDto dto);
}