using System.Net;

using Domain.Shared;

using Hangfire.Annotations;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Extensions;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
internal static class ResultExtensions
{
    private static IHttpContextAccessor? s_accessor;

    public static void Init(IHttpContextAccessor accessor) => s_accessor = accessor;

    extension(Result result)
    {
        public IActionResult ToActionResult()
            => result.IsSuccess
                ? new OkResult()
                : result.Error.ToProblemDetails();
    }

    extension<T>(Result<T> result)
    {
        public IActionResult ToActionResult()
            => result.IsSuccess
                ? new OkObjectResult(result.Value)
                : result.Error.ToProblemDetails();

        public IActionResult ToCreatedAtRouteResult(string routeName, ulong routeValues)
            => result.IsSuccess
                ? new CreatedAtRouteResult(routeName, routeValues, result.Value)
                : result.Error.ToProblemDetails();
    }

    extension(Error error)
    {
        private ObjectResult ToProblemDetails()
        {
            var httpContext = s_accessor?.HttpContext ?? throw new InvalidOperationException("HttpContext is null");
            var statusCode = (int)error.StatusCode;

            var problemDetails = new ProblemDetails
            {
                Title = GetTitle(error.StatusCode),
                Detail = error.Message,
                Status = statusCode,
                Type = GetTypeUrl(error.StatusCode),
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            if (error.ValidationErrors?.Count > 0)
            {
                problemDetails.Extensions["errors"] = error.ValidationErrors;
            }
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            problemDetails.Extensions["requestId"] = httpContext.Features.Get<IHttpActivityFeature>()?.Activity.Id;

            return new ObjectResult(problemDetails) { StatusCode = statusCode };
        }
    }

    private static string GetTitle(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => "Bad Request",
        HttpStatusCode.NotFound => "Resource Not Found",
        HttpStatusCode.Conflict => "Conflict",
        HttpStatusCode.Unauthorized => "Unauthorized",
        HttpStatusCode.Forbidden => "Forbidden",
        _ => "An error occurred"
    };

    private static string GetTypeUrl(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        HttpStatusCode.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        HttpStatusCode.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
        HttpStatusCode.Forbidden => "https://tools.ietf.org/html/rfc7235#section-3.3",
        _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
    };
}