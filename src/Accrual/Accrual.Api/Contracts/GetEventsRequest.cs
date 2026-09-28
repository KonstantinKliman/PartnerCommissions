using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Accrual.Api.Contracts;

public sealed record GetEventsRequest(
    [Required, ExternalId] string UserExternalId,
    [Range(1, 10_000)] int Page = 1,
    [Range(1, 100)] int PageSize = 50);