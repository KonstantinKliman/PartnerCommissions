using System.Net;
using System.Net.Http.Json;

namespace Accrual.IntegrationTests;

public class EventsTests(AccrualApiFactory factory) : IClassFixture<AccrualApiFactory>
{
    private sealed record CommissionResponse(
        string BeneficiaryExternalId, int Level, decimal Amount, string SchemaType, bool IsPaid);

    private sealed record EventResponse(string ExternalId, decimal Profit, List<CommissionResponse> Commissions);

    private readonly HttpClient _client = factory.CreateClient();

    private static string NewUserId() => $"user-{Guid.NewGuid():N}";

    private static string NewEventId() => $"evt-{Guid.NewGuid():N}";

    private async Task<HttpResponseMessage> PostEventAsync(string eventId, string userId, decimal profit)
    {
        return await _client.PostAsJsonAsync("events", new { externalId = eventId, userExternalId = userId, profit });
    }

    private async Task<EventResponse> GetEventAsync(string eventId)
    {
        return (await _client.GetFromJsonAsync<EventResponse>($"events/{eventId}"))!;
    }
        

    private async Task SetSchemaAsync(string schemaType)
    {
        var response = await _client.PutAsJsonAsync("admin/schema", new { schemaType });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_PositiveProfit_CreatesCommissionsForUpline()
    {
        await SetSchemaAsync("Linear");
        var userId = NewUserId();
        var upline = new[] { NewUserId(), NewUserId(), NewUserId() };
        factory.Users.SetUpline(userId, upline);
        var eventId = NewEventId();

        var response = await PostEventAsync(eventId, userId, 1000m);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var commissions = (await GetEventAsync(eventId)).Commissions.OrderBy(c => c.Level).ToList();
        Assert.Equal(new[] { 1, 2, 3 }, commissions.Select(c => c.Level));
        Assert.Equal(upline, commissions.Select(c => c.BeneficiaryExternalId));
        Assert.Equal(new[] { 10m, 20m, 30m }, commissions.Select(c => c.Amount));
        Assert.All(commissions, c => Assert.Equal("Linear", c.SchemaType));
        Assert.All(commissions, c => Assert.False(c.IsPaid));
    }

    [Fact]
    public async Task CreateEvent_SameEventTwice_Returns200WithoutDuplicates()
    {
        var userId = NewUserId();
        factory.Users.SetUpline(userId, NewUserId(), NewUserId());
        var eventId = NewEventId();

        var first = await PostEventAsync(eventId, userId, 1000m);
        var second = await PostEventAsync(eventId, userId, 1000m);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal(2, (await GetEventAsync(eventId)).Commissions.Count);
    }

    [Fact]
    public async Task CreateEvent_SameIdDifferentProfit_Returns409()
    {
        var userId = NewUserId();
        factory.Users.SetUpline(userId, NewUserId());
        var eventId = NewEventId();

        var first = await PostEventAsync(eventId, userId, 1000m);
        var second = await PostEventAsync(eventId, userId, 2000m);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task CreateEvent_NonPositiveProfit_NoCommissions(int profit)
    {
        var userId = NewUserId();
        factory.Users.SetUpline(userId, NewUserId(), NewUserId());
        var eventId = NewEventId();

        var response = await PostEventAsync(eventId, userId, profit);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Empty((await GetEventAsync(eventId)).Commissions);
    }

    [Fact]
    public async Task CreateEvent_UnknownUser_Returns404()
    {
        var response = await PostEventAsync(NewEventId(), NewUserId(), 1000m);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_UsersUnavailable_Returns503()
    {
        var userId = NewUserId();
        factory.Users.SetUnavailable(userId);

        var response = await PostEventAsync(NewEventId(), userId, 1000m);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_ConcurrentDuplicates_CommissionsCreatedOnce()
    {
        var userId = NewUserId();
        factory.Users.SetUpline(userId, NewUserId(), NewUserId());
        var eventId = NewEventId();

        var tasks = Enumerable.Range(0, 10).Select(_ => PostEventAsync(eventId, userId, 1000m));
        var responses = await Task.WhenAll(tasks);

        var statuses = responses.Select(r => r.StatusCode).ToList();
        Assert.Single(statuses, s => s == HttpStatusCode.Created);
        Assert.Equal(9, statuses.Count(s => s == HttpStatusCode.OK));
        Assert.Equal(2, (await GetEventAsync(eventId)).Commissions.Count);
    }
}