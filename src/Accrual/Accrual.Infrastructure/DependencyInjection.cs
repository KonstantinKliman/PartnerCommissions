using Accrual.Application.Interfaces;
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
        
        services.AddScoped<IAccrualDbContext>(sp => sp.GetRequiredService<AccrualDbContext>());
        
        return services;
    }
}