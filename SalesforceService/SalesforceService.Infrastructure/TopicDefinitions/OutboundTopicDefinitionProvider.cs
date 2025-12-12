using SalesforceService.Application.Services.TopicDefinitions;

namespace SalesforceService.Infrastructure.TopicDefinitions;

public class OutboundTopicDefinitionProvider : IOutboundTopicDefinitionProvider
{
    private readonly Dictionary<string, OutboundTopicDefinition> _byInternalTopic;
    public OutboundTopicDefinitionProvider(TopicDefinitionConfig config)
    {
        _byInternalTopic = config.Outbound
            .ToDictionary(
                topicDef => topicDef.InternalTopic,
                topicDef => topicDef);
    }

    OutboundTopicDefinition? IOutboundTopicDefinitionProvider.GetByInternalTopic(string topic)
        => _byInternalTopic.TryGetValue(topic, out var def) ? def : null;
}