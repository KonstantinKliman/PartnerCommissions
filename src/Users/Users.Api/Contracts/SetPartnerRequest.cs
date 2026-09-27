using System.ComponentModel.DataAnnotations;
using Users.Api.Validation;

namespace Users.Api.Contracts;

public sealed record SetPartnerRequest([ExternalId]string? PartnerExternalId);