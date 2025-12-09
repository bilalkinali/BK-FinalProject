namespace SalesforceService.Application.Helpers;

public interface IRecordIdentificationHelper
{
    string? ExtractRecordId(Dictionary<string, object?> fields);
    string ResolveObjectType(string recordId);
}