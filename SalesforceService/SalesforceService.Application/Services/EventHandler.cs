using Microsoft.Extensions.Logging;
using SalesforceService.Application.Services.Interfaces;
using SalesforceService.Application.Services.TopicDefinitions;

namespace SalesforceService.Application.Services;

public class EventHandler : IEventHandler
{
    private readonly ITopicDefinitionProvider _topicDefinitionProvider;
    private readonly IPublisherService _publisherService;
    private readonly ISalesforcePublisherService _salesforcePublisherService;
    private readonly ILogger<EventHandler> _logger;

    public EventHandler(
        ITopicDefinitionProvider topicDefinitionProvider,
        IPublisherService publisherService,
        ISalesforcePublisherService salesforcePublisherService,
        ILogger<EventHandler> logger)
    {
        _topicDefinitionProvider = topicDefinitionProvider;
        _publisherService = publisherService;
        _salesforcePublisherService = salesforcePublisherService;
        _logger = logger;
    }

    async Task IEventHandler.PublishInboundEventAsync(string salesforceTopic, string correlationId, Dictionary<string, object?> fields)
    {
        var topicDefinition = _topicDefinitionProvider.GetBySalesforceTopic(salesforceTopic)
            ?? throw new Exception($"Uknown topic: {salesforceTopic}");

        switch (topicDefinition.DtoType)
        {
            case "ContentSubmittedDto":
                if (!fields.TryGetValue("Case_Description__c", out var content) || content is null)
                    throw new Exception($"Missing required field 'Case_Description__c' for topic '{salesforceTopic}'");

                var contentValue = content.ToString();

                var caseSubmittedDto = new ContentSubmittedDto(correlationId, contentValue!);

                _logger.LogInformation("Publishing {Dto} to topic {Topic}", topicDefinition.DtoType, topicDefinition.InternalTopic);
                _logger.LogInformation("EventId: {EventId}, Content: {Content}", caseSubmittedDto.CorrelationId, caseSubmittedDto.Content);

                await _publisherService.PublishAsync(topicDefinition.InternalTopic, caseSubmittedDto);
                break;

            default:
                throw new NotSupportedException($"Unhandled DtoType: {topicDefinition.DtoType}");
        }
    }

    async Task IEventHandler.PublishOutboundEventAsync(string internalTopic, string recordId, string result)
    {
        var topicDefintion = _topicDefinitionProvider.GetByInternalTopic(internalTopic)
            ?? throw new Exception($"Uknown topic: {internalTopic}");

        switch (topicDefintion.DtoType)
        {
            case "ContentModeratedDto":
                var payload = new Dictionary<string, object?>
                {
                    ["Case_Id__c"] = recordId,
                    ["Moderation_Result__c"] = result,
                    ["CreatedDate"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    ["CreatedById"] = "005dL00001TQ0MbQAL",
                    ["EventUuid"] = Guid.NewGuid(),
                };

                _logger.LogInformation(
                    "Publishing to Salesforce topic {Topic}", topicDefintion.SalesforceTopic);

                await _salesforcePublisherService.PublishAsync(topicDefintion.SalesforceTopic, payload);
                break;

            default:
                throw new NotSupportedException($"Unhandled DtoType: {topicDefintion.DtoType}");
        }
    }
}

internal record ContentSubmittedDto(string CorrelationId, string Content); // Move to Dtos folder