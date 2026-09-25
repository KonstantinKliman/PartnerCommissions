using Accrual.Application.Dtos;

namespace Accrual.Application.Interfaces;

public interface IEventsService
{
    Task<CreateEventResult> CreateEventAsync(string externalId, string userExternalId, decimal profit, CancellationToken ct);
    Task<EventDto> GetEventByExternalIdAsync(string externalId, CancellationToken ct);
}