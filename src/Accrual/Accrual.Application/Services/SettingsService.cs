using Accrual.Application.Dtos;
using Accrual.Application.Interfaces;
using Accrual.Domain;
using Microsoft.EntityFrameworkCore;

namespace Accrual.Application.Services;

public class SettingsService(IAccrualDbContext context) : ISettingsService
{
    private const int SettingsId = 1;

    public async Task<SchemaDto> GetSchemaAsync(CancellationToken ct)
    {
        var schemaType = await context.Settings
            .Where(s => s.Id == SettingsId)
            .Select(s => s.SchemaType)
            .SingleAsync(ct);

        return new SchemaDto(schemaType);
    }

    public async Task SetSchemaAsync(SchemaType schemaType, CancellationToken ct)
    {
        var settings = await context.Settings
            .SingleAsync(s => s.Id == SettingsId, ct);

        settings.SchemaType = schemaType;
        await context.SaveChangesAsync(ct);
    }
}