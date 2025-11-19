using Avro;
using Eventbus.V1;
using Grpc.Core;
using SalesforceService.Api.Auth;

namespace SalesforceService.Api;

public class SalesforceSchemaService
{
    private readonly ILogger<SalesforceSchemaService> _logger;
    private readonly PubSub.PubSubClient _client;
    private readonly IConfiguration _configuration;
    private readonly SalesforceAuthService _authService;

    private readonly Dictionary<string, Schema> _cache = new();

    public SalesforceSchemaService(
        ILogger<SalesforceSchemaService> logger,
        PubSub.PubSubClient client,
        IConfiguration configuration,
        SalesforceAuthService authService)
    {
        _logger = logger;
        _client = client;
        _configuration = configuration;
        _authService = authService;
    }

    public async Task<Schema> GetSchemaAsync(string schemaId)
    {
        if (_cache.TryGetValue(schemaId, out var cached))
            return cached;

        _logger.LogInformation("Fetching schema with ID: {SchemaId}", schemaId);

        var (accessToken, instanceUrl) = await _authService.GetSessionAsync();

        var tenantId = _configuration["Salesforce:TenantId"] ?? null;

        var metadata = new Metadata
        {
            { "accesstoken", accessToken },
            { "instanceurl", instanceUrl },
            { "tenantid", tenantId }
        };

        var schemaRequest = new SchemaRequest { SchemaId = schemaId };
        var schemaResponse = await _client.GetSchemaAsync(schemaRequest, metadata);

        var schema = Schema.Parse(schemaResponse.SchemaJson);

        _logger.LogInformation("Successfully fetched schema with ID: {SchemaId}", schemaId);

        _cache[schemaId] = schema;
        return schema;
    }
}