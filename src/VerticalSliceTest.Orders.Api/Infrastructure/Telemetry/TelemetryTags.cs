namespace VerticalSliceTest.Orders.Api.Infrastructure.Telemetry;

internal static class TelemetryTags
{
    public const string CodeFunction = "code.function";

    public const string RequestType = "app.request.type";

    public const string ResultSuccess = "app.result.success";

    public const string ResultFailureCode = "app.result.failure_code";

    public const string MessagingSystem = "messaging.system";

    public const string MessagingDestinationName = "messaging.destination.name";

    public const string MessagingOperationName = "messaging.operation.name";

    public const string MessagingMessageId = "messaging.message.id";

    public const string MessagingMessageType = "messaging.message.type";

    public const string OutboxBatchSize = "app.outbox.batch_size";

    public const string OutboxMessageCount = "app.outbox.message_count";

    public const string OutboxMessageId = "app.outbox.message_id";

    public const string InboxMessageId = "app.inbox.message_id";

    public const string InboxDuplicate = "app.inbox.duplicate";
}
