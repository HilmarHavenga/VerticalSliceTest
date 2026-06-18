namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Outbox;

internal static partial class OutboxWorkerLog
{
    [LoggerMessage(EventId = 2000, Level = LogLevel.Information, Message = "Background service started")]
    public static partial void ServiceStarted(ILogger logger);

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information, Message = "Successfully published event: {EventType}")]
    public static partial void PublishedEvent(ILogger logger, string eventType);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Error, Message = "Exception while processing outbox message {MessageId}")]
    public static partial void ProcessingFailed(ILogger logger, Exception exception, string messageId);
}