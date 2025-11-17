using Eventbus.V1;
using Grpc.Net.Client;
using SalesforceService.Api;
using SalesforceService.Api.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDaprClient();

// Http Client for Salesforce Auth Service
builder.Services.AddHttpClient<SalesforceAuthService>();


// App Services
builder.Services.AddSingleton<SalesforceAuthService>();
builder.Services.AddSingleton<SalesforceSchemaService>();


// Register gRPC Client for PubSub
builder.Services.AddSingleton(_ =>
    GrpcChannel.ForAddress(builder.Configuration["Salesforce:PubSubEndpoint"]!));
// Then register the PubSubClient using the channel
builder.Services.AddSingleton(sp =>
    new PubSub.PubSubClient(sp.GetRequiredService<GrpcChannel>()));


// Background subscriber service
builder.Services.AddHostedService<SalesforceInboundSubscriber>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();


app.MapGet("/test", () => "Hello World - From Salesforce Service"); 

app.Run();