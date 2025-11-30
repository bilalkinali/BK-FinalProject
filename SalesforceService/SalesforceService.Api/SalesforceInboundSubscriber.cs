//using Dapr.Client;
using Eventbus.V1;
using Grpc.Core;
using SalesforceService.Api.Auth;
using SalesforceService.Api.Helpers;
using SalesforceService.Api.Schema;

namespace SalesforceService.Api;

public class SalesforceInboundSubscriber : BackgroundService
{
    private readonly ILogger<SalesforceInboundSubscriber> _logger;
    private readonly PubSub.PubSubClient _client;
    //private readonly DaprClient _daprClient;
    private readonly IConfiguration _config;
    private readonly ISalesforceAuthService _authService;
    private readonly ISalesforceSchemaService _schemaService;

    public SalesforceInboundSubscriber(
        ILogger<SalesforceInboundSubscriber> logger,
        //DaprClient daprClient,
        PubSub.PubSubClient client,
        IConfiguration config,
        ISalesforceAuthService authService,
        ISalesforceSchemaService schemaService)
    {
        _logger = logger;
        _client = client;
        //_daprClient = daprClient;
        _config = config;
        _authService = authService;
        _schemaService = schemaService;
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
        //// Get Access token and instance URL
        //var (accessToken, instanceUrl) = await _authService.GetSessionAsync();

        //// Create gRPC channel
        //var channel = GrpcChannel.ForAddress(_config["Salesforce:PubSubEndpoint"]!);

        ///* Imported Salesforce.EventBus.V1 after building project since
        //adding ItemGroup <Protobuf Include="..\Protos\pubsub_api.proto" GrpcServices="Client" /> in .csproj */
        //var client = new PubSub.PubSubClient(channel);

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

            //// Request more events
            //await call.RequestStream.WriteAsync(new FetchRequest
            //{
            //    TopicName = topicName,
            //    NumRequested = 10
            //}, cancellationToken);
        }
    }

    private async Task ProcessEventAsync(string topicName, ConsumerEvent consumerEvent)
    {
        try
        {
            _logger.LogInformation("Processing event with ReplayId: {ReplayId}",
                consumerEvent.ReplayId.ToBase64());

            _schemaService.RegisterTopicSchema(topicName, consumerEvent.Event.SchemaId);

            // Need to parse AvroConverter payload - for test, just simulate in logs without content
            var schema = await _schemaService.GetSchemaByIdAsync(consumerEvent.Event.SchemaId);

            var eventData = AvroConverter.DeserializeAvroPayload(consumerEvent.Event.Payload.ToByteArray(), schema);

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
}