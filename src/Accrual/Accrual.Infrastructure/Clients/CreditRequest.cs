namespace Accrual.Infrastructure.Clients;

internal sealed record CreditRequest(Guid CommissionId, string EventExternalId, decimal Amount);