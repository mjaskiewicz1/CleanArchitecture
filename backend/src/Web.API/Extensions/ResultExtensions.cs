using System.Net;

using Domain.Shared;


using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Extensions;

internal static class ResultExtensions
{
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
            var statusCode = (int)error.StatusCode;

            var problemDetails = new ProblemDetails
            {
                Title = GetTitle(error.StatusCode),
                Detail = error.Message,
                Status = statusCode,
                Type = GetTypeUrl(error.StatusCode)
            };

            if (error.ValidationErrors?.Count > 0)
            {
                problemDetails.Extensions["errors"] = error.ValidationErrors;
            }

            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };
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