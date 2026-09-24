using System.ComponentModel.DataAnnotations;

namespace Users.Api.Contracts;

public sealed record CreateUserRequest
(
    [Required, MaxLength(128)] string ExternalId, 
    [MaxLength(128)] string? PartnerExternalId
);