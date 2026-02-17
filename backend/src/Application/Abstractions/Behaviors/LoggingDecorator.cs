using MediatR;

using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using (logger.BeginScope(new Dictionary<string, object> { ["RequestName"] = typeof(TRequest).Name }))
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);

            var response = await next(cancellationToken);
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);

            return response;
        }
    }
}