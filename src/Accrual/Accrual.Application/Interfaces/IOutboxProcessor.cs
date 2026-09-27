namespace Accrual.Application.Interfaces;

public interface IOutboxProcessor
{
    Task ProcessBatchAsync(CancellationToken stoppingToken);
}