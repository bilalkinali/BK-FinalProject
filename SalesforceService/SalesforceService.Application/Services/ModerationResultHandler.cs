using SalesforceService.Application.Commands;
using SalesforceService.Application.Services.Interfaces;
using SalesforceService.Domain.Enums;

namespace SalesforceService.Application.Services;

public class ModerationResultHandler : IModerationResultHandler
{
    private readonly IEventCommand _command;

    public ModerationResultHandler(IEventCommand command)
    {
        _command = command;
    }

    async Task IModerationResultHandler.HandleModerationResultAsync(string topic, ContentModeratedDto dto)
    {
        await _command.CreateOutboundEventAsync(topic, dto.CorrelationId, dto.Result.ToString());
    }
}
public record ContentModeratedDto(string CorrelationId, ModerationResult Result);