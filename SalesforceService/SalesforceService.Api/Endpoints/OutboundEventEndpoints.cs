using SalesforceService.Application.Queries;

namespace SalesforceService.Api.Endpoints;

public static class OutboundEventEndpoints
{
    public static void MapOutboundEventEndpoints(this WebApplication app)
    {
        const string tag = "Outbound Events";

        // Query Endpoints
        app.MapGet("/api/outboundevents",
            async (IEventQuery query) =>
            {
                try
                {
                    var result = await query.GetOutboundEventsAsync();

                    return Results.Ok(result);
                }
                catch (Exception)
                {
                    return Results.Problem(
                        "Unable to retrieve outbound events",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithTags(tag);


        app.MapGet("/api/outboundevents/{outboundEventId}",
            async (IEventQuery query, int outboundEventId) =>
            {
                try
                {
                    var result = await query.GetOutboundEventAsync(outboundEventId);

                    return result is null
                        ? Results.NotFound()
                        : Results.Ok(result);
                }
                catch (Exception e)
                {
                    return Results.Problem(
                        $"Unable to retrieve outbound event",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithTags(tag);
    }
}