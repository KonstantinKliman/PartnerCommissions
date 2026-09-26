using System.ComponentModel.DataAnnotations;
using Wallets.Api.Validation;

namespace Wallets.Api.Contracts;

public sealed record CreateCreditRequest(
    [Required] Guid? CommissionId,
    [Required, MaxLength(128)] string EventExternalId,
    [Required, PositiveAmount] decimal? Amount);