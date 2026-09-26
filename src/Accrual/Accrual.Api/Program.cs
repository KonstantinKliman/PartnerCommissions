using System.Text.Json.Serialization;
using Accrual.Api.ExceptionHandlers;
using Accrual.Api.Workers;
using Accrual.Application;
using Accrual.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(allowIntegerValues: false)));

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();

builder.Services.AddHostedService<OutboxWorker>();

builder.Services.Configure<HostOptions>(opt =>
    opt.ShutdownTimeout = TimeSpan.FromSeconds(30));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions()
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions()
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();