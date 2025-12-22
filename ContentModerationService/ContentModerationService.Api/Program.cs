using ContentModerationService.Application;
using ContentModerationService.Infrastructure;
using ContentModerationService.Application.Commands;
using ContentModerationService.Domain.Enums;
using Action = ContentModerationService.Domain.Enums.Action;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();
builder.Services.AddDaprClient();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseCloudEvents();
app.MapSubscribeHandler();

//app.UseHttpsRedirection();


app.MapGet("/test", () => "Hello World! - From Content Moderation Service");


app.MapPost("/moderation", (ILogger<Program> logger, ContentModerationDto payload) =>
    {
        logger.LogInformation("Content for moderation received:\n" +
                              "EventId: {Id}\n" +
                              "Content: {Content}", payload.CorrelationId, payload.Content);

        // Simulate moderation logic

        logger.LogInformation("Content moderated:\n" +
                              "EventId: {Id}\n" +
                              "Action: {Action}", payload.CorrelationId, Action.Accept);

        return new ContentModeratedDto(CorrelationId: payload.CorrelationId, Result: Action.Accept);
    });

app.MapPost("/events/contentmoderation", 
    async (ILogger<Program> logger,
        ContentModerationDto payload,
        IContentModerationCommand command) =>
{
    try
    {
        logger.LogInformation(
            "Event received for content moderation:\n" +
            "EventId: {Id}\n" +
            "Content: {Content}", payload.CorrelationId, payload.Content);

        await command.ModerateContentAsync(
            MediaType.Text, 
            payload.CorrelationId, 
            payload.Content);

        return Results.Created();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing content moderation for EventId: {Id}", payload.CorrelationId);
        return Results.Problem("Content moderation failed");
    }
}).WithTopic("pubsub", "content-submitted");

app.Run();

public record ContentModerationDto (string CorrelationId, string Content);
public record ContentModeratedDto(string CorrelationId, Action Result);