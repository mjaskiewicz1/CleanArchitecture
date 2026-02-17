using System.Text.Json;
using System.Text.Json.Serialization;

using Grafana.OpenTelemetry;

using Microsoft.AspNetCore.Http.Features;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using Web.Api.Exceptions;

namespace Web.Api.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPresentation()
        {
            services.AddOpenApi(static options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

            services.AddControllers(static mvcOptions =>
            {
                mvcOptions.ReturnHttpNotAcceptable = true;
                mvcOptions.SuppressAsyncSuffixInActionNames = false;
            }).AddJsonOptions(static options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.AllowTrailingCommas = false;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddOpenTelemetry()
                .UseGrafana()
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(options => options.RecordException = true))
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation());
            services.AddExceptionHandling();
        }

        private void AddExceptionHandling()
        {
            // Order is important! Exceptions handlers are executed top to bottom.
            services.AddProblemDetails(static problemDetailsOptions => problemDetailsOptions.CustomizeProblemDetails =
                static context =>
                {
                    context.ProblemDetails.Instance =
                        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    context.ProblemDetails.Extensions.TryAdd("requestId",
                        context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity.Id);
                    context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
                });

            services.AddExceptionHandler<GlobalExceptionHandler>();
        }
    }
}