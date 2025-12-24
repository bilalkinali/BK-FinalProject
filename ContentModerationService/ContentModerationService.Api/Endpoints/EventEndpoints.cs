using ContentModerationService.Application.Commands;
using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this WebApplication app)
    {
        var events = app.MapGroup("/events");

        // Event Endpoints
        events.MapPost("/contentmoderation",
            async (ILogger<Program> logger, ContentModerationDto payload, IContentModerationCommand command) =>
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
    }
}