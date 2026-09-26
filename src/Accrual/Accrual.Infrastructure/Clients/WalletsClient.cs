using System.Net.Http.Json;
using Accrual.Application.Interfaces;
using Accrual.Application.Outbox;

namespace Accrual.Infrastructure.Clients;

public class WalletsClient(HttpClient httpClient) : IWalletsClient
{
    public async Task CreditAsync(CommissionPayoutMessage message, CancellationToken ct)
    {
        var request = new CreditRequest(message.CommissionId, message.EventExternalId, message.Amount);

        using var response = await httpClient.PostAsJsonAsync(
            $"wallets/{Uri.EscapeDataString(message.BeneficiaryExternalId)}/credits", request, ct);

        response.EnsureSuccessStatusCode();
    }
}