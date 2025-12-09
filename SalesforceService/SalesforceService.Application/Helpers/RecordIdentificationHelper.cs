
namespace SalesforceService.Application.Helpers;

public class RecordIdentificationHelper : IRecordIdentificationHelper
{
    string? IRecordIdentificationHelper.ExtractRecordId(Dictionary<string, object?> fields)
    {
        if (fields.TryGetValue("RecordId__c", out var value) && value != null)
            return value.ToString();

        return null;
    }


    string IRecordIdentificationHelper.ResolveObjectType(string recordId)
    {
        var prefix = recordId[..3];

        return prefix switch
        {
            "500" => "Case",
            "001" => "Account",
            "003" => "Contact",
            _ when prefix.StartsWith("a") => "CustomObject",
            _ => "Unknown"
        };
    }
}