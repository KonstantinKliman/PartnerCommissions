using System.Net;
using System.Net.Http.Json;

namespace Users.IntegrationTests;

public class UsersTests(UsersApiFactory factory) : IClassFixture<UsersApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    
    private static string NewId() => $"user-{Guid.NewGuid():N}";
    
    private async Task<HttpResponseMessage> PostUserAsync(string externalId, string? partnerExternalId = null)
    {
        return await _client.PostAsJsonAsync("users", new { externalId, partnerExternalId });
    }

    private async Task<HttpResponseMessage> SetPartnerAsync(string externalId, string? partnerExternalId)
    {
        return await _client.PutAsJsonAsync($"users/{externalId}/partner", new { partnerExternalId });
    }
    
    private async Task<string> CreateUserAsync(string? partnerExternalId = null)
    {
        var id = NewId();
        var response = await PostUserAsync(id, partnerExternalId);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return id;
    }

    [Fact]
    public async Task Create_DuplicateExternalId_Returns409()
    {
        var userId = await CreateUserAsync();
        
        var response = await PostUserAsync(userId);
        
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task SetPartner_WouldCreateCycle_Returns422()
    {
        var partnerId = await CreateUserAsync();
        var userId = await CreateUserAsync(partnerId);
        
        var response = await SetPartnerAsync(partnerId, userId);
        
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Create_ConcurrentDuplicates_OnlyOneSucceeds()
    {
        var id = NewId();
        var tasks = Enumerable.Range(0, 10).Select(_ => PostUserAsync(id));

        var responses = await Task.WhenAll(tasks);
        var statuses = responses.Select(r => r.StatusCode).ToList();
        
        Assert.Single(statuses, s => s == HttpStatusCode.Created);
        Assert.Equal(9, statuses.Count(s => s == HttpStatusCode.Conflict));
    }
    
    [Fact]
    public async Task SetPartner_ConcurrentOppositeLinks_NoCycle()
    {
        for (var i = 0; i < 20; i++)
        {
            var firstId = await CreateUserAsync();
            var secondId = await CreateUserAsync();

            var tasks = new List<Task<HttpResponseMessage>>
            {
                SetPartnerAsync(firstId, secondId),
                SetPartnerAsync(secondId, firstId)
            };
            var responses = await Task.WhenAll(tasks);

            var statuses = responses.Select(r => r.StatusCode).ToList();
            Assert.Single(statuses, s => s == HttpStatusCode.NoContent);
            Assert.Single(statuses, s => s == HttpStatusCode.UnprocessableEntity);
        }
    }

    [Fact]
    public async Task SetPartner_Self_Returns422()
    {
        var userId = await CreateUserAsync();
        
        var response = await SetPartnerAsync(userId, userId);
        
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
    
    [Fact]
    public async Task SetPartner_UnknownPartner_Returns404()
    {
        var userId = await CreateUserAsync();
        
        var response = await SetPartnerAsync(userId, NewId());
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_InvalidExternalId_Returns400()
    {
        var response = await PostUserAsync("bad id!");
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}