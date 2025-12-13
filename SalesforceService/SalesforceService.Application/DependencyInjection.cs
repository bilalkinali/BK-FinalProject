using Microsoft.Extensions.DependencyInjection;
using SalesforceService.Application.Commands;
using SalesforceService.Application.Helpers;
using SalesforceService.Application.Services;
using SalesforceService.Application.Services.Interfaces;
using EventHandler = SalesforceService.Application.Services.EventHandler;

namespace SalesforceService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddScoped<IRecordIdentificationHelper, RecordIdentificationHelper>();
        services.AddScoped<IEventCommand, EventCommand>();
        services.AddScoped<IEventHandler, EventHandler>();

        return services;
    }
}