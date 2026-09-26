using Accrual.Application.Interfaces;
using Accrual.Infrastructure.Clients;
using Accrual.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

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
            })
            .AddStandardResilienceHandler(opt =>
            {
                opt.Retry.MaxRetryAttempts = 2;
                opt.Retry.Delay = TimeSpan.FromMilliseconds(200);
                opt.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
                opt.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(15);
                opt.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
                opt.CircuitBreaker.MinimumThroughput = 5;
                opt.CircuitBreaker.FailureRatio = 0.5;
                opt.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
            });

        services.AddHttpClient<IWalletsClient, WalletsClient>(client =>
            {
                var walletsBaseUrl = config["Services:Wallets:BaseUrl"];
                if (walletsBaseUrl is null)
                    throw new InvalidOperationException("Services:Wallets:BaseUrl is not configured");
                client.BaseAddress = new Uri(walletsBaseUrl);
            })
            .AddResilienceHandler("wallets", pipelineBuilder =>
                {
                    pipelineBuilder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                    {
                        SamplingDuration = TimeSpan.FromSeconds(10),
                        MinimumThroughput = 5,
                        FailureRatio = 0.5,
                        BreakDuration = TimeSpan.FromSeconds(15)
                    });
                    pipelineBuilder.AddTimeout(TimeSpan.FromSeconds(2));
                }
            );

        services.AddScoped<IAccrualDbContext>(sp => sp.GetRequiredService<AccrualDbContext>());

        return services;
    }
}