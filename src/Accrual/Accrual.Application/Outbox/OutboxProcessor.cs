using System.Text.Json;
using Accrual.Application.Interfaces;
using Accrual.Application.Metrics;
using Microsoft.Extensions.Logging;

namespace Accrual.Application.Outbox;

public class OutboxProcessor(
    IAccrualDbContext context,
    ICommissionPayoutHandler payoutHandler,
    ILogger<OutboxProcessor> logger,
    AccrualMetrics metrics) : IOutboxProcessor
{
    private const int BatchSize = 20;
    private const int MaxAttempts = 10;

    public async Task ProcessBatchAsync(CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var messages = await context.LockPendingOutboxMessagesAsync(BatchSize, MaxAttempts, ct);

        foreach (var message in messages)
        {
            try
            {
                var payout = JsonSerializer.Deserialize<CommissionPayoutMessage>(message.Payload)!;
                await payoutHandler.HandleAsync(payout, ct);

                message.ProcessedAt = DateTimeOffset.UtcNow;
                metrics.OutboxDelivered();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                message.Attempts++;
                
                metrics.OutboxFailed();
                if (message.Attempts >= MaxAttempts)
                    metrics.OutboxDeadLettered();
                
                message.LastError = ex.Message;
                message.NextAttemptAt = DateTimeOffset.UtcNow
                    .AddSeconds(Math.Min(Math.Pow(2, message.Attempts), 300));

                logger.LogWarning(ex, "Outbox message {MessageId} failed, attempt {Attempt}",
                    message.Id, message.Attempts);
            }
        }

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}