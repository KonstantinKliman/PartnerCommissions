using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Accrual.Application.Interfaces;

public interface IAccrualDbContext
{
    DbSet<Commission> Commissions { get; }
    
    DbSet<Event> Events { get; }
    
    DbSet<AccrualSettings> Settings { get; }
    
    DatabaseFacade Database { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct);
}