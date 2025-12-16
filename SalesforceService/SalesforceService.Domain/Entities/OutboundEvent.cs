namespace SalesforceService.Domain.Entities;

public class OutboundEvent : DomainEntity
{
    protected OutboundEvent()
    {
    }

    private OutboundEvent(string correlationId, string internalTopic, string recordId, string result)
    {
        CorrelationId = correlationId;
        InternalTopic = internalTopic;
        RecordId = recordId;
        Result = result;
        CreatedAt = DateTime.UtcNow;
    }

    public string CorrelationId { get; protected set; } // Correlation Id from Inbound Event
    public string InternalTopic { get; protected set; } // Internal Topic Name
    public string RecordId { get; protected set; } // Salesforce Record Id
    public string Result { get; protected set; } // Result of processing (e.g., Accept, Reject)
    public DateTime CreatedAt { get; protected set; }

    public static OutboundEvent Create(string correlationId, string internalTopic, string recordId, string result)
    {
        return new OutboundEvent(correlationId, internalTopic, recordId, result);
    }
}