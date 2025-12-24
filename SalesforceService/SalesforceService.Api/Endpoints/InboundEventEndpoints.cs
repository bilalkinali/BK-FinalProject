using SalesforceService.Application.Queries;

namespace SalesforceService.Api.Endpoints;

public static class InboundEventEndpoints
{
    public static void MapInboundEventEndpoints(this WebApplication app)
    {
        const string tag = "Inbound Events";

        // Query Endpoints
        app.MapGet("/api/inboundevents",
            async (IEventQuery query) =>
            {
                try
                {
                    var result = await query.GetInboundEventsAsync();

                    return Results.Ok(result);
                }
                catch (Exception)
                {
                    return Results.Problem(
                        "Unable to retrieve inbound events",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithTags(tag);


        app.MapGet("/api/inboundevents/{inboundEventId}",
            async (IEventQuery query, int inboundEventId) =>
            {
                try
                {
                    var result = await query.GetInboundEventAsync(inboundEventId);

                    return result is null
                        ? Results.NotFound()
                        : Results.Ok(result);
                }
                catch (Exception e)
                {
                    return Results.Problem(
                        $"Unable to retrieve inbound event",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithTags(tag);
    }
}