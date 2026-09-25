using Microsoft.AspNetCore.Mvc;
using Users.Api.Contracts;
using Users.Application.Interfaces;

namespace Users.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController(IUsersService usersService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var result = await usersService.CreateAsync(request.ExternalId, request.PartnerExternalId, ct);
        return CreatedAtAction(nameof(GetByExternalId), new { externalId = result.ExternalId }, result);
    }

    [HttpGet("{externalId}")]
    public async Task<IActionResult> GetByExternalId([FromRoute] string externalId, CancellationToken ct)
    {
        var result = await usersService.GetByExternalIdAsync(externalId, ct);
        return Ok(result);
    }

    [HttpPut("{externalId}/partner")]
    public async Task<IActionResult> SetPartner([FromRoute] string externalId, [FromBody] SetPartnerRequest request,
        CancellationToken ct)
    {
        await usersService.SetPartnerAsync(externalId, request.PartnerExternalId, ct);
        return NoContent();
    }

    [HttpGet("{externalId}/downline")]
    public async Task<IActionResult> GetDownline([FromRoute] string externalId, CancellationToken ct)
    {
        var result = await usersService.GetDownlineAsync(externalId, ct);
        return Ok(result);
    }

    [HttpGet("{externalId}/upline")]
    public async Task<IActionResult> GetUpline([FromRoute] string externalId, CancellationToken ct)
    {
        var result = await usersService.GetUplineAsync(externalId, ct);
        return Ok(result);
    }
}