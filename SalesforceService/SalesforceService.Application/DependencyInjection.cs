using Microsoft.Extensions.DependencyInjection;
using SalesforceService.Application.Commands;
using SalesforceService.Application.Helpers;

namespace SalesforceService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddScoped<IRecordIdentificationHelper, RecordIdentificationHelper>();
        services.AddScoped<IEventCommand, EventCommand>();


        return services;
    }
}