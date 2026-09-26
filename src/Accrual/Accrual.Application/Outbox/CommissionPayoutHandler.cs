using Accrual.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accrual.Application.Outbox;

public class CommissionPayoutHandler(IAccrualDbContext context, IWalletsClient walletsClient)
    : ICommissionPayoutHandler
{
    public async Task HandleAsync(CommissionPayoutMessage message, CancellationToken ct)
    {
        await walletsClient.CreditAsync(message, ct);

        var commission = await context.Commissions
            .SingleAsync(c => c.Id == message.CommissionId, ct);

        commission.PaidAt = DateTimeOffset.UtcNow;
    }
}