namespace SalesforceService.Application.Services.TopicDefinitions;

public class TopicDefinitionConfig
{
    public List<InboundTopicDefinition> Inbound { get; set; } = new();
    public List<OutboundTopicDefinition> Outbound { get; set; } = new();
}

public class InboundTopicDefinition
{
    public string SalesforceTopic { get; set; }
    public string InternalTopic { get; set; }
    public string DtoType { get; set; }
    public string Description { get; set; }
}

public class OutboundTopicDefinition
{
    public string InternalTopic { get; set; }
    public string SalesforceTopic { get; set; }
    public string DtoType { get; set; }
    public string Description { get; set; }
}