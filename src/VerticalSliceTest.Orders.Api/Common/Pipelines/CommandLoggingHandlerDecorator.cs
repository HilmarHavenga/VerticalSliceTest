namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal sealed class CommandLoggingHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    ILogger<CommandLoggingHandlerDecorator<TCommand, TResponse>> logger)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        string commandName = typeof(TCommand).Name;
        logger.LogInformation("Handling command {CommandName}", commandName);

        try
        {
            return await inner.Handle(command, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed handling command {CommandName}", commandName);
            throw;
        }
        finally
        {
            logger.LogInformation("Handled command {CommandName}", commandName);
        }
    }
}
