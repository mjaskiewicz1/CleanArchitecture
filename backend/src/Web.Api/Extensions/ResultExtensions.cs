using System.Net;

using Application.Common;

using Domain.Shared;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Extensions;

internal static class ResultExtensions
{
    private static IHttpContextAccessor? s_accessor;

    public static void Init(IHttpContextAccessor accessor) => s_accessor = accessor;

    extension(Result result)
    {
        public IActionResult ToOkResult()
            => result.IsSuccess
                ? new OkResult()
                : result.Error.ToProblemDetails();
        public IActionResult NoContentResult()
            => result.IsSuccess
                ? new NoContentResult()
                : result.Error.ToProblemDetails();
    }

    extension<T>(Result<T> result)
    {
        public IActionResult ToOkObjectResult()
            => result.IsSuccess
                ? new OkObjectResult(result.Value)
                : result.Error.ToProblemDetails();
    }

    extension<T>(Result<T> result) where T : BaseResponse
    {
        public IActionResult ToCreatedAtAction(string actionName)
        {
            return result.IsSuccess
                ? new CreatedAtActionResult(actionName, null, new { id = result.Value.Id }, result)
                : result.Error.ToProblemDetails();
        }
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
                problemDetails.Extensions["errors"] = error.ValidationErrors;

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