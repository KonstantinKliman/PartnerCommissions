using Accrual.Application.Dtos;
using Accrual.Domain;

namespace Accrual.Application.Interfaces;

public interface ISettingsService
{
    Task<SchemaDto> GetSchemaAsync(CancellationToken ct);
    Task SetSchemaAsync(SchemaType schemaType, CancellationToken ct);
}