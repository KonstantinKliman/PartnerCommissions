using Accrual.Application.Exceptions;
using Accrual.Application.Interfaces;
using Accrual.Application.Outbox;
using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Accrual.Infrastructure.Persistence;

public class AccrualDbContext(DbContextOptions<AccrualDbContext> options) : DbContext(options), IAccrualDbContext
{
    public DbSet<Commission> Commissions => Set<Commission>();
    
    public DbSet<Event> Events => Set<Event>();
    
    public DbSet<SchemaChange> SchemaChanges => Set<SchemaChange>();
    
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    
    public async Task<List<OutboxMessage>> LockPendingOutboxMessagesAsync(int batchSize, CancellationToken ct)
    {
        FormattableString query = $"""
                     SELECT * FROM outbox_messages
                     WHERE processed_at IS NULL
                       AND dead_lettered_at IS NULL
                       AND next_attempt_at <= now()
                     ORDER BY next_attempt_at
                     LIMIT {batchSize}
                     FOR UPDATE SKIP LOCKED
                     """;
        
        return await OutboxMessages
            .FromSql(query)
            .ToListAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccrualDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("Entity already exists.", ex);
        }
    }
}