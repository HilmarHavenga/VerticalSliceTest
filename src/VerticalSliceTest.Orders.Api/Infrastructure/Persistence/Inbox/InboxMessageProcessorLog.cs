namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Inbox;

internal static partial class InboxMessageProcessorLog
{
    [LoggerMessage(EventId = 2100, Level = LogLevel.Information, Message = "Skipping already processed inbox message {MessageId}")]
    public static partial void SkippingAlreadyProcessed(ILogger logger, Guid messageId);
}
