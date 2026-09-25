using Accrual.Application.Exceptions;
using Accrual.Application.Interfaces;
using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Accrual.Infrastructure.Persistence;

public class AccrualDbContext(DbContextOptions<AccrualDbContext> options) : DbContext(options), IAccrualDbContext
{
    public DbSet<Commission> Commissions => Set<Commission>();
    
    public DbSet<Event> Events => Set<Event>();
    
    public DbSet<SchemaChange> SchemaChanges => Set<SchemaChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccrualDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("Entity already exists.", ex);
        }
    }
}