namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal static partial class QueryHandlerLog
{
    [LoggerMessage(EventId = 4200, Level = LogLevel.Information, Message = "Handling query {QueryName}")]
    public static partial void Handling(ILogger logger, string queryName);

    [LoggerMessage(EventId = 4201, Level = LogLevel.Information, Message = "Handled query {QueryName}")]
    public static partial void Handled(ILogger logger, string queryName);

    [LoggerMessage(EventId = 4202, Level = LogLevel.Error, Message = "Failed handling query {QueryName}")]
    public static partial void Failed(ILogger logger, Exception exception, string queryName);
}
