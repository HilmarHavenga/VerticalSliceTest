namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal static partial class CommandHandlerLog
{
    [LoggerMessage(EventId = 4100, Level = LogLevel.Information, Message = "Handling command {CommandName}")]
    public static partial void Handling(ILogger logger, string commandName);

    [LoggerMessage(EventId = 4101, Level = LogLevel.Information, Message = "Handled command {CommandName}")]
    public static partial void Handled(ILogger logger, string commandName);

    [LoggerMessage(EventId = 4102, Level = LogLevel.Error, Message = "Failed handling command {CommandName}")]
    public static partial void Failed(ILogger logger, Exception exception, string commandName);
}
