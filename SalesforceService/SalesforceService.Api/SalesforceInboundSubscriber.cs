//using Dapr.Client;
using Eventbus.V1;
using Grpc.Core;
using Grpc.Net.Client;
using SalesforceService.Api.Auth;

namespace SalesforceService.Api;

public class SalesforceInboundSubscriber : BackgroundService
{
    private readonly ILogger<SalesforceInboundSubscriber> _logger;
    //private readonly DaprClient _daprClient;
    private readonly IConfiguration _configuration;
    private readonly SalesforceAuthService _authService;

    public SalesforceInboundSubscriber(
        ILogger<SalesforceInboundSubscriber> logger,
        //DaprClient daprClient,
        IConfiguration configuration,
        SalesforceAuthService authService)
    {
        _logger = logger;
        //_daprClient = daprClient;
        _configuration = configuration;
        _authService = authService;
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
        // Get Session ID and instance URL
        var (sessionId, instanceUrl) = await _authService.GetSessionAsync();

        // Create gRPC channel
        var channel = GrpcChannel.ForAddress(_configuration["Salesforce:PubSubEndpoint"]!);

        /* Imported Salesforce.EventBus.V1 after building project since
        adding ItemGroup <Protobuf Include="..\Protos\pubsub_api.proto" GrpcServices="Client" /> in .csproj */
        var client = new PubSub.PubSubClient(channel);

        // Simplified metadata with session ID and instance URL for testing (Grpc.Core)
        var metadata = new Metadata
        {
            { "accesstoken", sessionId },
            { "instanceurl", instanceUrl },
            { "tenantid", "unused" } // This is not needed with session auth, but required
        };

        // Start bidirectional streaming
        using var call = client.Subscribe(metadata, cancellationToken: cancellationToken);

        // Send subscription request
        var topicName = "/event/CloudNews__e"; // Salesforce Platform Event API name
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

            // Request more events
            await call.RequestStream.WriteAsync(new FetchRequest
            {
                TopicName = topicName,
                NumRequested = 10
            }, cancellationToken);
        }
    }

    private async Task ProcessEventAsync(ConsumerEvent consumerEvent)
    {
        try
        {
            _logger.LogInformation("Processing event with ReplayId: {ReplayId}",
                consumerEvent.ReplayId.ToBase64());

            // Need to payy Avro payload - for test, just simulate in logs without content
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Processing event");
        }

    }
}