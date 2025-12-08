
using SalesforceService.Application.Services.Interfaces;
using SalesforceService.Application.Services.TopicDefinitions;

namespace SalesforceService.Application.Services;

public class EventHandler : IEventHandler
{
    private readonly ITopicDefinitionProvider _topicDefinitions;
    private readonly IPublisherService _publisherService;

    public EventHandler(
        ITopicDefinitionProvider topicDefinitions,
        IPublisherService publisherService)
    {
        _topicDefinitions = topicDefinitions;
        _publisherService = publisherService;
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

                await _publisherService.PublishAsync(topicDefinition.InternalTopic, caseSubmittedDto);
                break;

            default:
                throw new NotSupportedException($"Unhandled DtoType: {topicDefinition.DtoType}");
        }
    }
}

internal record CaseSubmittedDto (string EventId, string Content);