using System.ComponentModel.DataAnnotations;

namespace Users.Api.Contracts;

public sealed record SetPartnerRequest([MaxLength(128)]string? PartnerExternalId);