using System.Net;
using System.Net.Http.Json;

namespace Wallets.IntegrationTests;

public class WalletsTests(WalletsApiFactory factory) : IClassFixture<WalletsApiFactory>
{
    private sealed record BalanceResponse(decimal Balance);
    
    private readonly HttpClient _client = factory.CreateClient();

    private static string NewUserId()
    {
        return $"user-{Guid.NewGuid():N}";
    }

    private static string NewEventId()
    {
        return $"evt-{Guid.NewGuid():N}";
    }

    private static Guid NewCommissionId()
    {
        return Guid.NewGuid();
    }
    
    private async Task<HttpResponseMessage> PostCreditAsync(string userExternalId, Guid commissionId, string eventExternalId, decimal amount)
    {
        return await _client.PostAsJsonAsync($"wallets/{userExternalId}/credits", new { commissionId, eventExternalId, amount });
    }

    private async Task<decimal> GetBalanceAsync(string userExternalId)
    {
        var response = await _client.GetFromJsonAsync<BalanceResponse>($"wallets/{userExternalId}");

        return response!.Balance;
    }
    
    [Fact]
    public async Task Credit_SameCommissionTwice_CreditedOnce()
    {
        var userId = NewUserId();
        var commissionId = NewCommissionId();
        var eventId = NewEventId();
        const decimal amount = 100.5m;
        
        var first = await PostCreditAsync(userId, commissionId, eventId, amount);
        var second = await PostCreditAsync(userId, commissionId, eventId, amount);
        
        var balance = await GetBalanceAsync(userId);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal(amount, balance);
    }
    
    [Fact]
    public async Task Credit_SameCommissionDifferentAmount_Returns409()
    {
        var userId = NewUserId();
        var commissionId = NewCommissionId();
        var eventId = NewEventId();
        const decimal firstAmount = 100.5m;
        const decimal secondAmount = 120.5m;
        
        var first = await PostCreditAsync(userId, commissionId, eventId, firstAmount);
        var second = await PostCreditAsync(userId, commissionId, eventId, secondAmount);
        
        var balance = await GetBalanceAsync(userId);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        Assert.Equal(firstAmount, balance);
    }

    [Fact]
    public async Task Credit_ConcurrentSameCommission_CreditedOnce()
    {
        var userId = NewUserId();
        var commissionId = NewCommissionId();
        var eventId = NewEventId();
        const decimal amount = 100m;

        var tasks = Enumerable.Range(0, 10).Select(_ => PostCreditAsync(userId, commissionId, eventId, amount));
        var responses = await Task.WhenAll(tasks);

        var statuses = responses.Select(r => r.StatusCode).ToList();
        Assert.Single(statuses, s => s == HttpStatusCode.Created);
        Assert.Equal(9, statuses.Count(s => s == HttpStatusCode.OK));

        var balance = await GetBalanceAsync(userId);
        Assert.Equal(amount, balance);
    }

    [Fact]
    public async Task Credit_ConcurrentDifferentCommissionsForNewUser_AllCredited()
    {
        var userId = NewUserId();
        const decimal amount = 100m;
        
        var tasks = Enumerable.Range(0, 10).Select(_ => PostCreditAsync(userId, NewCommissionId(), NewEventId(), amount));
        var responses = await Task.WhenAll(tasks);

        var statuses = responses.Select(r => r.StatusCode).ToList();
        Assert.All(statuses, s => Assert.Equal(HttpStatusCode.Created, s));

        var balance = await GetBalanceAsync(userId);
        Assert.Equal(amount * 10, balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task Credit_NonPositiveAmount_Returns400(int amount)
    {
        var userId = NewUserId();
        var commissionId = NewCommissionId();
        var eventId = NewEventId();
        
        var result = await PostCreditAsync(userId, commissionId, eventId, amount);
        
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}