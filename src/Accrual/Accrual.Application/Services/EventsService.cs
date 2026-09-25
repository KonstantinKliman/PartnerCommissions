using Accrual.Application.Dtos;
using Accrual.Application.Exceptions;
using Accrual.Application.Interfaces;
using Accrual.Application.Mappings;
using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accrual.Application.Services;

public class EventsService(IAccrualDbContext context, IUsersClient usersClient, ISchemaService schemaService)
    : IEventsService
{
    public async Task<CreateEventResult> CreateEventAsync(
        string externalId, string userExternalId, decimal profit, CancellationToken ct)
    {
        var existingEvent = await context.Events
            .AsNoTracking()
            .Include(e => e.Commissions)
            .FirstOrDefaultAsync(e => e.ExternalId == externalId, ct);

        if (existingEvent is not null)
        {
            ThrowIfDataDifferent(existingEvent, userExternalId, profit);
            
            return new CreateEventResult(existingEvent.ToDto(), IsCreated: false);
        }

        var upline = await usersClient.GetUplineAsync(userExternalId, ct);
        if (upline is null)
            throw new NotFoundException($"User '{userExternalId}' not found.");

        var schema = await schemaService.GetCurrentAsync(ct);
        var newEvent = Event.Create(externalId, userExternalId, profit, upline, schema.SchemaType);

        context.Events.Add(newEvent);
        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (ConflictException)
        {
            var eventItem = await context.Events
                .AsNoTracking()
                .Include(e => e.Commissions)
                .FirstOrDefaultAsync(e => e.ExternalId == externalId, ct);
            
            if (eventItem is null)
                throw;
            
            ThrowIfDataDifferent(eventItem, userExternalId, profit);

            return new CreateEventResult(eventItem.ToDto(), IsCreated: false);
        }

        return new CreateEventResult(newEvent.ToDto(), IsCreated: true);
    }

    public async Task<EventDto> GetEventByExternalIdAsync(string externalId, CancellationToken ct)
    {
        var eventItem = await context.Events
            .AsNoTracking()
            .Include(e => e.Commissions)
            .FirstOrDefaultAsync(e => e.ExternalId == externalId, ct);

        return eventItem?.ToDto() ?? throw new NotFoundException($"Event '{externalId}' not found.");
    }

    public async Task<List<EventSummaryDto>> GetUserEventsAsync(string userExternalId, int page, int pageSize, CancellationToken ct)
    {
        return await context.Events
            .Where(e => e.UserExternalId == userExternalId)
            .OrderByDescending(e => e.CreatedAt)
            .ThenByDescending(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EventSummaryDto(e.ExternalId, e.Profit, e.CreatedAt))
            .ToListAsync(ct);
    }

    private static void ThrowIfDataDifferent(Event eventItem, string userExternalId, decimal profit)
    {
        if (eventItem.UserExternalId != userExternalId || eventItem.Profit != profit)
            throw new ConflictException($"Event '{eventItem.ExternalId}' already exists with different data.");
    }
}