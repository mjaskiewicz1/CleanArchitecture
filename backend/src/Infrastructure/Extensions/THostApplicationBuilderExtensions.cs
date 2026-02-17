using Grafana.OpenTelemetry;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Infrastructure.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension<THostApplicationBuilder>(THostApplicationBuilder builder)
        where THostApplicationBuilder : IHostApplicationBuilder
    {
        public void ConfigureInfrastructure()
        {
            builder.ConfigureOpenTelemetry();
        }

        private void ConfigureOpenTelemetry()
        {
            builder.Services.AddOpenTelemetry()
                .UseGrafana()
                .WithMetrics(static metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation();
                    metrics.AddHttpClientInstrumentation();
                    metrics.AddRuntimeInstrumentation();
                })
                .WithTracing(tracing => tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation());
            builder.Logging.ClearProviders();
            builder.Logging.AddOpenTelemetry(static logging =>
            {
                logging.UseGrafana();
                logging.AddConsoleExporter();
            });
        }
    }
}