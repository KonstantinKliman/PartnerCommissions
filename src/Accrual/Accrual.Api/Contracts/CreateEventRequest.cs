using System.ComponentModel.DataAnnotations;
using Accrual.Api.Validation;

namespace Accrual.Api.Contracts;

public sealed record CreateEventRequest(
    [Required, MaxLength(128)] string ExternalId,
    [Required, MaxLength(128)] string UserExternalId,
    [Required, MaxDecimalPlaces(4)] decimal? Profit);