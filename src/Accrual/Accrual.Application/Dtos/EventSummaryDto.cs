namespace Accrual.Application.Dtos;

public sealed record EventSummaryDto(string ExternalId, decimal Profit, DateTimeOffset CreatedAt);