using Accrual.Application.Interfaces;
using Accrual.Application.Outbox;
using Microsoft.Extensions.Options;

namespace Accrual.Api.Workers;

public class OutboxWorker(
    IServiceScopeFactory scopeFactory, 
    ILogger<OutboxWorker> logger,
    IOptions<OutboxOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(options.Value.PollingInterval);
        
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();

                    await processor.ProcessBatchAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Outbox processing failed");
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Outbox worker stopped");
        }
    }
}