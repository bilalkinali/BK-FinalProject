using SalesforceService.Api.Endpoints;
using SalesforceService.Application;
using SalesforceService.Application.Services.Interfaces;
using SalesforceService.Infrastructure;
using SalesforceService.Infrastructure.Services.Schema;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDaprClient();

var path = Path.Combine(builder.Environment.ContentRootPath,
    "config",
    "topic-definitions.yaml");

Console.WriteLine(path);

// Load Salesforce settings from YAML
builder.Configuration.AddYamlFile(path, optional: false);

// Dependency Injection
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();


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


using (var scope = app.Services.CreateScope())
{
    var schemaService = scope.ServiceProvider.GetRequiredService<ISalesforceSchemaService>();
    var outboundTopics = builder.Configuration
        .GetSection("Salesforce:OutboundTopics")
        .Get<string[]>()!;
    foreach (var topic in outboundTopics)
    {
        try
        {
            await schemaService.PreloadSchemaIdForTopicAsync(topic);
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Failed to preload schema for topic {Topic}.", topic);
        }
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseCloudEvents();
app.MapSubscribeHandler();

//app.UseHttpsRedirection();


// Testing
app.MapGet("/test", () => "Hello World - From Salesforce Service");

app.MapPost("/salesforce/test-publish", async (
    ISalesforcePublisherService publisher) =>
{
    // Testing - publish a test event to Salesforce
    var payload = new Dictionary<string, object?>
    {
        ["RecordId__c"] = "500dL00002Ppt4fQAB",
        ["Moderation_Result__c"] = "Accept",

        ["CreatedDate"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        ["CreatedById"] = "005dL00001TQ0MbQAL",
        ["EventUuid"] = Guid.NewGuid().ToString()
    };

    await publisher.PublishAsync("/event/Case_Moderation_Event__e", payload);

    return Results.Ok("Published test event to Salesforce.");
});

app.MapEventEndpoints();
app.MapInboundEventEndpoints();
app.MapOutboundEventEndpoints();

app.Run();