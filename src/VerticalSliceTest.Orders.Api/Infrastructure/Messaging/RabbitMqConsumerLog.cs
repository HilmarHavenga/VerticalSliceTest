namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal static partial class RabbitMqConsumerLog
{
    [LoggerMessage(EventId = 3100, Level = LogLevel.Information, Message = "RabbitMQ consumer started for queue {QueueName}.")]
    public static partial void Started(ILogger logger, string queueName);

    [LoggerMessage(EventId = 3101, Level = LogLevel.Information, Message = "RabbitMQ message processing was cancelled for delivery {DeliveryTag}.")]
    public static partial void MessageProcessingCancelled(ILogger logger, ulong deliveryTag);

    [LoggerMessage(EventId = 3102, Level = LogLevel.Error, Message = "Exception while consuming RabbitMQ message {DeliveryTag}.")]
    public static partial void ConsumingFailed(ILogger logger, Exception exception, ulong deliveryTag);
}
