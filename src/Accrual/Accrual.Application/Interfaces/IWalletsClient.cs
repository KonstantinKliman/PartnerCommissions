using Accrual.Application.Outbox;

namespace Accrual.Application.Interfaces;

public interface IWalletsClient
{
    Task CreditAsync(CommissionPayoutMessage message, CancellationToken ct);
}