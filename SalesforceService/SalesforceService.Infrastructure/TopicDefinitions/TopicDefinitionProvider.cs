using SalesforceService.Application.Services.TopicDefinitions;

namespace SalesforceService.Infrastructure.TopicDefinitions;

// ref: https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#custom-configuration-provider
// ref: https://github.com/andrewlock/NetEscapades.Configuration

public class TopicDefinitionProvider : ITopicDefinitionProvider
{
    private readonly Dictionary<string, InboundTopicDefinition> _bySalesforceTopic;
    private readonly Dictionary<string, OutboundTopicDefinition> _byInternalTopic;
    public TopicDefinitionProvider(TopicDefinitionConfig config)
    {
        _bySalesforceTopic = config.Inbound
            .ToDictionary(
                topicDef => topicDef.SalesforceTopic,
                topicDef => topicDef);

        _byInternalTopic = config.Outbound
            .ToDictionary(
                topicDef => topicDef.InternalTopic,
                topicDef => topicDef);
    }

    InboundTopicDefinition? ITopicDefinitionProvider.GetBySalesforceTopic(string salesforceTopic)
        => _bySalesforceTopic.TryGetValue(salesforceTopic, out var def) ? def : null;

    OutboundTopicDefinition? ITopicDefinitionProvider.GetByInternalTopic(string internalTopic)
        => _byInternalTopic.TryGetValue(internalTopic, out var def) ? def : null;

}