using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Accrual.Application.Exceptions;
using Accrual.Application.Interfaces;
using Accrual.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Accrual.IntegrationTests;

public class OutboxTests(AccrualApiFactory factory) : IClassFixture<AccrualApiFactory>
{
    private sealed record CommissionResponse(bool IsPaid);

    private sealed record EventResponse(List<CommissionResponse> Commissions);

    private readonly HttpClient _client = factory.CreateClient();

    private static string NewUserId() => $"user-{Guid.NewGuid():N}";

    private async Task<string> CreateEventAsync(params string[] upline)
    {
        var userId = NewUserId();
        factory.Users.SetUpline(userId, upline);
        var eventId = $"evt-{Guid.NewGuid():N}";

        var response = await _client.PostAsJsonAsync("events", new { externalId = eventId, userExternalId = userId, profit = 1000m });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return eventId;
    }

    private async Task<List<CommissionResponse>> GetCommissionsAsync(string eventId)
    {
        var response = await _client.GetFromJsonAsync<EventResponse>($"events/{eventId}");
        return response!.Commissions;
    }
        

    private async Task<OutboxMessage> GetOutboxMessageAsync(string eventId)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<IAccrualDbContext>();

        var filter = JsonSerializer.Serialize(new { EventExternalId = eventId });
        return await db.OutboxMessages
            .AsNoTracking()
            .SingleAsync(m => EF.Functions.JsonContains(m.Payload, filter));
    }

    [Fact]
    public async Task Outbox_Success_MarksCommissionsPaid()
    {
        var eventId = await CreateEventAsync(NewUserId(), NewUserId());

        await factory.RunOutboxAsync();

        Assert.Equal(2, factory.Wallets.AttemptsFor(eventId).Count);
        Assert.All(await GetCommissionsAsync(eventId), c => Assert.True(c.IsPaid));
    }

    [Fact]
    public async Task Outbox_TransientFailure_RetriedLaterWithBackoff()
    {
        var eventId = await CreateEventAsync(NewUserId());
        factory.Wallets.FailFor(eventId, new HttpRequestException("Wallets is down"));

        await factory.RunOutboxAsync();
        await factory.RunOutboxAsync();

        Assert.Single(factory.Wallets.AttemptsFor(eventId));
        Assert.All(await GetCommissionsAsync(eventId), c => Assert.False(c.IsPaid));

        var message = await GetOutboxMessageAsync(eventId);
        Assert.Equal(1, message.Attempts);
        Assert.True(message.NextAttemptAt > DateTimeOffset.UtcNow);
        Assert.Null(message.DeadLetteredAt);
    }

    [Fact]
    public async Task Outbox_PermanentFailure_DeadLettered()
    {
        var eventId = await CreateEventAsync(NewUserId());
        factory.Wallets.FailFor(eventId, new PermanentDeliveryException("Wallets rejected the credit"));

        await factory.RunOutboxAsync();
        await factory.RunOutboxAsync();

        Assert.Single(factory.Wallets.AttemptsFor(eventId));
        Assert.All(await GetCommissionsAsync(eventId), c => Assert.False(c.IsPaid));

        var message = await GetOutboxMessageAsync(eventId);
        Assert.NotNull(message.DeadLetteredAt);
        Assert.Null(message.ProcessedAt);
    }
}