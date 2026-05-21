using Grafana.OpenTelemetry;

using Infrastructure.Database;

using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

const string sqlConnectionStringName = "SQLConnection";
var builder = FunctionsApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<ApplicationDbContext>(options =>
{
    var sqlConnection = Environment.GetEnvironmentVariable(sqlConnectionStringName);
    if (string.IsNullOrWhiteSpace(sqlConnection))
#pragma warning disable CA2208
        throw new ArgumentNullException(nameof(sqlConnection), $"Connection string '{sqlConnectionStringName}' is not configured properly.");
#pragma warning restore CA2208

    options.UseSqlServer(sqlConnection);

#if DEBUG
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
#endif
});

services.AddOpenTelemetry()
    .UseGrafana()
    .WithTracing(tracing => tracing
        .AddSource("CleanArchitecture.Functions")
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation(options => options.RecordException = true))
    .WithMetrics(metrics => metrics
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation());

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.UseGrafana();
    logging.AddConsoleExporter();
});

builder.ConfigureFunctionsWebApplication();

await builder.Build().RunAsync();