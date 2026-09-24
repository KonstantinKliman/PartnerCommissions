namespace Accrual.Domain;

public sealed record CommissionLine(Guid BeneficiaryId, int Level, decimal Amount, SchemaType Schema);