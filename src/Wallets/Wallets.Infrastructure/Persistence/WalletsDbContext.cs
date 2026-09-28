using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wallets.Application.Exceptions;
using Wallets.Application.Interfaces;
using Wallets.Domain.Entities;

namespace Wallets.Infrastructure.Persistence;

public class WalletsDbContext(DbContextOptions<WalletsDbContext> options) : DbContext(options), IWalletsDbContext
{
    public DbSet<Wallet> Wallets => Set<Wallet>();
    
    public DbSet<WalletCredit> WalletCredits => Set<WalletCredit>();
    
    public async Task<Guid> EnsureWalletAsync(string userExternalId, CancellationToken ct)
    {
        var newId = Guid.CreateVersion7();
        await Database.ExecuteSqlAsync($"""
                                        INSERT INTO wallets (id, user_external_id, created_at)
                                        VALUES ({newId}, {userExternalId}, now())
                                        ON CONFLICT (user_external_id) DO NOTHING
                                        """, ct);

        return await Wallets
            .Where(w => w.UserExternalId == userExternalId)
            .Select(w => w.Id)
            .FirstAsync(ct);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletsDbContext).Assembly);

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