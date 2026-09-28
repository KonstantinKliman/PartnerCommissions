using System.Net;
using System.Net.Http.Json;
using Accrual.Application.Exceptions;
using Accrual.Application.Interfaces;
using Accrual.Application.Outbox;
using Shared.Exceptions;

namespace Accrual.Infrastructure.Clients;

public class WalletsClient(HttpClient httpClient) : IWalletsClient
{
    public async Task CreditAsync(CommissionPayoutMessage message, CancellationToken ct)
    {
        var request = new CreditRequest(message.CommissionId, message.EventExternalId, message.Amount);

        using var response = await httpClient.PostAsJsonAsync(
            $"wallets/{Uri.EscapeDataString(message.BeneficiaryExternalId)}/credits", request, ct);

        if ((int)response.StatusCode is >= 400 and < 500
            && response.StatusCode is not HttpStatusCode.RequestTimeout
            && response.StatusCode is not HttpStatusCode.TooManyRequests)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new PermanentDeliveryException(
                $"Wallets rejected the credit with {(int)response.StatusCode}: {body}");
        }
        
        response.EnsureSuccessStatusCode();
    }
}