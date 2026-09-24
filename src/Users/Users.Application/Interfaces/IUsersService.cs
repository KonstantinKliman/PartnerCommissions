using Users.Application.Dtos;

namespace Users.Application.Interfaces;

public interface IUsersService
{
    Task<UserDto> CreateAsync(string externalId, string? partnerExternalId, CancellationToken ct);

    Task<UserDto> GetByExternalIdAsync(string externalId, CancellationToken ct);
}