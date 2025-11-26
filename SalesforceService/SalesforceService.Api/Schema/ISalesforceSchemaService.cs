namespace SalesforceService.Api.Schema;

public interface ISalesforceSchemaService
{
    Task<Avro.Schema> GetSchemaAsync(string schemaId);
}