using Accrual.Application.Interfaces;
using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accrual.Infrastructure.Persistence;

public class AccrualDbContext(DbContextOptions<AccrualDbContext> options) : DbContext(options), IAccrualDbContext
{
    public DbSet<Commission> Commissions => Set<Commission>();
    
    public DbSet<Event> Events => Set<Event>();
    
    public DbSet<AccrualSettings> Settings => Set<AccrualSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccrualDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}