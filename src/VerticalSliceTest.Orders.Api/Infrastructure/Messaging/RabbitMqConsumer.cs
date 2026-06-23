namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal sealed class RabbitMqConsumer(
    IConnection connection,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqConsumer> logger) : IConsumer
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task ConsumeAsync(
        Func<IntegrationEventEnvelope, CancellationToken, Task> handleMessageAsync,
        CancellationToken cancellationToken = default)
    {
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: _options.PrefetchCount,
            global: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            await HandleReceivedAsync(channel, eventArgs, handleMessageAsync, cancellationToken).ConfigureAwait(false);
        };

        await channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        RabbitMqConsumerLog.Started(logger, _options.QueueName);

        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(false);
    }

    private async Task HandleReceivedAsync(
        IChannel channel,
        BasicDeliverEventArgs eventArgs,
        Func<IntegrationEventEnvelope, CancellationToken, Task> handleMessageAsync,
        CancellationToken cancellationToken)
    {
        try
        {
            string message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            IntegrationEventEnvelope envelope = MessageSerialization.Deserialize<IntegrationEventEnvelope>(message) ??
                throw new InvalidOperationException("RabbitMQ message could not be deserialized as an integration event envelope.");

            using Activity? activity = TelemetryActivitySource.StartActivity(TelemetryActivityNames.MessagingConsume(envelope.EventType), ActivityKind.Consumer);
            activity?.SetTag(TelemetryTags.MessagingSystem, TelemetryTagValues.RabbitMq);
            activity?.SetTag(TelemetryTags.MessagingDestinationName, _options.QueueName);
            activity?.SetTag(TelemetryTags.MessagingOperationName, TelemetryTagValues.Consume);
            activity?.SetTag(TelemetryTags.MessagingMessageId, envelope.MessageId);
            activity?.SetTag(TelemetryTags.MessagingMessageType, envelope.EventType);

            try
            {
                await handleMessageAsync(envelope, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
                activity?.AddException(exception);
                throw;
            }

            await channel.BasicAckAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            RabbitMqConsumerLog.MessageProcessingCancelled(logger, eventArgs.DeliveryTag);
        }
        catch (Exception exception)
        {
            RabbitMqConsumerLog.ConsumingFailed(logger, exception, eventArgs.DeliveryTag);

            await channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
    }
}
