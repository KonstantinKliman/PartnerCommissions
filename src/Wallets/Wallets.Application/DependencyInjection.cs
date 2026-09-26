using Microsoft.Extensions.DependencyInjection;
using Wallets.Application.Interfaces;
using Wallets.Application.Services;

namespace Wallets.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWalletsService, WalletsService>();
        
        return services;
    }
}