namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal sealed class CommandValidationHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    IEnumerable<IRequestValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        Validate(command, validators);

        return await inner.Handle(command, cancellationToken).ConfigureAwait(false);
    }

    private static void Validate(TCommand command, IEnumerable<IRequestValidator<TCommand>> validators)
    {
        var errors = validators
            .SelectMany(validator => validator.Validate(command))
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
