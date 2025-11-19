using ContentModerationService.Application.Configuration;
using ContentModerationService.Infrastructure.Helpers;
using ContentModerationService.Infrastructure.ServiceProxyImpl;
using ContentModerationService.Application.Services.ProxyInterface;
using ContentModerationService.Infrastructure.Configuration;
using ContentModerationService.Infrastructure.Services;
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
        return services;
    }
}