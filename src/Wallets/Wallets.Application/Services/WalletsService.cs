using Microsoft.EntityFrameworkCore;
using Wallets.Application.Dtos;
using Wallets.Application.Exceptions;
using Wallets.Application.Interfaces;
using Wallets.Application.Mappings;
using Wallets.Domain.Entities;

namespace Wallets.Application.Services;

public class WalletsService(IWalletsDbContext context) : IWalletsService
{
    public async Task<CreditResult> CreditAsync(
        string userExternalId, Guid commissionId, string eventExternalId, decimal amount, CancellationToken ct)
    {
        var existingWalletCredit = await context.WalletCredits
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommissionId == commissionId, ct);

        if (existingWalletCredit is not null)
        {
            var existingWalletId = await context.Wallets
                .Where(w => w.UserExternalId == userExternalId)
                .Select(w => (Guid?)w.Id)
                .FirstOrDefaultAsync(ct);

            ThrowIfDataDifferent(existingWalletCredit, existingWalletId, eventExternalId, amount);

            return new CreditResult(existingWalletCredit.ToDto(), IsCreated: false);
        }

        var walletId = await context.EnsureWalletAsync(userExternalId, ct);

        var credit = new WalletCredit
        {
            WalletId = walletId,
            CommissionId = commissionId,
            EventExternalId = eventExternalId,
            Amount = amount
        };

        context.WalletCredits.Add(credit);
        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (ConflictException)
        {
            var winner = await context.WalletCredits
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CommissionId == commissionId, ct);

            if (winner is null)
                throw;

            var userExternalIdWalletId = await context.Wallets
                .Where(w => w.UserExternalId == userExternalId)
                .Select(w => (Guid?)w.Id)
                .FirstOrDefaultAsync(ct);

            ThrowIfDataDifferent(winner, userExternalIdWalletId, eventExternalId, amount);

            return new CreditResult(winner.ToDto(), IsCreated: false);
        }

        return new CreditResult(credit.ToDto(), IsCreated: true);
    }
    
    private static void ThrowIfDataDifferent(WalletCredit walletCredit, Guid? walletId, string eventExternalId, decimal amount)
    {
        if (walletCredit.WalletId != walletId || walletCredit.EventExternalId != eventExternalId || walletCredit.Amount != amount)
            throw new ConflictException($"Commission '{walletCredit.CommissionId}' is already credited with different data.");
    }
}