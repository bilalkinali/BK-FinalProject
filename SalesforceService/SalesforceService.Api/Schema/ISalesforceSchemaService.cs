namespace SalesforceService.Api.Schema;

public interface ISalesforceSchemaService
{
    Task PreloadSchemaIdForTopicAsync(string topicName);
    void RegisterTopicSchema(string topic, string schemaId);
    Task<Avro.Schema> GetSchemaByTopicAsync(string topic);
    Task<(string schemaId, Avro.Schema schema)> GetSchemaWithIdByTopicAsync(string topic);
    Task<Avro.Schema> GetSchemaByIdAsync(string schemaId);
}