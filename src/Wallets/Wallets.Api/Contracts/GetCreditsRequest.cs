using System.ComponentModel.DataAnnotations;

namespace Wallets.Api.Contracts;

public sealed record GetCreditsRequest(
    [Range(1, 10_000)] int Page = 1,
    [Range(1, 100)] int PageSize = 50);