using Eventbus.V1;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesforceService.Application.Services.Interfaces;
using SalesforceService.Infrastructure.Auth;
using SalesforceService.Infrastructure.Helpers;
using SalesforceService.Infrastructure.Services.Schema;

namespace SalesforceService.Infrastructure.Messaging.Outbound;

public class SalesforceOutboundPublisher : ISalesforcePublisherService
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

    async Task ISalesforcePublisherService.PublishAsync(string salesforceTopic, Dictionary<string, Object?> payload)
    {
        var (accessToken, instanceUrl) = await _authService.GetSessionAsync();

        var metadata = new Metadata
        {
            { "accesstoken", accessToken },
            { "instanceurl", instanceUrl },
            { "tenantid", _config["Salesforce:TenantId"]! }
        };

        // Fetch schema with Id for outbount event
        var (schemaId, schema) = await _schemaService.GetSchemaWithIdByTopicAsync(salesforceTopic);

        // Convert payload to Avro binary
        var avroBytes = AvroConverter.SerializeToAvroBytes(payload, schema);

        // Build ProducerEvent
        var producerEvent = new ProducerEvent
        {
            SchemaId = schemaId,
            Payload = Google.Protobuf.ByteString.CopyFrom(avroBytes)
        };

        // Build request
        var request = new PublishRequest
        {
            TopicName = salesforceTopic
        };

        request.Events.Add(producerEvent);

        // Publish event
        var response = await _client.PublishAsync(request, metadata);

        _logger.LogInformation(
            "Published event to topic {Topic}. CorrelationKey={CorrelationKey}, Error={Error}, ReplayId={ReplayId}",
            salesforceTopic,
            response.Results[0].CorrelationKey,
            response.Results[0].Error,
            response.Results[0].ReplayId.ToBase64()
        );


    }
}