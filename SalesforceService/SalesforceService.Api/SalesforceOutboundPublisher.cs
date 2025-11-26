using Avro.Generic;
using Avro.IO;
using Eventbus.V1;
using Grpc.Core;
using SalesforceService.Api.Auth;
using SalesforceService.Api.Helpers;
using SalesforceService.Api.Schema;

namespace SalesforceService.Api;

public class SalesforceOutboundPublisher
{
    private readonly ILogger<SalesforceOutboundPublisher> _logger;
    private readonly PubSub.PubSubClient _client;
    private readonly IConfiguration _config;
    private readonly ISalesforceAuthService _authService;
    private readonly ISalesforceSchemaService _schemaService;

    public SalesforceOutboundPublisher(
        ILogger<SalesforceOutboundPublisher> logger,
        PubSub.PubSubClient client,
        IConfiguration config,
        ISalesforceAuthService authService,
        ISalesforceSchemaService schemaService
        )
    {
        _logger = logger;
        _client = client;
        _config = config;
        _authService = authService;
        _schemaService = schemaService;
    }

    public async Task PublishAsync(string topicName, string payload)
    {
        var (accessToken, instanceUrl) = await _authService.GetSessionAsync();
        var tenantId = _config["Salesforce:TenantId"]!;

        var metadata = new Metadata
        {
            { "accesstoken", accessToken },
            { "instanceurl", instanceUrl },
            { "tenantid", tenantId }
        };

        // Fetch schema for outbount event
        var schema = await _schemaService.GetSchemaByTopicAsync(topicName);

        // Convert payload to Avro binary
        var avroBytes = AvroConverter.SerializeJsonToAvro(payload, schema);
    }
}