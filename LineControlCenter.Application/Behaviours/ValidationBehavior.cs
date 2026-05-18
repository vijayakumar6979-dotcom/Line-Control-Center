using FluentValidation;
using LineControlCenter.Domain.Primitives;
using MediatR;

namespace LineControlCenter.Application.Behaviours;

/// <summary>
/// MediatR pipeline behaviour that runs all registered <see cref="IValidator{T}"/>
/// implementations before the handler executes.  Returns <c>Result.Failure</c> on
/// the first validation failure so handlers never receive invalid input.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!_validators.Any())
            return await next(ct);

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(ct);

        // Build a single Error from the first validation failure
        var first = failures[0];
        var error  = new Error(first.PropertyName, first.ErrorMessage);

        // TResponse must be a Result type — use reflection-free pattern via dynamic
        // (safe here: this only runs when validation fails, so not on the hot path)
        var resultType = typeof(TResponse);

        if (resultType == typeof(Result))
            return (Result.Failure(error) as TResponse)!;

        if (resultType.IsGenericType &&
            resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType    = resultType.GetGenericArguments()[0];
            var failureMethod = typeof(Result)
                .GetMethods()
                .First(m => m.Name == nameof(Result.Failure)
                         && m.IsGenericMethod)
                .MakeGenericMethod(valueType);

            return (failureMethod.Invoke(null, [error]) as TResponse)!;
        }

        // Fallback — should not happen if handlers follow Result<T> convention
        throw new ValidationException(failures);
    }
}
