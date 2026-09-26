namespace Wallets.Application.Dtos;

public sealed record CreditDto(Guid CommissionId, string EventExternalId, decimal Amount, DateTimeOffset CreditedAt);