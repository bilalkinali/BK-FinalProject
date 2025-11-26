//using Dapr.Client;
using Avro.Generic;
using Avro.IO;
using Eventbus.V1;
using Grpc.Core;
using SalesforceService.Api.Auth;
using SalesforceService.Api.Schema;

namespace SalesforceService.Api;

public class SalesforceInboundSubscriber : BackgroundService
{
    private readonly ILogger<SalesforceInboundSubscriber> _logger;
    private readonly PubSub.PubSubClient _client;
    //private readonly DaprClient _daprClient;
    private readonly IConfiguration _configuration;
    private readonly ISalesforceAuthService _authService;
    private readonly ISalesforceSchemaService _schemaService;

    public SalesforceInboundSubscriber(
        ILogger<SalesforceInboundSubscriber> logger,
        //DaprClient daprClient,
        PubSub.PubSubClient client,
        IConfiguration configuration,
        ISalesforceAuthService authService,
        ISalesforceSchemaService schemaService)
    {
        _logger = logger;
        _client = client;
        //_daprClient = daprClient;
        _configuration = configuration;
        _authService = authService;
        _schemaService = schemaService;
    }    

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Salesforce Inbound Subscriber Service is starting...");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await SubscribeToSalesforceAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Salesforce Subscription. Reconnecting...");
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }
    }

    private async Task SubscribeToSalesforceAsync(CancellationToken cancellationToken)
    {
        // Get Access token and instance URL
        var (accessToken, instanceUrl) = await _authService.GetSessionAsync();

        //// Create gRPC channel
        //var channel = GrpcChannel.ForAddress(_configuration["Salesforce:PubSubEndpoint"]!);

        ///* Imported Salesforce.EventBus.V1 after building project since
        //adding ItemGroup <Protobuf Include="..\Protos\pubsub_api.proto" GrpcServices="Client" /> in .csproj */
        //var client = new PubSub.PubSubClient(channel);

        // Tenant Id for multi-tenant support - for testing, hardcoded value
        var tenantId = _configuration["Salesforce:TenantId"]!;

        // Simplified metadata with session ID and instance URL for testing (Grpc.Core)
        var metadata = new Metadata
        {
            { "accesstoken", accessToken },
            { "instanceurl", instanceUrl },
            { "tenantid", tenantId }
        };

        // Start bidirectional streaming
        using var call = _client.Subscribe(metadata, cancellationToken: cancellationToken);

        // Send subscription request
        var topicName = "/event/Cloud_News__e"; // Salesforce Platform Event API name
        var subscribeRequest = new FetchRequest
        {
            TopicName = topicName,
            ReplayPreset = ReplayPreset.Latest,
            NumRequested = 10
        };

        await call.RequestStream.WriteAsync(subscribeRequest, cancellationToken);

        _logger.LogInformation("Subscribed to Salesforce topics {Topic}", topicName);

        // Listen for events
        await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
        {
            _logger.LogInformation("Received {Count} events from Salesforce", response.Events.Count);

            foreach (var evt in response.Events)
            {
                await ProcessEventAsync(evt);
            }

            //// Request more events
            //await call.RequestStream.WriteAsync(new FetchRequest
            //{
            //    TopicName = topicName,
            //    NumRequested = 10
            //}, cancellationToken);
        }
    }

    private async Task ProcessEventAsync(ConsumerEvent consumerEvent)
    {
        try
        {
            _logger.LogInformation("Processing event with ReplayId: {ReplayId}",
                consumerEvent.ReplayId.ToBase64());

            // Need to parse Avro payload - for test, just simulate in logs without content
            var schema = await _schemaService.GetSchemaAsync(consumerEvent.Event.SchemaId);

            var eventData = DeserializeAvroPayload(consumerEvent.Event.Payload.ToByteArray(), schema);

            _logger.LogInformation("Event fields:");

            foreach (var field in eventData.Schema.Fields)
            {
                var value = eventData[field.Name] ?? "(null)";
                _logger.LogInformation($"  {field.Name,-20}: {value}");
            }

            // Publish dapr for internal
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Processing event");
        }

    }

    private GenericRecord DeserializeAvroPayload(byte[] payload, Avro.Schema schema)
    {
        using var stream = new MemoryStream(payload);
        var reader = new GenericDatumReader<GenericRecord>(schema, schema);
        var decoder = new BinaryDecoder(stream);
        return reader.Read(null, decoder);
    }
}