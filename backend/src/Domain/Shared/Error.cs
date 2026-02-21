using System.Net;

using JetBrains.Annotations;

namespace Domain.Shared;
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed record Error(
    string Message,
    HttpStatusCode StatusCode = HttpStatusCode.BadRequest,
    Dictionary<string, string[]>? ValidationErrors = null
)
{
    public static Error None => new(string.Empty);

    public static Error NotFound(string message)
        => new(message, HttpStatusCode.NotFound);

    public static Error BadRequest(string message)
        => new(message);

    public static Error Conflict(string message)
        => new(message, HttpStatusCode.Conflict);

    public static Error Unauthorized(string message)
        => new(message, HttpStatusCode.Unauthorized);

    public static Error Forbidden(string message)
        => new(message, HttpStatusCode.Forbidden);

    public static Error ValidationGroup(Dictionary<string, string[]> validationErrors)
    {
        if (validationErrors == null || validationErrors.Count == 0)
            throw new ArgumentException($"{nameof(ValidationErrors)} cannot be null or empty", nameof(validationErrors));

        return new Error(
            Message: "One or more validation errors occurred",
            StatusCode: HttpStatusCode.BadRequest,
            ValidationErrors: validationErrors);
    }
}