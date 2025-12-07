using Eventbus.V1;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesforceService.Infrastructure.Auth;

namespace SalesforceService.Infrastructure.Schema;

public class SalesforceSchemaService : ISalesforceSchemaService
{
    private readonly ILogger<SalesforceSchemaService> _logger;
    private readonly PubSub.PubSubClient _client;
    private readonly IConfiguration _config;
    private readonly ISalesforceAuthService _auth;

    private readonly Dictionary<string, string> _topicToSchemaId = new();   // topicName -> schemaId
    private readonly Dictionary<string, Avro.Schema> _schemaCache = new();  // schemaId  -> schema

    public SalesforceSchemaService(
        ILogger<SalesforceSchemaService> logger,
        PubSub.PubSubClient client,
        IConfiguration config,
        ISalesforceAuthService auth)
    {
        _logger = logger;
        _client = client;
        _config = config;
        _auth = auth;
    }

    public async Task PreloadSchemaIdForTopicAsync(string topicName)
    {
        _logger.LogInformation("Preloading schema for topic: {Topic}", topicName);

        var (accessToken, instanceUrl) = await _auth.GetSessionAsync();

        var metadata = new Metadata
        {
            { "accesstoken", accessToken },
            { "instanceurl", instanceUrl },
            { "tenantid", _config["Salesforce:TenantId"]! }
        };

        // Get topic Metadata
        var topicRequest = new TopicRequest { TopicName = topicName };
        var topicInfo = await _client.GetTopicAsync(topicRequest, metadata);

        var schemaId = topicInfo.SchemaId;

        // Save mapping
        _topicToSchemaId[topicName] = schemaId;
        
        _logger.LogInformation("Preloaded topic {Topic}: schemaId: {SchemaId}",
            topicName, schemaId);
    }

    public void RegisterTopicSchema(string topic, string schemaId)
    {
        _topicToSchemaId.TryAdd(topic, schemaId);
        _logger.LogInformation("Registered schema ID {SchemaId} for topic {Topic}", schemaId, topic);
    }

    public async Task<Avro.Schema> GetSchemaByTopicAsync(string topic)
    {
        if (!_topicToSchemaId.TryGetValue(topic, out var schemaId))
        {
            throw new KeyNotFoundException($"No schemaId registered for topic: {topic}");
        }

        return await GetSchemaByIdAsync(schemaId);
    }

    public async Task<(string schemaId, Avro.Schema schema)> GetSchemaWithIdByTopicAsync(string topic)
    {
        if (!_topicToSchemaId.TryGetValue(topic, out var schemaId))
        {
            throw new KeyNotFoundException($"No schemaId registered for topic: {topic}");
        }

        var schema = await GetSchemaByIdAsync(schemaId);
        return (schemaId, schema);
    }

    public async Task<Avro.Schema> GetSchemaByIdAsync(string schemaId)
    {
        if (_schemaCache.TryGetValue(schemaId, out var cached))
            return cached;

        _logger.LogInformation("Fetching schema with ID: {SchemaId}", schemaId);

        var (accessToken, instanceUrl) = await _auth.GetSessionAsync();

        var metadata = new Metadata
        {
            { "accesstoken", accessToken },
            { "instanceurl", instanceUrl },
            { "tenantid", _config["Salesforce:TenantId"]! }
        };

        var request = new SchemaRequest { SchemaId = schemaId };
        var response = await _client.GetSchemaAsync(request, metadata);

        var schema = Avro.Schema.Parse(response.SchemaJson);

        _logger.LogInformation("Successfully fetched schema with ID: {SchemaId}", schemaId);

        _schemaCache.TryAdd(schemaId, schema);

        _logger.LogInformation("Cached schemaId: {SchemaId}", schemaId);

        return schema;
    }
}