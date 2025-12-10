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

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapSubscribeHandler();

//app.UseHttpsRedirection();


app.MapGet("/test", () => "Hello World! - From Content Moderation Service");


app.MapPost("/moderation", (ILogger<Program> logger, ContentModerationDto payload) =>
    {
        logger.LogInformation("Content for moderation received:\n" +
                              "EventId: {Id}\n" +
                              "Content: {Content}", payload.EventId, payload.Content);

        // Simulate moderation logic

        logger.LogInformation("Content moderated:\n" +
                              "EventId: {Id}\n" +
                              "Action: {Action}", payload.EventId, Action.Accept);

        return new ContentModeratedDto(EventId: payload.EventId, Result: Action.Accept);
    });

app.MapPost("/events/contentmoderation", 
    async (ILogger<Program> logger,
        ContentModerationDto payload,
        IContentModerationCommand command) =>
{
    try
    {
        logger.LogInformation("Event received for content moderation:\n" +
                          "EventId: {Id}\n" +
                          "Content: {Content}", payload.EventId, payload.Content);

        var decision = await command.ModerateContentAsync(MediaType.Text, payload.Content);

        logger.LogInformation("Decision made by AI: {Decision}", decision.SuggestedAction);

        var contentModeratedDto = new ContentModeratedDto(payload.EventId, decision.SuggestedAction);
        // Publish


        return Results.Created();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return Results.Problem(ex.Message);
    }
});

app.Run();

public record ContentModerationDto (string EventId, string Content);
public record ContentModeratedDto(string EventId, Action Result);