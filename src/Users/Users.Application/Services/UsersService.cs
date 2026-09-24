using Microsoft.EntityFrameworkCore;
using Users.Application.Interfaces;
using Users.Application.Dtos;
using Users.Application.Exceptions;
using Users.Domain.Entities;

namespace Users.Application.Services;

public class UsersService(IUsersDbContext context) : IUsersService
{
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
                            .FirstOrDefaultAsync(ct)
                        ?? throw new NotFoundException($"Partner '{partnerExternalId}' not found.");
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
}