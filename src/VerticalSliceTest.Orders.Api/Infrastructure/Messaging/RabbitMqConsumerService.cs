namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal sealed class RabbitMqConsumerService(
    IConnection connection,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqConsumerService> logger) : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IChannel? channel = null;

        try
        {
            channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken).ConfigureAwait(false);

            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: _options.PrefetchCount,
                global: false,
                cancellationToken: stoppingToken).ConfigureAwait(false);

            AsyncEventingBasicConsumer consumer = new(channel);
            consumer.ReceivedAsync += HandleReceivedAsync;

            await channel.BasicConsumeAsync(
                queue: _options.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken).ConfigureAwait(false);

            logger.LogInformation("RabbitMQ consumer started for queue {QueueName}.", _options.QueueName);

            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        finally
        {
            if (channel is not null)
            {
                await channel.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task HandleReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        IChannel channel = ((AsyncEventingBasicConsumer)sender).Channel;

        try
        {
            string message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            using IServiceScope scope = serviceScopeFactory.CreateScope();
            IMessageSerializer serializer = scope.ServiceProvider.GetRequiredService<IMessageSerializer>();
            IInboxMessageProcessor processor = scope.ServiceProvider.GetRequiredService<IInboxMessageProcessor>();

            IntegrationEventEnvelope envelope = serializer.Deserialize<IntegrationEventEnvelope>(message) ??
                throw new InvalidOperationException("RabbitMQ message could not be deserialized as an integration event envelope.");

            await processor.ProcessAsync(envelope).ConfigureAwait(false);

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Exception while consuming RabbitMQ message {DeliveryTag}.", eventArgs.DeliveryTag);

            await channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                requeue: false).ConfigureAwait(false);
        }
    }
}
