namespace VerticalSliceTest.Orders.Api.Common.Queries;

internal sealed class QueryValidationHandlerDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> inner,
    IEnumerable<IRequestValidator<TQuery>> validators)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        Validate(query, validators);

        return await inner.Handle(query, cancellationToken).ConfigureAwait(false);
    }

    private static void Validate(TQuery query, IEnumerable<IRequestValidator<TQuery>> validators)
    {
        var errors = validators
            .SelectMany(validator => validator.Validate(query))
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.Message).ToArray());

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }
}
