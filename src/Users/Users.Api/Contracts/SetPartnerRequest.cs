using Shared.Validations;

namespace Users.Api.Contracts;

public sealed record SetPartnerRequest([ExternalId]string? PartnerExternalId);