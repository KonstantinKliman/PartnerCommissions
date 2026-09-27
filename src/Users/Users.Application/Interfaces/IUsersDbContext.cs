using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Users.Application.Dtos;
using Users.Domain.Entities;

namespace Users.Application.Interfaces;

public interface IUsersDbContext
{
    DbSet<User> Users { get; }
    
    DatabaseFacade Database { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct);

    Task LockTreeAsync(CancellationToken ct);
    
    Task<List<TreeNodeDto>> QueryDownlineAsync(Guid userId, int maxDepth, int maxNodes, CancellationToken ct);
    
    Task<List<TreeNodeDto>> QueryUplineAsync(Guid userId, int maxDepth, CancellationToken ct);
}