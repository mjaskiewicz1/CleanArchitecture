using Domain.Shared;

using FluentValidation;

using MediatR;

namespace Application.Abstractions.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var validationErrors = results
            .SelectMany(r => r.Errors)
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        if (validationErrors.Count == 0)
            return await next(cancellationToken);

        var error = Error.ValidationGroup(validationErrors);

        return CreateFailureResult(error);

    }

    /// <summary>
    /// Creates a failure result of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <param name="error">The error to include in the failure result.</param>
    /// <returns>A failure result with the specified error.</returns>
    private static TResponse CreateFailureResult(Error error)
    {
        var responseType = typeof(TResponse);

        if (!responseType.IsGenericType ||
            responseType.GetGenericTypeDefinition() != typeof(Result<>))
            return (TResponse)Result.Failure(error);

        var valueType = responseType.GetGenericArguments()[0];

        var failureMethod = typeof(Result<>)
            .MakeGenericType(valueType)
            .GetMethod(nameof(Result<>.Failure))!;

        return (TResponse)failureMethod.Invoke(null, [error])!;
    }
}