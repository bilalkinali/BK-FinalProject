namespace SalesforceService.Domain.Entities;

public class InboundEvent
{
    protected InboundEvent()
    {
    }

    public InboundEvent(string topicName, string replayId, string recordId, string objectType)
    {
        EventId = Guid.NewGuid().ToString();
        TopicName = topicName;
        ReplayId = replayId;
        RecordId = recordId;
        ObjectType = objectType;
        CreatedAt = DateTime.UtcNow;
    }

    public string EventId { get; protected set; }
    public string TopicName { get; protected set; }
    public string ReplayId { get; protected set; }
    public string RecordId { get; protected set; }
    public string ObjectType { get; protected set; }
    public DateTime CreatedAt { get; protected set; }

    public static InboundEvent Create(string topicName, string replayId, string recordId, string objectType)
    {
        return new InboundEvent(topicName, replayId, recordId, objectType);
    }
}