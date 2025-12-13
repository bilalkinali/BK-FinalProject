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

    async Task IEventCommand.CreateInboundEventAsync(string salesforceTopic, string replayId, Dictionary<string, object?> fields)
    {
        var recordId = _idHelper.ExtractRecordId(fields)
            ?? throw new Exception("RecordId__c missing in Inbound Event");

        var objectType = _idHelper.ResolveObjectType(recordId); // Domain logic? Maybe value object

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Do
            var inboundEvent = InboundEvent.Create(salesforceTopic, replayId, recordId, objectType);

            // Save
            await _eventRepository.AddInboundEventAsync(inboundEvent);
            await _unitOfWork.CommitAsync();

            // Publish internal event
            await _eventHandler.PublishInboundEventAsync(salesforceTopic, inboundEvent.CorrelationId, fields);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inbound event for Topic: {SalesforceTopic}, ReplayId: {ReplayId}, RecordId: {RecordId}, ObjectType: {ObjectType}",
                salesforceTopic, replayId, recordId, objectType);

            await _unitOfWork.RollbackAsync();
            throw;
        }
        
    }

    async Task IEventCommand.CreateOutboundEventAsync(string topicName, string correlationId, string result)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Load
            var inboundEvent = await _eventRepository.GetInboundEventByCorrelationIdAsync(correlationId);

            // Do
            var outboundEvent = OutboundEvent.Create(correlationId, topicName, inboundEvent.RecordId, result);

            // Save
            await _eventRepository.AddOutboundEventAsync(outboundEvent);
            await _unitOfWork.CommitAsync();

            // Publish external event
            await _eventHandler.PublishOutboundEventAsync(topicName, outboundEvent.RecordId, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating outbound event for CorrelationId: {CorrelationId}, Result: {Result}",
                correlationId, result);
            throw;
        }
    }
}