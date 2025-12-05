using Eventbus.V1;
using Grpc.Net.Client;
using SalesforceService.Api;
using SalesforceService.Api.Auth;
using SalesforceService.Api.Schema;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDaprClient();

// Auth and Schema services
builder.Services.AddSingleton<SalesforceTokenCache>();
builder.Services.AddHttpClient<ISalesforceAuthService, SalesforceAuthService>();
//builder.Services.AddSingleton<ISalesforceAuthService, SalesforceAuthService>();
builder.Services.AddSingleton<ISalesforceSchemaService, SalesforceSchemaService>();


// gRPC infrastructure (long-lived)
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return GrpcChannel.ForAddress(configuration["Salesforce:PubSubEndpoint"]!);
});

// gRPC PubSub client (factory?)
builder.Services.AddSingleton(sp =>
{
    var channel = sp.GetRequiredService<GrpcChannel>();
    return new PubSub.PubSubClient(channel);
});


// Background subscriber service
builder.Services.AddHostedService<SalesforceInboundSubscriber>(); // Use interface
builder.Services.AddScoped<SalesforceOutboundPublisher>(); // Use interface


var app = builder.Build();


//// TESTING - Preload schemas for outbound topics

var schemaService = app.Services.GetRequiredService<ISalesforceSchemaService>();

var outboundTopics = builder.Configuration
    .GetSection("Salesforce:OutboundTopics")
    .Get<string[]>()!;

foreach (var topic in outboundTopics)
{
    await schemaService.PreloadSchemaIdForTopicAsync(topic);
}

//// END TESTING


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();


app.MapGet("/test", () => "Hello World - From Salesforce Service");

app.MapPost("/salesforce/test-publish", async (
    SalesforceOutboundPublisher publisher) =>
{
    // Testing - publish a test event to Salesforce
    var payload = new Dictionary<string, object?>
    {
        ["Case_Id__c"] = "500dL00002Ppt4fQAB",
        ["Moderation_Result__c"] = "Accept",

        ["CreatedDate"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        ["CreatedById"] = "005dL00001TQ0MbQAL",
        ["EventUuid"] = Guid.NewGuid().ToString()
    };

    await publisher.PublishAsync("/event/Case_Moderation_Event__e", payload);

    return Results.Ok("Published test event to Salesforce.");
});


app.Run();