using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Users.Domain.Entities;

namespace Users.Application.Interfaces;

public interface IUsersDbContext
{
    DbSet<User> Users { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}