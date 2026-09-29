using Accrual.Api.Contracts;
using Accrual.Application.Dtos;
using Accrual.Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Validations;

namespace Accrual.Api.Endpoints;

public static class EventsEndpoints
{
    private const string GetEventRoute = "GetEvent";
    
    public static IEndpointRouteBuilder MapEventsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("events");

        group.MapPost("", CreateEventAsync);
        group.MapGet("{externalId}", GetEventByExternalIdAsync).WithName(GetEventRoute);
        group.MapGet("", GetEventsAsync);
        
        return app;
    }

    private static async Task<Ok<List<EventSummaryDto>>> GetEventsAsync(
        [AsParameters] GetEventsRequest request,  
        IEventsService eventsService, 
        CancellationToken ct)
    {
        var result = await eventsService.GetUserEventsAsync(request.UserExternalId, request.Page, request.PageSize, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<EventDto>> GetEventByExternalIdAsync(
        [FromRoute, ExternalId] string externalId, 
        IEventsService eventsService, 
        CancellationToken ct)
    {
        var result = await eventsService.GetEventByExternalIdAsync(externalId, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<CreatedAtRoute<EventDto>, Ok<EventDto>>> CreateEventAsync(
        [FromBody] CreateEventRequest request, 
        IEventsService eventsService, 
        CancellationToken ct)
    {
        var result = await eventsService.CreateEventAsync(
            request.ExternalId, request.UserExternalId, request.Profit!.Value, ct);

        var eventDto = result.Event;
        
        return result.IsCreated
            ? TypedResults.CreatedAtRoute(eventDto, GetEventRoute, new { externalId = result.Event.ExternalId })
            : TypedResults.Ok(eventDto);
    }
}