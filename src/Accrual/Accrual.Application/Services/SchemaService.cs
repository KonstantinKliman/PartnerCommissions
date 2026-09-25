using Accrual.Application.Dtos;
using Accrual.Application.Interfaces;
using Accrual.Domain;
using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accrual.Application.Services;

public class SchemaService(IAccrualDbContext context) : ISchemaService
{
    private const SchemaType DefaultSchema = SchemaType.Linear;
    
    public async Task<SchemaDto> GetCurrentAsync(CancellationToken ct)
    {
        var schemaType = await GetCurrentSchemaTypeAsync(ct);
        return new SchemaDto(schemaType);
    }

    public async Task SetAsync(SchemaType schemaType, CancellationToken ct)
    {
        var current = await GetCurrentSchemaTypeAsync(ct);
        
        if (current == schemaType)
            return;

        context.SchemaChanges.Add(new SchemaChange { SchemaType = schemaType });
        await context.SaveChangesAsync(ct);
    }

    private async Task<SchemaType> GetCurrentSchemaTypeAsync(CancellationToken ct)
    {
        var currentSchema = await context.SchemaChanges
            .OrderByDescending(s => s.ChangedAt)
            .ThenByDescending(s => s.Id)
            .Select(s => (SchemaType?)s.SchemaType)
            .FirstOrDefaultAsync(ct);
        
        return currentSchema ?? DefaultSchema;
    }
        
}