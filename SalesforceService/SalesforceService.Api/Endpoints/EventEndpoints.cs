using SalesforceService.Application.Services;
using SalesforceService.Application.Services.Interfaces;

namespace SalesforceService.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this WebApplication app)
    {
        var events = app.MapGroup("/events");

        // Event Endpoints
        events.MapPost("/contentmoderated", async (
            IModerationResultHandler moderationResultHandler,
            ContentModeratedDto contentModeratedDto) =>
        {
            var topic = "content-moderated";
            Console.WriteLine("Received content moderation event");
            Console.WriteLine($"Topic: {topic}");
            Console.WriteLine($"CorrelationId: {contentModeratedDto.CorrelationId}");
            Console.WriteLine($"Result: {contentModeratedDto.Result}");

            await moderationResultHandler.HandleModerationResultAsync(topic, contentModeratedDto);

            return Results.Ok();
        }).WithTopic("pubsub", "content-moderated");



        events.MapPost("/contentmoderationfailed", async (
            IModerationResultHandler moderationResultHandler,
            ContentModeratedDto contentModeratedDto) =>
        {
            var topic = "content-moderation-failed";
            Console.WriteLine("Received content moderation failed event");
            Console.WriteLine($"Topic: {topic}");
            Console.WriteLine($"CorrelationId: {contentModeratedDto.CorrelationId}");
            Console.WriteLine($"Result: {contentModeratedDto.Result}");

            await moderationResultHandler.HandleModerationResultAsync(topic, contentModeratedDto);

            return Results.Ok();
        }).WithTopic("pubsub", "content-moderation-failed");
    }
}
