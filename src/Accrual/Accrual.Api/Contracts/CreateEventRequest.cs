using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Accrual.Api.Contracts;

public sealed record CreateEventRequest(
    [Required, ExternalId] string ExternalId,
    [Required, ExternalId] string UserExternalId,
    [Required, MoneyAmount] decimal? Profit);