namespace Accrual.Application.Dtos;

public sealed record EventDto(
    string ExternalId,
    string UserExternalId,
    decimal Profit,
    DateTimeOffset CreatedAt,
    IReadOnlyList<CommissionDto> Commissions);