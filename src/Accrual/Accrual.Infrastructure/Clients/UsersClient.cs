using System.Net;
using System.Net.Http.Json;
using Accrual.Application.Exceptions;
using Accrual.Application.Interfaces;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Shared.Exceptions;

namespace Accrual.Infrastructure.Clients;

public class UsersClient(HttpClient httpClient) : IUsersClient
{
    public async Task<List<string>?> GetUplineAsync(string userExternalId, CancellationToken ct)
    {
        try
        {
            using var response = await httpClient.GetAsync($"users/{Uri.EscapeDataString(userExternalId)}/upline", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var nodes = await response.Content.ReadFromJsonAsync<List<UplineNodeResponse>>(ct) ?? [];

            return nodes
                .OrderBy(n => n.Level)
                .Select(n => n.ExternalId)
                .ToList();
        }
        catch (Exception e) when (e is HttpRequestException or TimeoutRejectedException or BrokenCircuitException)
        {
            throw new ServiceNotAvailableException("Users service is unavailable.", e);
        }
    }
}