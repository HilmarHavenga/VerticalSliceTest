namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal static partial class IntegrationEventConsumerServiceLog
{
    [LoggerMessage(EventId = 3000, Level = LogLevel.Error, Message = "Integration event consumer stopped unexpectedly.")]
    public static partial void StoppedUnexpectedly(ILogger logger, Exception exception);
}
