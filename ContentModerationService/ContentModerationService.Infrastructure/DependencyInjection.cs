using ContentModerationService.Application;
using ContentModerationService.Application.Configuration;
using ContentModerationService.Application.Services;
using ContentModerationService.Application.Services.ProxyInterface;
using ContentModerationService.Infrastructure.Configuration;
using ContentModerationService.Infrastructure.Helpers;
using ContentModerationService.Infrastructure.Repositories;
using ContentModerationService.Infrastructure.ServiceProxyImpl;
using ContentModerationService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContentModerationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ContentModerationSettings>(configuration.GetSection("ContentModerationSettings"));
        services.Configure<RejectionThresholds>(configuration.GetSection("RejectionThresholds"));

        services.AddScoped<IRequestBuilder, RequestBuilder>();
        services.AddScoped<IRejectionThresholdProvider, RejectionThresholdProvider>();
        services.AddScoped<IAzureContentSafetyProxy, AzureContentSafetyProxy>();
        services.AddScoped<IContentDetection, ContentDetection>();
        services.AddScoped<IPublisherService, DaprPublisherService>();

        // Database context
        services.AddScoped<IModerationDecisionRepository, ModerationDecisionRepository>();

        // Add-Migration InitialMigration -Context ContentModerationContext -Project ContentModerationService.DatabaseMigration
        // Update-Database -Context ContentModerationContext -Project ContentModerationService.DatabaseMigration
        services.AddDbContext<ContentModerationContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default"),
                x => x.MigrationsAssembly("ContentModerationService.DatabaseMigration")));



        return services;
    }
}