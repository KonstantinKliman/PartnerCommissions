using System.Text.Json;
using Accrual.Application.Exceptions;
using Accrual.Application.Interfaces;
using Accrual.Application.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Accrual.Application.Outbox;

public class OutboxProcessor(
    IAccrualDbContext context,
    ICommissionPayoutHandler payoutHandler,
    ILogger<OutboxProcessor> logger,
    AccrualMetrics metrics,
    IOptions<OutboxOptions> options) : IOutboxProcessor
{
    public async Task ProcessBatchAsync(CancellationToken  stoppingToken)
    {
        var ct = CancellationToken.None;
        
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var messages = await context.LockPendingOutboxMessagesAsync(options.Value.BatchSize, ct);

        var processed = 0;
        foreach (var message in messages)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Outbox batch stopped on shutdown: {Processed} of {Total} messages processed",
                    processed, messages.Count);
                break;
            }
            
            try
            {
                var payout = JsonSerializer.Deserialize<CommissionPayoutMessage>(message.Payload);
                if (payout is null)
                    throw new PermanentDeliveryException("Outbox message payload is empty.");

                await payoutHandler.HandleAsync(payout, ct);

                message.ProcessedAt = DateTimeOffset.UtcNow;
                metrics.OutboxDelivered();
            }
            catch (Exception ex) when (ex is PermanentDeliveryException or JsonException)
            {
                message.Attempts++;
                message.LastError = ex.Message;
                message.DeadLetteredAt = DateTimeOffset.UtcNow;

                metrics.OutboxDeadLettered();
                logger.LogError(ex, "Outbox message {MessageId} dead-lettered after a permanent failure", message.Id);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                message.Attempts++;
                message.LastError = ex.Message;

                var delaySeconds = Math.Min(Math.Pow(2, message.Attempts), options.Value.MaxRetryDelay.TotalSeconds);
                
                message.NextAttemptAt = DateTimeOffset.UtcNow
                    .AddSeconds(delaySeconds);

                metrics.OutboxFailed();
                logger.LogWarning(ex, "Outbox message {MessageId} failed, attempt {Attempt}, will retry",
                    message.Id, message.Attempts);
            }

            processed++;
        }

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}