namespace SalesforceService.Application.Services.TopicDefinitions;

public interface IInboundTopicDefinitionProvider
{
    InboundTopicDefinition? GetBySalesforceTopic(string topicName);
}
