using System.ComponentModel.DataAnnotations;
using Users.Api.Validation;

namespace Users.Api.Contracts;

public sealed record CreateUserRequest
(
    [Required, ExternalId] string ExternalId, 
    [ExternalId] string? PartnerExternalId
);