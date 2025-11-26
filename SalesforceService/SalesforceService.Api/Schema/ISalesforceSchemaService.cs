namespace SalesforceService.Api.Schema;

public interface ISalesforceSchemaService
{
    void RegisterTopicSchema(string topic, string schemaId);
    Task<Avro.Schema> GetSchemaByTopicAsync(string topic);

    Task<Avro.Schema> GetSchemaByIdAsync(string schemaId);
}