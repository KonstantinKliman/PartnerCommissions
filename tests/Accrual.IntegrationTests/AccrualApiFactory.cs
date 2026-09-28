using Accrual.Api.Workers;
using Accrual.Application.Interfaces;
using Accrual.IntegrationTests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace Accrual.IntegrationTests;

public class AccrualApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public FakeUsersClient Users { get; } = new();
    
    public FakeWalletsClient Wallets { get; } = new();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    public async Task RunOutboxAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
        await processor.ProcessBatchAsync(CancellationToken.None);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", _postgres.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IUsersClient>();
            services.AddSingleton<IUsersClient>(Users);

            services.RemoveAll<IWalletsClient>();
            services.AddSingleton<IWalletsClient>(Wallets);

            var outboxWorker = services.Single(d => d.ImplementationType == typeof(OutboxWorker));
            services.Remove(outboxWorker);
        });
    }
}