using System.Diagnostics.Metrics;
using Accrual.Domain;

namespace Accrual.Application.Metrics;

public sealed class AccrualMetrics
{
    public const string MeterName = "Accrual";

    private readonly Counter<long> _eventsReceived;
    private readonly Counter<long> _commissionsCreated;
    private readonly Counter<double> _commissionsAmount;
    private readonly Counter<long> _outboxDelivered;
    private readonly Counter<long> _outboxFailed;
    private readonly Counter<long> _outboxDeadLettered;

    public AccrualMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _eventsReceived = meter.CreateCounter<long>("accrual.events.received", "{event}");
        _commissionsCreated = meter.CreateCounter<long>("accrual.commissions.created", "{commission}");
        _commissionsAmount = meter.CreateCounter<double>("accrual.commissions.amount");
        _outboxDelivered = meter.CreateCounter<long>("accrual.outbox.delivered", "{message}");
        _outboxFailed = meter.CreateCounter<long>("accrual.outbox.failed", "{attempt}");
        _outboxDeadLettered = meter.CreateCounter<long>("accrual.outbox.dead_lettered", "{message}");
    }

    public void EventReceived(bool isCreated)
    {
        _eventsReceived.Add(1, new KeyValuePair<string, object?>("result", isCreated ? "created" : "duplicate"));
    }
    
    public void CommissionCreated(SchemaType schema, decimal amount)
    {
        var tag = new KeyValuePair<string, object?>("schema", schema.ToString());
        _commissionsCreated.Add(1, tag);
        _commissionsAmount.Add((double)amount, tag);
    }

    public void OutboxDelivered()
    {
        _outboxDelivered.Add(1); 
    }

    public void OutboxFailed()
    {
        _outboxFailed.Add(1);
    }

    public void OutboxDeadLettered()
    {
        _outboxDeadLettered.Add(1);
    } 
}