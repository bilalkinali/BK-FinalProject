using SalesforceService.Application.Helpers;
using SalesforceService.Application.Services.Interfaces;
using SalesforceService.Domain.Entities;

namespace SalesforceService.Application.Commands;

public class EventCommand : IEventCommand
{
    private readonly IRecordIdentificationHelper _idHelper;
    private readonly IEventHandler _eventHandler;

    public EventCommand(
        IRecordIdentificationHelper idHelper,
        IEventHandler eventHandler)
    {
        _idHelper = idHelper;
        _eventHandler = eventHandler;
    }

    async Task IEventCommand.CreateInboundEventAsync(string topicName, string replayId, Dictionary<string, object?> fields)
    {
        var recordId = _idHelper.ExtractRecordId(fields)
            ?? throw new Exception("RecordId__c missing in Inbound Event");

        var objectType = _idHelper.ResolveObjectType(recordId); // Domain logic? Maybe value object

        var inboundEvent = InboundEvent.Create(topicName, replayId, recordId, objectType);

        // Save to DB (UoW)

        // Publish internal event (Dapr)
        await _eventHandler.HandleAsync(topicName, inboundEvent.EventId, fields);
    }

    async Task IEventCommand.CreateOutboundEventAsync(string topicName, string replayId, Dictionary<string, object?> fields)
    {
        throw new NotImplementedException();
    }
}