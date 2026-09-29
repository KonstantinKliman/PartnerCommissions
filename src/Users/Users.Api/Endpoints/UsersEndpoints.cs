using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Validations;
using Users.Api.Contracts;
using Users.Application.Dtos;
using Users.Application.Interfaces;

namespace Users.Api.Endpoints;

public static class UsersEndpoints
{
    private const string GetUserRoute = "GetUser";

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("users");

        group.MapPost("", CreateAsync);
        group.MapGet("{externalId}", GetByExternalIdAsync).WithName(GetUserRoute);
        group.MapPut("{externalId}/partner", SetPartnerAsync);
        group.MapGet("{externalId}/downline", GetDownlineAsync);
        group.MapGet("{externalId}/upline", GetUplineAsync);
        
        return app;
    }

    private static async Task<Ok<List<TreeNodeDto>>> GetUplineAsync(
        [FromRoute, ExternalId] string externalId, 
        IUsersService usersService, 
        CancellationToken ct)
    {
        var result = await usersService.GetUplineAsync(externalId, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<TreeNodeDto>>> GetDownlineAsync(
        [FromRoute, ExternalId] string externalId, 
        IUsersService usersService, 
        CancellationToken ct)
    {
        var result = await usersService.GetDownlineAsync(externalId, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<NoContent> SetPartnerAsync(
        [FromRoute, ExternalId] string externalId, 
        [FromBody] SetPartnerRequest request,
        IUsersService usersService, 
        CancellationToken ct)
    {
        await usersService.SetPartnerAsync(externalId, request.PartnerExternalId, ct);
        return TypedResults.NoContent();
    }

    private static async Task<Ok<UserDto>> GetByExternalIdAsync(
        [FromRoute, ExternalId] string externalId, 
        IUsersService usersService, 
        CancellationToken ct)
    {
        var result = await usersService.GetByExternalIdAsync(externalId, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<CreatedAtRoute<UserDto>> CreateAsync(
        CreateUserRequest request, 
        IUsersService usersService, 
        CancellationToken ct)
    {
        var result = await usersService.CreateAsync(request.ExternalId, request.PartnerExternalId, ct);
        
        return TypedResults.CreatedAtRoute(result, GetUserRoute, new { externalId = result.ExternalId });
    }
}