using Microsoft.Extensions.Logging;
using SalesforceService.Application.Helpers;
using SalesforceService.Application.Services.Interfaces;
using SalesforceService.Domain.Entities;

namespace SalesforceService.Application.Commands;

public class EventCommand : IEventCommand
{
    private readonly IRecordIdentificationHelper _idHelper;
    private readonly IEventHandler _eventHandler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventCommand> _logger;

    public EventCommand(
        IRecordIdentificationHelper idHelper,
        IEventHandler eventHandler,
        IUnitOfWork unitOfWork,
        IEventRepository eventRepository,
        ILogger<EventCommand> logger)
    {
        _idHelper = idHelper;
        _eventHandler = eventHandler;
        _unitOfWork = unitOfWork;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    async Task IEventCommand.CreateInboundEventAsync(string topicName, string replayId, Dictionary<string, object?> fields)
    {
        var recordId = _idHelper.ExtractRecordId(fields)
            ?? throw new Exception("RecordId__c missing in Inbound Event");

        var objectType = _idHelper.ResolveObjectType(recordId); // Domain logic? Maybe value object

        try
        {
            // Do
            var inboundEvent = InboundEvent.Create(topicName, replayId, recordId, objectType);

            // Save
            await _eventRepository.AddInboundEventAsync(inboundEvent);
            await _unitOfWork.CommitAsync();

            // Publish internal event
            await _eventHandler.HandleAsync(topicName, inboundEvent.EventId, fields);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inbound event for Topic: {TopicName}, ReplayId: {ReplayId}, RecordId: {RecordId}, ObjectType: {ObjectType}",
                topicName, replayId, recordId, objectType);

            await _unitOfWork.RollbackAsync();
            throw;
        }
        
    }

    async Task IEventCommand.CreateOutboundEventAsync(string topicName, string replayId, Dictionary<string, object?> fields)
    {
        throw new NotImplementedException();
    }
}