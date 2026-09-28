using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Wallets.Api.Contracts;

public sealed record CreateCreditRequest(
    [Required] Guid? CommissionId,
    [Required, ExternalId] string EventExternalId,
    [Required, MoneyAmount(MustBePositive = true)] decimal? Amount);