using Accrual.Application.Dtos;
using Accrual.Domain;

namespace Accrual.Application.Interfaces;

public interface ISchemaService
{
    Task<SchemaDto> GetCurrentAsync(CancellationToken ct);
    Task SetAsync(SchemaType schemaType, CancellationToken ct);
}