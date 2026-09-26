using Accrual.Application.Outbox;

namespace Accrual.Application.Interfaces;

public interface ICommissionPayoutHandler
{
    Task HandleAsync(CommissionPayoutMessage message, CancellationToken ct);
}
