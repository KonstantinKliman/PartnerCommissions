using Accrual.Api.Contracts;
using Accrual.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Validations;

namespace Accrual.Api.Controllers;

[ApiController]
[Route("events")]
public class EventsController(IEventsService eventsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request, CancellationToken ct)
    {
        var result = await eventsService.CreateEventAsync(
            request.ExternalId, request.UserExternalId, request.Profit!.Value, ct);

        return result.IsCreated
            ? CreatedAtAction(nameof(GetEventByExternalId), new { externalId = result.Event.ExternalId }, result.Event)
            : Ok(result.Event);
    }

    [HttpGet("{externalId}")]
    public async Task<IActionResult> GetEventByExternalId([FromRoute, ExternalId] string externalId, CancellationToken ct)
    {
        var result = await eventsService.GetEventByExternalIdAsync(externalId, ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] GetEventsRequest request, CancellationToken ct)
    {
        var result = await eventsService.GetUserEventsAsync(request.UserExternalId, request.Page, request.PageSize, ct);
        return Ok(result);
    }
}