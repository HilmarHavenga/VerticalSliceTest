namespace VerticalSliceTest.Orders.Api.Common.Commands;

internal sealed class CommandTelemetryHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        string commandName = typeof(TCommand).Name;

        using Activity? activity = TelemetryActivitySource.StartActivity(TelemetryActivityNames.Command(commandName));
        activity?.SetTag(TelemetryTags.CodeFunction, commandName);
        activity?.SetTag(TelemetryTags.RequestType, TelemetryTagValues.Command);

        try
        {
            TResponse response = await inner.Handle(command, cancellationToken).ConfigureAwait(false);

            if (response is Result result)
            {
                activity?.SetTag(TelemetryTags.ResultSuccess, result.IsSuccess);
                activity?.SetTag(TelemetryTags.ResultFailureCode, result.Error.Code);
            }

            return response;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity?.AddException(exception);
            throw;
        }
    }
}
