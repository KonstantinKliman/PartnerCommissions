using Microsoft.EntityFrameworkCore;
using Npgsql;
using Users.Application.Exceptions;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Infrastructure.Persistence;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options), IUsersDbContext
{
    public DbSet<User> Users => Set<User>();

    private const long TreeLockKey = 1_001;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
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
    
    public Task LockTreeAsync(CancellationToken ct)
    {
        return Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock({TreeLockKey})", ct);
    }
}