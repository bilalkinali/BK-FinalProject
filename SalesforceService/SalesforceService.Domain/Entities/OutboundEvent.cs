namespace SalesforceService.Domain.Entities;

public class OutboundEvent : DomainEntity
{
    protected OutboundEvent()
    {
    }

    private OutboundEvent(string correlationId, string salesforceTopic, string recordId, string result)
    {
        CorrelationId = correlationId;
        SalesforceTopic = salesforceTopic;
        RecordId = recordId;
        Result = result;
        CreatedAt = DateTime.UtcNow;
    }

    public string CorrelationId { get; protected set; } // Correlation Id from Inbound Event
    public string SalesforceTopic { get; protected set; } // Salesforce outbound Topic Name
    public string RecordId { get; protected set; } // Salesforce Record Id
    public string Result { get; protected set; } // Result of processing (e.g., Accept, Reject)
    public DateTime CreatedAt { get; protected set; }

    public static OutboundEvent Create(string correlationId, string salesforceTopic, string recordId, string result)
    {
        return new OutboundEvent(correlationId, salesforceTopic, recordId, result);
    }
}