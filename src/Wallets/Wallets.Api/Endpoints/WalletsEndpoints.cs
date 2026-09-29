using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Validations;
using Wallets.Api.Contracts;
using Wallets.Application.Dtos;
using Wallets.Application.Interfaces;

namespace Wallets.Api.Endpoints;

public static class WalletsEndpoints
{
    public static IEndpointRouteBuilder MapWalletsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("wallets");
        
        group.MapPost("{userExternalId}/credits", CreateCreditAsync);
        group.MapGet("{userExternalId}", GetBalanceAsync);
        group.MapGet("{userExternalId}/credits", GetCreditsAsync);
        
        return app;
    }

    private static async Task<Ok<List<CreditDto>>> GetCreditsAsync(
        [FromRoute, ExternalId] string userExternalId,
        [AsParameters] GetCreditsRequest request,
        IWalletsService walletsService,
        CancellationToken ct)
    {
        var result = await walletsService.GetCreditsAsync(userExternalId, request.Page, request.PageSize, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<BalanceDto>> GetBalanceAsync(
        [FromRoute, ExternalId] string userExternalId,
        IWalletsService walletsService,
        CancellationToken ct)
    {
        var result = await walletsService.GetBalanceAsync(userExternalId, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created<CreditDto>, Ok<CreditDto>>> CreateCreditAsync(
        [FromRoute, ExternalId] string userExternalId, 
        [FromBody] CreateCreditRequest request, 
        IWalletsService walletsService,
        CancellationToken ct)
    {
        var result = await walletsService.CreditAsync(
            userExternalId, request.CommissionId!.Value, request.EventExternalId, request.Amount!.Value, ct);

        return result.IsCreated
            ? TypedResults.Created((string?)null, result.Credit)
            : TypedResults.Ok(result.Credit);
    }
}