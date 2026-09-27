using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Validation;
using Wallets.Api.Contracts;
using Wallets.Application.Interfaces;

namespace Wallets.Api.Controllers;

[ApiController]
[Route("wallets")]
public class WalletsController(IWalletsService walletsService) : ControllerBase
{
    [HttpPost("{userExternalId}/credits")]
    public async Task<IActionResult> CreateCredit(
        [FromRoute, ExternalId] string userExternalId, 
        [FromBody] CreateCreditRequest request, 
        CancellationToken ct)
    {
        var result = await walletsService.CreditAsync(
            userExternalId, request.CommissionId!.Value, request.EventExternalId, request.Amount!.Value, ct);

        return result.IsCreated
            ? StatusCode(StatusCodes.Status201Created, result.Credit)
            : Ok(result.Credit);
    }
    
    [HttpGet("{userExternalId}")]
    public async Task<IActionResult> GetBalance([FromRoute, ExternalId] string userExternalId, CancellationToken ct)
    {
        var result = await walletsService.GetBalanceAsync(userExternalId, ct);
        return Ok(result);
    }

    [HttpGet("{userExternalId}/credits")]
    public async Task<IActionResult> GetCredits(
        [FromRoute, ExternalId] string userExternalId,
        [FromQuery] GetCreditsRequest request,
        CancellationToken ct)
    {
        var result = await walletsService.GetCreditsAsync(userExternalId, request.Page, request.PageSize, ct);
        return Ok(result);
    }
}