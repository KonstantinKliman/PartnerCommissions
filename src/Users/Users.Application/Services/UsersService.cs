using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using Users.Application.Interfaces;
using Users.Application.Dtos;
using Users.Domain.Entities;
using Users.Domain.Exceptions;

namespace Users.Application.Services;

public class UsersService(IUsersDbContext context) : IUsersService
{
    private const int MaxTreeDepth = 10;
    private const int MaxDownlineNodes = 1000;
    
    public async Task<UserDto> CreateAsync(string externalId, string? partnerExternalId, CancellationToken ct)
    {
        var userExists = await context.Users
            .AnyAsync(u => u.ExternalId == externalId, ct);

        if (userExists)
            throw new ConflictException($"User '{externalId}' already exists.");

        Guid? partnerId = null;
        if (partnerExternalId is not null)
        {
            partnerId = await context.Users
                .Where(u => u.ExternalId == partnerExternalId)
                .Select(u => (Guid?)u.Id)
                .FirstOrDefaultAsync(ct);
            
            if (partnerId is null)
                throw new NotFoundException($"Partner '{partnerExternalId}' not found.");
        }

        var user = new User
        {
            ExternalId = externalId,
            PartnerId = partnerId
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(ct);

        return new UserDto(user.ExternalId, partnerExternalId, user.CreatedAt);
    }

    public async Task<UserDto> GetByExternalIdAsync(string externalId, CancellationToken ct)
    {
        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.ExternalId == externalId)
            .FirstOrDefaultAsync(ct);

        if (user is null)
            throw new NotFoundException($"User '{externalId}' not found.");

        string? partnerExternalId = null;
        if (user.PartnerId is not null)
        {
            partnerExternalId = await context.Users
                .Where(p => p.Id == user.PartnerId)
                .Select(p => p.ExternalId)
                .FirstOrDefaultAsync(ct);
        }

        return new UserDto(user.ExternalId, partnerExternalId, user.CreatedAt);
    }

    public async Task SetPartnerAsync(string externalId, string? partnerExternalId, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        await context.LockTreeAsync(ct);

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.ExternalId == externalId, ct);
        
        if (user is null)
            throw new NotFoundException($"User '{externalId}' not found.");

        User? partner = null;
        if (partnerExternalId is not null)
        {
            partner = await context.Users
                .FirstOrDefaultAsync(u => u.ExternalId == partnerExternalId, ct);

            if (partner is null)
                throw new NotFoundException($"Partner '{partnerExternalId}' not found.");
        }

        if (partner is not null)
        {
            Guid? ancestorId = partner.Id;
            while (ancestorId is not null)
            {
                if (ancestorId == user.Id)
                    throw new DomainException("Setting this partner would create a cycle.");

                var currentId = ancestorId;
                ancestorId = await context.Users
                    .Where(u => u.Id == currentId)
                    .Select(u => u.PartnerId)
                    .FirstOrDefaultAsync(ct);
            }
        }

        user.PartnerId = partner?.Id;
        
        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    public async Task<List<TreeNodeDto>> GetDownlineAsync(string externalId, CancellationToken ct)
    {
        var userId = await context.Users
            .Where(u => u.ExternalId == externalId)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(ct);

        if (userId is null)
            throw new NotFoundException($"User '{externalId}' not found.");

        return await context.QueryDownlineAsync(userId.Value, MaxTreeDepth, MaxDownlineNodes, ct);
    }

    public async Task<List<TreeNodeDto>> GetUplineAsync(string externalId, CancellationToken ct)
    {
        var userId = await context.Users
            .Where(u => u.ExternalId == externalId)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(ct);
        
        if (userId is null)
            throw new NotFoundException($"User '{externalId}' not found.");
        
        return await context.QueryUplineAsync(userId.Value, MaxTreeDepth, ct);
    }
}