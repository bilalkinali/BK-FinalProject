using SalesforceService.Application.Services.TopicDefinitions;

namespace SalesforceService.Infrastructure.TopicDefinitions;

// ref: https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#custom-configuration-provider
// ref: https://github.com/andrewlock/NetEscapades.Configuration

public class InboundTopicDefinitionProvider : IInboundTopicDefinitionProvider
{
    private readonly Dictionary<string, InboundTopicDefinition> _bySalesforceTopic;
    public InboundTopicDefinitionProvider(TopicDefinitionConfig config)
    {
        // {Key} topicName | {Value} TopicDefinition
        _bySalesforceTopic = config.Inbound
            .ToDictionary(
                topicDef => topicDef.SalesforceTopic,
                topicDef => topicDef);
    }

    InboundTopicDefinition? IInboundTopicDefinitionProvider.GetBySalesforceTopic(string topic)
    {
        //_lookup.TryGetValue(topicName, out var topicDefinition);

        Console.WriteLine($"Requested key: '{topic}'");

        Console.WriteLine("Available keys:");
        foreach (var key in _bySalesforceTopic.Keys)
            Console.WriteLine($" - '{key}'");

        _bySalesforceTopic.TryGetValue(topic, out var def);

        Console.WriteLine(def == null
            ? "Result: NULL"
            : $"Result: FOUND → {def.InternalTopic}");

        return def;
    }
}