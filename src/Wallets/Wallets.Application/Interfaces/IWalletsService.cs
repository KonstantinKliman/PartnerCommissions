using Wallets.Application.Dtos;

namespace Wallets.Application.Interfaces;

public interface IWalletsService
{
    Task<CreditResult> CreditAsync(string userExternalId, Guid commissionId, string eventExternalId, decimal amount, CancellationToken ct);
}