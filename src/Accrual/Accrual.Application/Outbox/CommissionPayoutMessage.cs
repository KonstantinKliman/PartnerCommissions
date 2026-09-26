namespace Accrual.Application.Outbox;

public sealed record CommissionPayoutMessage(
    Guid CommissionId, string BeneficiaryExternalId, string EventExternalId, decimal Amount)
{
    public const string Type = "CommissionPayout";
}