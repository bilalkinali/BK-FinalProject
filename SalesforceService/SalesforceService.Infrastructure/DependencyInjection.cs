using Eventbus.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesforceService.Application.Services.TopicDefinitions;
using SalesforceService.Infrastructure.Auth;
using SalesforceService.Infrastructure.Schema;
using SalesforceService.Infrastructure.TopicDefinitions;

namespace SalesforceService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // Token cache for Salesforce auth tokens
        services.AddSingleton<SalesforceTokenCache>();

        // Auth service
        services.AddHttpClient<ISalesforceAuthService, SalesforceAuthService>();

        // Schema service
        services.AddSingleton<ISalesforceSchemaService, SalesforceSchemaService>();

        // gRPC channel
        services.AddSingleton(sp =>
            GrpcChannel.ForAddress(config["Salesforce:PubSubEndpoint"]!)
        );

        // gRPC PubSub client
        services.AddSingleton(sp =>
        {
            var channel = sp.GetRequiredService<GrpcChannel>();
            return new PubSub.PubSubClient(channel);
        });


        // Topic definitions
        var topicConfig = new TopicDefinitionConfig();
        config.GetSection("topics").Bind(topicConfig);

        services.AddSingleton(topicConfig);
        services.AddSingleton<ITopicDefinitionProvider, TopicDefinitionProvider>();

        return services;
    }
}