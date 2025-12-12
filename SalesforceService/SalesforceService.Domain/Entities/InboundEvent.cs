using System.ComponentModel.DataAnnotations;

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

    [Key]
    public string EventId { get; protected set; } // Used as correlation id for downstream processing
    public string TopicName { get; protected set; } // Salesforce Topic Name
    public string ReplayId { get; protected set; } // Salesforce Replay Id
    public string RecordId { get; protected set; } // Salesforce Record Id
    public string ObjectType { get; protected set; } // Salesforce Object Type (e.g., Case, Account)
    public DateTime CreatedAt { get; protected set; }

    public static InboundEvent Create(string topicName, string replayId, string recordId, string objectType)
    {
        return new InboundEvent(topicName, replayId, recordId, objectType);
    }
}