using Eventbus.V1;
using Grpc.Core;
using SalesforceService.Api.Auth;

namespace SalesforceService.Api.Schema;

public class SalesforceSchemaService : ISalesforceSchemaService
{
    private readonly ILogger<SalesforceSchemaService> _logger;
    private readonly PubSub.PubSubClient _client;
    private readonly IConfiguration _configuration;
    private readonly ISalesforceAuthService _authService;

    private readonly Dictionary<string, Avro.Schema> _schemaCache = new();
    private readonly Dictionary<string, string> _topicToSchema = new();

    public SalesforceSchemaService(
        ILogger<SalesforceSchemaService> logger,
        PubSub.PubSubClient client,
        IConfiguration configuration,
        ISalesforceAuthService authService)
    {
        _logger = logger;
        _client = client;
        _configuration = configuration;
        _authService = authService;
    }

    public void RegisterTopicSchema(string topic, string schemaId)
    {
        if (!_topicToSchema.ContainsKey(topic))
        {
            _topicToSchema[topic] = schemaId;
            _logger.LogInformation("Registered schema ID {SchemaId} for topic {Topic}", schemaId, topic);
        }
    }

    public async Task<Avro.Schema> GetSchemaByTopicAsync(string topic)
    {
        if (!_topicToSchema.TryGetValue(topic, out var schemaId))
        {
            throw new KeyNotFoundException($"No schema registered for topic: {topic}");
        }

        return await GetSchemaByIdAsync(schemaId);
    }

    public async Task<Avro.Schema> GetSchemaByIdAsync(string schemaId)
    {
        if (_schemaCache.TryGetValue(schemaId, out var cached))
            return cached;

        _logger.LogInformation("Fetching schema with ID: {SchemaId}", schemaId);

        var (accessToken, instanceUrl) = await _authService.GetSessionAsync();

        var tenantId = _configuration["Salesforce:TenantId"]!;

        var metadata = new Metadata
        {
            { "accesstoken", accessToken },
            { "instanceurl", instanceUrl },
            { "tenantid", tenantId }
        };

        var schemaRequest = new SchemaRequest { SchemaId = schemaId };
        var schemaResponse = await _client.GetSchemaAsync(schemaRequest, metadata);

        var schema = Avro.Schema.Parse(schemaResponse.SchemaJson);

        _logger.LogInformation("Successfully fetched schema with ID: {SchemaId}", schemaId);

        _schemaCache[schemaId] = schema;
        return schema;
    }
}