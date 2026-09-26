using Accrual.Application.Interfaces;
using Accrual.Application.Metrics;
using Accrual.Application.Outbox;
using Accrual.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Accrual.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISchemaService, SchemaService>();
        services.AddScoped<IEventsService, EventsService>();
        
        services.AddScoped<ICommissionPayoutHandler, CommissionPayoutHandler>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();

        services.AddSingleton<AccrualMetrics>();
        
        return services;
    }
}