using SalesforceService.Application;
using SalesforceService.Infrastructure;
using SalesforceService.Infrastructure.Messaging.Inbound;
using SalesforceService.Infrastructure.Messaging.Outbound;

//using SalesforceService.Api.Schema;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDaprClient();

// Load Salesforce settings from YAML
builder.Configuration.AddYamlFile("config/salesforce-settings.yaml", optional: false, reloadOnChange: true);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

// Background subscriber service
builder.Services.AddHostedService<SalesforceInboundSubscriber>();
builder.Services.AddScoped<SalesforceOutboundPublisher>(); // Use interface


var app = builder.Build();


//// TESTING - Preload schemas for outbound topics

//var schemaService = app.Services.GetRequiredService<ISalesforceSchemaService>();

//var outboundTopics = builder.Configuration
//    .GetSection("Salesforce:OutboundTopics")
//    .Get<string[]>()!;

//foreach (var topic in outboundTopics)
//{
//    await schemaService.PreloadSchemaIdForTopicAsync(topic);
//}

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