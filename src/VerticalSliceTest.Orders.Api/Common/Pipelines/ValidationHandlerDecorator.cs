namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal sealed class ValidationHandlerDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> inner,
    IEnumerable<IRequestValidator<TRequest>> validators)
    : IRequestHandler<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var errors = validators
            .SelectMany(validator => validator.Validate(request))
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.Message).ToArray());

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }

        return await inner.Handle(request, cancellationToken);
    }
}