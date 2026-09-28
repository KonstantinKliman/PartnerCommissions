using System.Collections.Concurrent;
using Accrual.Application.Exceptions;
using Accrual.Application.Interfaces;

namespace Accrual.IntegrationTests.Fakes;

public class FakeUsersClient : IUsersClient
{
    private readonly ConcurrentDictionary<string, List<string>> _uplines = new();
    private readonly ConcurrentDictionary<string, bool> _unavailable = new();

    public void SetUpline(string userExternalId, params string[] upline)
    {
        _uplines[userExternalId] = upline.ToList();
    }
    
    public void SetUnavailable(string userExternalId)
    {
        _unavailable[userExternalId] = true;
    }
    
    public Task<List<string>?> GetUplineAsync(string userExternalId, CancellationToken ct)
    {
        if (_unavailable.ContainsKey(userExternalId))
            throw new ServiceNotAvailableException("Users service is unavailable.", new HttpRequestException());

        _uplines.TryGetValue(userExternalId, out var upline);
        return Task.FromResult<List<string>?>(upline);
    }
}