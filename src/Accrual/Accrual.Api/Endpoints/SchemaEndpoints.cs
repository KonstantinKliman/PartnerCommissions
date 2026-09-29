using Accrual.Api.Contracts;
using Accrual.Application.Dtos;
using Accrual.Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Accrual.Api.Endpoints;

public static class SchemaEndpoints
{
    public static IEndpointRouteBuilder MapSchemaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("admin/schema");

        group.MapGet("", GetSchemaAsync);
        group.MapPut("", SetSchemaAsync);
        
        return app;
    }

    private static async Task<NoContent> SetSchemaAsync(
        [FromBody] SetSchemaRequest request, 
        ISchemaService schemaService, 
        CancellationToken ct)
    {
        await schemaService.SetAsync(request.SchemaType!.Value, ct);
        return TypedResults.NoContent();
    }

    private static async Task<Ok<SchemaDto>> GetSchemaAsync(ISchemaService schemaService, CancellationToken ct)
    {
        var result = await schemaService.GetCurrentAsync(ct);
        return TypedResults.Ok(result);
    }
}