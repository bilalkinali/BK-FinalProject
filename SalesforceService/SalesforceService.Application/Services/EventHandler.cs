
using Microsoft.Extensions.Logging;
using SalesforceService.Application.Services.Interfaces;
using SalesforceService.Application.Services.TopicDefinitions;

namespace SalesforceService.Application.Services;

public class EventHandler : IEventHandler
{
    private readonly ITopicDefinitionProvider _topicDefinitions;
    private readonly IPublisherService _publisherService;
    private readonly ILogger<EventHandler> _logger;

    public EventHandler(
        ITopicDefinitionProvider topicDefinitions,
        IPublisherService publisherService,
        ILogger<EventHandler> logger)
    {
        _topicDefinitions = topicDefinitions;
        _publisherService = publisherService;
        _logger = logger;
    }
    async Task IEventHandler.HandleAsync(string topicName, string eventId, Dictionary<string, object?> fields)
    {
        var topicDefinition = _topicDefinitions.GetTopicDefinition(topicName)
            ?? throw new Exception($"Uknown topic: {topicName}");

        switch (topicDefinition.DtoType)
        {
            case "CaseSubmitted":
                if (!fields.TryGetValue("Case_Description__c", out var content) || content is null)
                    throw new Exception($"Missing required field 'Case_Description__c' for topic '{topicName}'");

                var contentValue = content.ToString();

                var caseSubmittedDto = new CaseSubmittedDto(eventId, contentValue!);

                _logger.LogInformation("Publishing {Dto} to topic {Topic}", typeof(CaseSubmittedDto), topicDefinition.InternalTopic);
                _logger.LogInformation("EventId: {EventId}, Content: {Content}", caseSubmittedDto.EventId, caseSubmittedDto.Content);

                await _publisherService.PublishAsync(topicDefinition.InternalTopic, caseSubmittedDto);
                break;

            default:
                throw new NotSupportedException($"Unhandled DtoType: {topicDefinition.DtoType}");
        }
    }
}

internal record CaseSubmittedDto (string EventId, string Content); // Move to Dtos folder