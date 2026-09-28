using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Users.Api.Contracts;

public sealed record CreateUserRequest
(
    [Required, ExternalId] string ExternalId, 
    [ExternalId] string? PartnerExternalId
);