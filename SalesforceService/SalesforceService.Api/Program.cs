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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();


app.MapGet("/test", () => "Hello World - From Salesforce Service"); 

app.Run();