using FluentValidation;
using MediatR;

namespace Api.Application.Common.Behaviors;

// This MediatR pipeline behavior runs before every handler.
// Its job is to execute all validators for a request and stop the pipeline if validation fails.
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        // If no validators exist for this request type, continue immediately.
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        // Run validators in parallel because they are independent checks against the same request object.
        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, ct))
        );

        // Flatten all validation failures into one list.
        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        // Throwing here means the actual handler never runs for invalid input.
        if (failures.Count != 0)
            throw new ValidationException(failures);

        // All checks passed, so the request can continue to the real handler.
        return await next();
    }
}
