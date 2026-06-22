namespace VerticalSliceTest.Orders.Api.Common.Commands;

internal sealed class CommandLoggingHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    ILogger<CommandLoggingHandlerDecorator<TCommand, TResponse>> logger)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        string commandName = typeof(TCommand).Name;
        CommandHandlerLog.Handling(logger, commandName);

        try
        {
            return await inner.Handle(command, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            CommandHandlerLog.Failed(logger, exception, commandName);
            throw;
        }
        finally
        {
            CommandHandlerLog.Handled(logger, commandName);
        }
    }
}
