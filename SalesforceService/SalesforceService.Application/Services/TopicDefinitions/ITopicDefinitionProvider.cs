namespace SalesforceService.Application.Services.TopicDefinitions;

public interface ITopicDefinitionProvider
{
    InboundTopicDefinition? GetBySalesforceTopic(string topicName);
    OutboundTopicDefinition? GetByInternalTopic(string internalTopic);
}
