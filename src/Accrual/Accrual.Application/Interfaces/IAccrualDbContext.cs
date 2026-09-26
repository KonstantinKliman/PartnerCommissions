using Accrual.Application.Outbox;
using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Accrual.Application.Interfaces;

public interface IAccrualDbContext
{
    DbSet<Commission> Commissions { get; }
    
    DbSet<Event> Events { get; }
    
    DbSet<SchemaChange> SchemaChanges { get; }
    
    DbSet<OutboxMessage> OutboxMessages { get; }
    
    DatabaseFacade Database { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct);
    
    Task<List<OutboxMessage>> LockPendingOutboxMessagesAsync(int batchSize, int maxAttempts, CancellationToken ct);
}