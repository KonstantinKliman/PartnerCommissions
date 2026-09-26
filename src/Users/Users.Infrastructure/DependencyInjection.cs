using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Interfaces;
using Users.Infrastructure.Persistence;

namespace Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<UsersDbContext>(opt => opt.UseNpgsql(config.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());

        services.AddHealthChecks()
            .AddDbContextCheck<UsersDbContext>(tags: ["ready"]);
        
        services.AddScoped<IUsersDbContext>(sp => sp.GetRequiredService<UsersDbContext>());
        
        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await db.Database.MigrateAsync();
    }
}