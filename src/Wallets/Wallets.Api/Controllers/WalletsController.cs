using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Wallets.Api.Contracts;
using Wallets.Application.Interfaces;

namespace Wallets.Api.Controllers;

[ApiController]
[Route("wallets")]
public class WalletsController(IWalletsService walletsService) : ControllerBase
{
    [HttpPost("{userExternalId}/credits")]
    public async Task<IActionResult> CreateCredit([FromRoute, MaxLength(128)] string userExternalId, [FromBody] CreateCreditRequest request, CancellationToken ct)
    {
        var result = await walletsService.CreditAsync(
            userExternalId, request.CommissionId!.Value, request.EventExternalId, request.Amount!.Value, ct);

        return result.IsCreated
            ? StatusCode(StatusCodes.Status201Created, result.Credit)
            : Ok(result.Credit);
    }
}