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

    private readonly Dictionary<string, Avro.Schema> _cache = new();

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

    async Task<Avro.Schema> ISalesforceSchemaService.GetSchemaAsync(string schemaId)
    {
        if (_cache.TryGetValue(schemaId, out var cached))
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

        _cache[schemaId] = schema;
        return schema;
    }
}