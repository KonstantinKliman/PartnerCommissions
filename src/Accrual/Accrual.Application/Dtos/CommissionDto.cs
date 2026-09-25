using Accrual.Domain;

namespace Accrual.Application.Dtos;

public sealed record CommissionDto(
    string BeneficiaryExternalId,
    int Level,
    decimal Amount,
    SchemaType SchemaType,
    bool IsPaid,
    DateTimeOffset? PaidAt);