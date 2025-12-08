using SalesforceService.Application.Services.TopicDefinitions;

namespace SalesforceService.Infrastructure.TopicDefinitions;

// ref: https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#custom-configuration-provider
// ref: https://github.com/andrewlock/NetEscapades.Configuration

public class TopicDefinitionProvider : ITopicDefinitionProvider
{
    private readonly Dictionary<string, TopicDefinition> _lookup;
    public TopicDefinitionProvider(TopicDefinitionConfig config)
    {
        // topicName -> TopicDefinition
        _lookup = config.Topics
            .ToDictionary(
                topicDef => topicDef.SalesforceTopic,
                topicDef => topicDef);
    }

    TopicDefinition? ITopicDefinitionProvider.GetTopicDefinition(string topicName)
    {
        _lookup.TryGetValue(topicName, out var topicDefinition);
        return topicDefinition;
    }
}