using SalesforceService.Api;
using SalesforceService.Api.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Http Client for Salesforce Auth Service
builder.Services.AddHttpClient<SalesforceAuthService>();

builder.Services.AddDaprClient();

builder.Services.AddSingleton<SalesforceAuthService>();
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