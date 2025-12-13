using Eventbus.V1;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesforceService.Application.Commands;
using SalesforceService.Infrastructure.Auth;
using SalesforceService.Infrastructure.Helpers;
using SalesforceService.Infrastructure.Services.Schema;

namespace SalesforceService.Infrastructure.Messaging.Inbound;

public class SalesforceInboundSubscriber : BackgroundService, ISalesforceInboundSubscriber
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SalesforceInboundSubscriber> _logger;

    private readonly PubSub.PubSubClient _client;

    //private readonly DaprClient _daprClient;
    private readonly IConfiguration _config;
    private readonly ISalesforceSchemaService _schemaService;
    private readonly ISalesforceAuthService _authService;

    public SalesforceInboundSubscriber(
        IServiceScopeFactory scopeFactory,
        ILogger<SalesforceInboundSubscriber> logger,
        //DaprClient daprClient,
        PubSub.PubSubClient client,
        IConfiguration config,
        ISalesforceSchemaService schemaService,
        ISalesforceAuthService authService)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _client = client;
        //_daprClient = daprClient;
        _config = config;
        _schemaService = schemaService;
        _authService = authService;
    }

    private string _accessToken = string.Empty;
    private string _instanceUrl = string.Empty;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Salesforce Inbound Subscriber Service is starting...");

        try
        {
            // Get Access token and instance URL
            (_accessToken, _instanceUrl) = await _authService.GetSessionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log in to Salesforce. Stopping service.");
            return;
        }

        var topics = _config.GetSection("Salesforce:IndboundTopics").Get<string[]>()!;

        var tasks = topics.Select(topic =>
            Task.Run(() => StartTopicSubscriptionAsync(topic, cancellationToken), cancellationToken)).ToArray();

        await Task.WhenAll(tasks);
    }


    private async Task StartTopicSubscriptionAsync(string topicName, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await SubscribeToTopicAsync(topicName, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Salesforce Subscription for topic {Topic}. Reconnecting...", topicName);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }
    }

    private async Task SubscribeToTopicAsync(string topicName, CancellationToken cancellationToken)
    {
        // Tenant Id for multi-tenant support - for testing, hardcoded value
        var tenantId = _config["Salesforce:TenantId"]!;

        // Simplified metadata with session ID and instance URL for testing (Grpc.Core)
        var metadata = new Metadata
        {
            { "accesstoken", _accessToken },
            { "instanceurl", _instanceUrl },
            { "tenantid", tenantId }
        };

        // Start bidirectional streaming
        using var call = _client.Subscribe(metadata, cancellationToken: cancellationToken);

        // Send subscription request
        //var topicName = "/event/Cloud_News__e"; // Salesforce Platform Event API name
        var subscribeRequest = new FetchRequest
        {
            TopicName = topicName,
            ReplayPreset = ReplayPreset.Latest,
            NumRequested = 10
        };

        await call.RequestStream.WriteAsync(subscribeRequest, cancellationToken);

        _logger.LogInformation("Subscribing to: {Topic}", topicName);

        // Listen for events
        await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
        {
            if (response.Events.Count > 0)
            {
                _logger.LogInformation("Received {Count} events from Salesforce", response.Events.Count);
            }

            foreach (var evt in response.Events)
            {
                await ProcessEventAsync(topicName, evt);
            }
        }
    }

    private async Task ProcessEventAsync(string topicName, ConsumerEvent consumerEvent)
    {
        try
        {
            var replayId = consumerEvent.ReplayId.ToBase64();
            var schemaId = consumerEvent.Event.SchemaId;
            var payload = consumerEvent.Event.Payload.ToByteArray();

            _logger.LogInformation("Processing event with ReplayId: {ReplayId}", replayId);

            // Register schema
            _schemaService.RegisterTopicSchema(topicName, schemaId);

            // Get schema to deserialize payload
            var schema = await _schemaService.GetSchemaByIdAsync(schemaId);
            var eventData = AvroConverter.DeserializeAvroPayload(payload, schema);
            var fields = AvroConverter.ToDictionary(eventData);

            // Test: log event fields
            _logger.LogInformation("Event fields:");
            foreach (var pair in fields)
            {
                _logger.LogInformation($"  {pair.Key,-20}: {pair.Value}");
            }

            using var scope = _scopeFactory.CreateScope();
            var eventCommand = scope.ServiceProvider.GetRequiredService<IEventCommand>();

            // Application command handler
            await eventCommand.CreateInboundEventAsync(topicName, replayId, fields);

            /* Test result:

            CreatedDate: 1764894281124
            CreatedById: 005dL00001TQ0MbQAL
            RecordId__c: 500dL00002VS9blQAD
            Content__c: test description


            // inside ProcessAsync, after deserializing or extracting summary/hash:
            var inbound = new InboundEvent
            {
               Id = Guid.NewGuid(),
               SourceEventId = consumerEvent.ReplayId.ToBase64(),
               Topic = topicName,
               SchemaId = consumerEvent.Event.SchemaId,
               TenantId = tenantIdFromConfigOrMetadata,
               CorrelationId = correlationIdIfAny,
               PayloadSummary = JsonDocument.Parse("{\"refId\":\"500d...\"}"), // small canonical keys only
               PayloadHash = ComputeSha256Base64(consumerEvent.Event.Payload.ToByteArray()),
               Status = "received",
               IngestedAt = DateTimeOffset.UtcNow
            };


            */

            // Save to database with more data like eventId as correlation key
            // Publish dapr for internal
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Processing event");
        }

        //}
    }
}