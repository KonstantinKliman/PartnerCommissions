using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Users.Api.ExceptionHandlers;
using Users.Application;
using Users.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(res => res.AddService("users"))
    .WithMetrics(metricsBuilder => metricsBuilder
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter((exporter, reader) =>
        {
            var metricsEndpoint = builder.Configuration["Otlp:MetricsEndpoint"];
            if (metricsEndpoint is null)
                throw new InvalidOperationException("Otlp:MetricsEndpoint is not configured");

            exporter.Endpoint = new Uri(metricsEndpoint);
            exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
            
            reader.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 10_000;
        })
    );

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