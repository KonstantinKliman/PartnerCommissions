using Accrual.Application.Interfaces;
using Accrual.Infrastructure.Clients;
using Accrual.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accrual.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AccrualDbContext>(opt => opt.UseNpgsql(config.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());
        
        services.AddHealthChecks()
            .AddDbContextCheck<AccrualDbContext>(tags: ["ready"]);

        services.AddHttpClient<IUsersClient, UsersClient>(client =>
        {
            var usersBaseUrl = config["Services:Users:BaseUrl"];
            if (usersBaseUrl is null)
                throw new InvalidOperationException("Services:Users:BaseUrl is not configured");
            client.BaseAddress = new Uri(usersBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        
        services.AddHttpClient<IWalletsClient, WalletsClient>(client =>
        {
            var walletsBaseUrl = config["Services:Wallets:BaseUrl"];
            if (walletsBaseUrl is null)
                throw new InvalidOperationException("Services:Wallets:BaseUrl is not configured");
            client.BaseAddress = new Uri(walletsBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        
        services.AddScoped<IAccrualDbContext>(sp => sp.GetRequiredService<AccrualDbContext>());
        
        return services;
    }
}