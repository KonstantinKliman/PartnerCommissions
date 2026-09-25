namespace Users.Application.Dtos;

public sealed record TreeNodeDto(string ExternalId, string? PartnerExternalId, int Level);