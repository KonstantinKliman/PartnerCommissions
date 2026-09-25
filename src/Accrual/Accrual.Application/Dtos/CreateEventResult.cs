namespace Accrual.Application.Dtos;

public sealed record CreateEventResult(EventDto Event, bool IsCreated);