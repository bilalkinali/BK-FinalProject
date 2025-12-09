namespace SalesforceService.Application.Services.TopicDefinitions;

public interface ITopicDefinitionProvider
{
    TopicDefinition? GetTopicDefinition(string topicName);
}
