using Microsoft.EntityFrameworkCore;
using Npgsql;
using Users.Application.Dtos;
using Users.Application.Exceptions;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Infrastructure.Persistence;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options), IUsersDbContext
{
    public DbSet<User> Users => Set<User>();

    private const long TreeLockKey = 1_001;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);

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

    public Task LockTreeAsync(CancellationToken ct)
    {
        return Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock({TreeLockKey})", ct);
    }

    public async Task<List<TreeNodeDto>> QueryDownlineAsync(Guid userId, int maxDepth, CancellationToken ct)
    {
        FormattableString query = $"""
                                   WITH RECURSIVE downline AS (
                                      SELECT c.id, c.external_id, p.external_id AS partner_external_id, 1 AS level
                                      FROM users c
                                      JOIN users p ON p.id = c.partner_id
                                      WHERE p.id = {userId}

                                      UNION ALL

                                      SELECT c.id, c.external_id, d.external_id, d.level + 1
                                      FROM users c
                                      JOIN downline d ON c.partner_id = d.id
                                      WHERE d.level < {maxDepth}
                                   )
                                   SELECT external_id, partner_external_id, level
                                   FROM downline
                                   ORDER BY level, external_id
                                   """;

        return await Database.SqlQuery<TreeNodeDto>(query).ToListAsync(ct);
    }

    public async Task<List<TreeNodeDto>> QueryUplineAsync(Guid userId, int maxDepth, CancellationToken ct)
    {
        FormattableString query = $"""
                                    WITH RECURSIVE upline AS (
                                        SELECT p.id, p.external_id, p.partner_id, 1 AS level
                                        FROM users u
                                        JOIN users p ON p.id = u.partner_id
                                        WHERE u.id = {userId}
                                    
                                        UNION ALL
                                    
                                        SELECT p.id, p.external_id, p.partner_id, up.level + 1
                                        FROM users p
                                        JOIN upline up ON p.id = up.partner_id
                                        WHERE up.level < {maxDepth}
                                    )
                                    SELECT up.external_id, pp.external_id AS partner_external_id, up.level
                                    FROM upline up
                                    LEFT JOIN users pp ON pp.id = up.partner_id
                                    ORDER BY up.level
                                    """;

        return await Database.SqlQuery<TreeNodeDto>(query).ToListAsync(ct);
    }
}