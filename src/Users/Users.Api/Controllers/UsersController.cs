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
}