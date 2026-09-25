using Microsoft.EntityFrameworkCore;
using Wallets.Domain.Entities;

namespace Wallets.Application.Interfaces;

public interface IWalletsDbContext
{
    DbSet<Wallet> Wallets { get; }

    DbSet<WalletCredit> WalletCredits { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);

    Task<Guid> EnsureWalletAsync(string userExternalId, CancellationToken ct);
}