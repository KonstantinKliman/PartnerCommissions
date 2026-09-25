using System.ComponentModel.DataAnnotations;

namespace Accrual.Api.Contracts;

public sealed record GetEventsRequest(
    [Required, MaxLength(128)] string UserExternalId,
    [Range(1, 10_000)] int Page = 1,
    [Range(1, 100)] int PageSize = 50);