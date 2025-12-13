namespace SalesforceService.Domain.Entities;

public class InboundEvent : DomainEntity
{
    protected InboundEvent()
    {
    }

    public InboundEvent(string salesforceTopic, string replayId, string recordId, string objectType)
    {
        CorrelationId = Guid.NewGuid().ToString();
        SalesforceTopic = salesforceTopic;
        ReplayId = replayId;
        RecordId = recordId;
        ObjectType = objectType;
        CreatedAt = DateTime.UtcNow;
    }
    
    public string CorrelationId { get; protected set; } // Used for downstream processing
    public string SalesforceTopic { get; protected set; } // Salesforce Topic Name
    public string ReplayId { get; protected set; } // Salesforce Replay Id
    public string RecordId { get; protected set; } // Salesforce Record Id
    public string ObjectType { get; protected set; } // Salesforce Object Type (e.g., Case, Account)
    public DateTime CreatedAt { get; protected set; }

    public static InboundEvent Create(string topicName, string replayId, string recordId, string objectType)
    {
        return new InboundEvent(topicName, replayId, recordId, objectType);
    }
}