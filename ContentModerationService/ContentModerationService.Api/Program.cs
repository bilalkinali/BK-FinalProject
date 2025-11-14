using Action = ContentModerationService.Domain.Enums.Action;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();


app.MapGet("/test", () => "Hello World! - From Content Moderation Service");


app.MapPost("/moderation", (ILogger<Program> logger, ContentModerationDto payload) =>
    {
        logger.LogInformation("Content for moderation received:\n" +
                              "ContentId: {Id}\n" +
                              "Content: {Content}", payload.ContentId, payload.Content);

        // Simulate moderation logic

        logger.LogInformation("Content moderated:\n" +
                              "ContentId: {Id}\n" +
                              "Action: {Action}", payload.ContentId, Action.Accept);

        return new ContentModeratedDto(ContentId: payload.ContentId, Result: Action.Accept);
    });

app.Run();

public record ContentModerationDto (string ContentId, string Content);
public record ContentModeratedDto(string ContentId, Action Result);