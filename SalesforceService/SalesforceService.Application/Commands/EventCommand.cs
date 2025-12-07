using SalesforceService.Application.Helpers;
using SalesforceService.Domain.Entities;

namespace SalesforceService.Application.Commands;

public class EventCommand : IEventCommand
{
    private readonly IRecordIdentificationHelper _idHelper;

    public EventCommand(IRecordIdentificationHelper idHelper)
    {
        _idHelper = idHelper;
    }
    async Task IEventCommand.CreateInboundEventAsync(string topicName, string replayId, Dictionary<string, object?> fields)
    {
        var recordId = _idHelper.ExtractRecordId(fields)
            ?? throw new Exception("RecordId__c missing in Inbound Event");

        var objectType = _idHelper.ResolveObjectType(recordId);

        var inboundEvent = InboundEvent.Create(
            topicName,
            replayId,
            recordId,
            objectType
        );

        // Save to DB (UoW)

        // Publish internal event (Dapr)
    }

    async Task IEventCommand.CreateOutboundEventAsync(string topicName, string replayId, Dictionary<string, object?> fields)
    {
        throw new NotImplementedException();
    }
}