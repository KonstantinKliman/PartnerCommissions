using Accrual.Api.Contracts;
using Accrual.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Accrual.Api.Controllers;

[ApiController]
[Route("admin/schema")]
public class AdminController(ISettingsService settingsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSchema(CancellationToken ct)
    {
        var result = await settingsService.GetSchemaAsync(ct);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> SetSchema([FromBody] SetSchemaRequest request, CancellationToken ct)
    {
        await settingsService.SetSchemaAsync(request.SchemaType!.Value, ct);
        return NoContent();
    }
}