using ContentModerationService.Application.Queries;

namespace ContentModerationService.Api.Endpoints;

public static class ModerationResultEndpoints
{
    public static void MapModerationResultEndpoints(this WebApplication app)
    {
        const string tag = "Moderation Result";

        // Query Endpoints
        app.MapGet("/api/moderationresults", async (IModerationResultQuery query) =>
            {
                try
                {
                    var result = await query.GetModerationResultsAsync();

                    return Results.Ok(result);
                }
                catch (Exception)
                {
                    return Results.Problem(
                        "Unable to retrieve moderation results",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithTags(tag);


        app.MapGet("/api/moderationresults/{moderationResultId}",
            async (IModerationResultQuery query, int moderationResultId) =>
            {
                try
                {
                    var result = await query.GetModerationResultAsync(moderationResultId);

                    return result is null
                        ? Results.NotFound()
                        : Results.Ok(result);
                }
                catch (Exception)
                {
                    return Results.Problem(
                        "Unable to retrieve moderation result",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            }).WithTags(tag);
    }
}