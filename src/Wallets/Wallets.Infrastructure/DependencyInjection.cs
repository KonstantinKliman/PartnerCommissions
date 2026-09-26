using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallets.Application.Interfaces;
using Wallets.Infrastructure.Persistence;

namespace Wallets.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<WalletsDbContext>(opt => opt.UseNpgsql(config.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());

        services.AddHealthChecks()
            .AddDbContextCheck<WalletsDbContext>(tags: ["ready"]);
        
        services.AddScoped<IWalletsDbContext>(sp => sp.GetRequiredService<WalletsDbContext>());
        
        return services;
    }
}