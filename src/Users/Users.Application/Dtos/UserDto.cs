namespace Users.Application.Dtos;

public sealed record UserDto(string ExternalId, string? PartnerExternalId, DateTimeOffset CreatedAt);