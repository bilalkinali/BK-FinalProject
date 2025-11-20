using ContentModerationService.Application.Commands;
using ContentModerationService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContentModerationService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IContentModerationCommand, ContentModerationCommand>();
        services.AddScoped<IDecisionService, DecisionService>();
        return services;
    }
}