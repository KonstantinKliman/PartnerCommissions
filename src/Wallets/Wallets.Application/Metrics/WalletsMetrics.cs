using System.Diagnostics.Metrics;

namespace Wallets.Application.Metrics;

public sealed class WalletsMetrics
{
    public const string MeterName = "Wallets";

    private readonly Counter<long> _credits;
    private readonly Counter<double> _creditsAmount;

    public WalletsMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _credits = meter.CreateCounter<long>("wallets.credits", "{credit}");
        _creditsAmount = meter.CreateCounter<double>("wallets.credits.amount");
    }

    public void CreditCreated(decimal amount)
    {
        _credits.Add(1, new KeyValuePair<string, object?>("result", "created"));
        _creditsAmount.Add((double)amount);
    }

    public void CreditDuplicate() =>
        _credits.Add(1, new KeyValuePair<string, object?>("result", "duplicate"));
}