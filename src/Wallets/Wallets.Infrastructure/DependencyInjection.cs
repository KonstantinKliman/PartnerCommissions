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
    
    public static async Task ApplyMigrationsAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<WalletsDbContext>();
        await db.Database.MigrateAsync();
    }
}