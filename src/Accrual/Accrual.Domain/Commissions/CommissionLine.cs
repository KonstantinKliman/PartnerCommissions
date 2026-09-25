namespace Accrual.Domain.Commissions;

public sealed record CommissionLine(string BeneficiaryExternalId, int Level, decimal Amount, SchemaType Schema);