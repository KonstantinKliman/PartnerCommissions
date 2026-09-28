using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using Wallets.Application.Dtos;
using Wallets.Application.Interfaces;
using Wallets.Application.Mappings;
using Wallets.Application.Metrics;
using Wallets.Domain.Entities;

namespace Wallets.Application.Services;

public class WalletsService(IWalletsDbContext context, WalletsMetrics metrics) : IWalletsService
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

            metrics.CreditDuplicate();
            
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

            metrics.CreditDuplicate();
            
            return new CreditResult(winner.ToDto(), IsCreated: false);
        }

        metrics.CreditCreated(amount);
        
        return new CreditResult(credit.ToDto(), IsCreated: true);
    }

    public async Task<BalanceDto> GetBalanceAsync(string userExternalId, CancellationToken ct)
    {
        var balance = await context.Wallets
            .Where(w => w.UserExternalId == userExternalId)
            .SelectMany(w => w.Credits)
            .SumAsync(c => c.Amount, ct);

        return new BalanceDto(userExternalId, balance);
    }

    public async Task<List<CreditDto>> GetCreditsAsync(string userExternalId, int page, int pageSize, CancellationToken ct)
    {
        return await context.Wallets
            .Where(w => w.UserExternalId == userExternalId)
            .SelectMany(w => w.Credits)
            .OrderByDescending(c => c.CreditedAt)
            .ThenByDescending(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CreditDto(c.CommissionId, c.EventExternalId, c.Amount, c.CreditedAt))
            .ToListAsync(ct);
    }

    private static void ThrowIfDataDifferent(WalletCredit walletCredit, Guid? walletId, string eventExternalId, decimal amount)
    {
        if (walletCredit.WalletId != walletId || walletCredit.EventExternalId != eventExternalId || walletCredit.Amount != amount)
            throw new ConflictException($"Commission '{walletCredit.CommissionId}' is already credited with different data.");
    }
}