using Wallets.Application.Dtos;

namespace Wallets.Application.Interfaces;

public interface IWalletsService
{
    Task<CreditResult> CreditAsync(string userExternalId, Guid commissionId, string eventExternalId, decimal amount, CancellationToken ct);
    
    Task<BalanceDto> GetBalanceAsync(string userExternalId, CancellationToken ct);

    Task<List<CreditDto>> GetCreditsAsync(string userExternalId, int page, int pageSize, CancellationToken ct);
}