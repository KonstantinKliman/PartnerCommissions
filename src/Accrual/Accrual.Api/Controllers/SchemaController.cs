using Accrual.Api.Contracts;
using Accrual.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Accrual.Api.Controllers;

[ApiController]
[Route("admin/schema")]
public class SchemaController(ISchemaService schemaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSchema(CancellationToken ct)
    {
        var result = await schemaService.GetCurrentAsync(ct);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> SetSchema([FromBody] SetSchemaRequest request, CancellationToken ct)
    {
        await schemaService.SetAsync(request.SchemaType!.Value, ct);
        return NoContent();
    }
}