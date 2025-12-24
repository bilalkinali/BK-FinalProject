using Eventbus.V1;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesforceService.Application;
using SalesforceService.Application.Queries;
using SalesforceService.Application.Services.Interfaces;
using SalesforceService.Application.Services.TopicDefinitions;
using SalesforceService.Infrastructure.Auth;
using SalesforceService.Infrastructure.Helpers;
using SalesforceService.Infrastructure.Messaging.Outbound;
using SalesforceService.Infrastructure.Queries;
using SalesforceService.Infrastructure.Repositories;
using SalesforceService.Infrastructure.Services;
using SalesforceService.Infrastructure.Services.Schema;
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

        // Publisher services
        services.AddScoped<IPublisherService, DaprPublisherService>();
        services.AddScoped<ISalesforcePublisherService, SalesforceOutboundPublisher>();

        // Topic definitions
        var topicConfig = new TopicDefinitionConfig();
        config.GetSection("topicDefinitions").Bind(topicConfig);

        services.AddSingleton(topicConfig);
        services.AddSingleton<ITopicDefinitionProvider, TopicDefinitionProvider>();


        // Database context
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork<SalesforceContext>>();

        // Queries
        services.AddScoped<IEventQuery, EventQuery>();
        services.AddScoped<IEventQueryMapper, EventQueryMapper>();

        // Add-Migration InitialMigration -Context SalesforceContext -Project SalesforceService.DatabaseMigration
        // Update-Database -Context SalesforceContext -Project SalesforceService.DatabaseMigration
        services.AddDbContext<SalesforceContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Default"),
                x => x.MigrationsAssembly("SalesforceService.DatabaseMigration")));

        return services;
    }
}