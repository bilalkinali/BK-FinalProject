namespace SalesforceService.Application.Services.TopicDefinitions;

public interface IOutboundTopicDefinitionProvider
{
    OutboundTopicDefinition? GetByInternalTopic(string topicName);
}
