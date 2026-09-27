using System.ComponentModel.DataAnnotations;
using Users.Api.Validation;
using Wallets.Api.Validation;

namespace Wallets.Api.Contracts;

public sealed record CreateCreditRequest(
    [Required] Guid? CommissionId,
    [Required, ExternalId] string EventExternalId,
    [Required, PositiveAmount] decimal? Amount);