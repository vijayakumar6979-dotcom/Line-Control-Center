using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LineControlCenter.Application.Behaviours;

/// <summary>
/// MediatR pipeline behaviour that catches unhandled exceptions thrown by any handler
/// and converts them into <c>Result.Failure</c> so callers receive a structured error
/// instead of a faulted <see cref="Task"/>.  This keeps the dashboard cards independent:
/// a DB failure in one query will not tear down the others.
/// </summary>
public sealed class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest  : IRequest<TResponse>
    where TResponse : class
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehavior(
        ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest                          request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken                 ct)
    {
        try
        {
            return await next(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception in handler for {Request}",
                typeof(TRequest).Name);

            var error = new Error(
                $"{typeof(TRequest).Name}.Exception",
                ex.Message);

            var responseType = typeof(TResponse);

            // Result (non-generic)
            if (responseType == typeof(Result))
                return (Result.Failure(error) as TResponse)!;

            // Result<T>
            if (responseType.IsGenericType &&
                responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType     = responseType.GetGenericArguments()[0];
                var failureMethod = typeof(Result)
                    .GetMethods()
                    .First(m => m.Name      == nameof(Result.Failure)
                             && m.IsGenericMethod)
                    .MakeGenericMethod(valueType);

                return (failureMethod.Invoke(null, [error]) as TResponse)!;
            }

            // Fallback — re-throw for non-Result handlers
            throw;
        }
    }
}
