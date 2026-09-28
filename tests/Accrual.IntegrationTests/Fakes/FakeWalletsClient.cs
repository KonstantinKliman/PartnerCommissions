using System.Collections.Concurrent;
using Accrual.Application.Interfaces;
using Accrual.Application.Outbox;

namespace Accrual.IntegrationTests.Fakes;

public class FakeWalletsClient : IWalletsClient
{
    private readonly ConcurrentQueue<CommissionPayoutMessage> _attempts = new();
    private readonly ConcurrentDictionary<string, Exception> _failures = new();

    public void FailFor(string eventExternalId, Exception exception)
    {
        _failures[eventExternalId] = exception;
    }

    public List<CommissionPayoutMessage> AttemptsFor(string eventExternalId)
    {
        return _attempts.Where(m => m.EventExternalId == eventExternalId).ToList();
    }
    
    public Task CreditAsync(CommissionPayoutMessage message, CancellationToken ct)
    {
        _attempts.Enqueue(message);

        if (_failures.TryGetValue(message.EventExternalId, out var exception))
            throw exception;

        return Task.CompletedTask;
    }
}