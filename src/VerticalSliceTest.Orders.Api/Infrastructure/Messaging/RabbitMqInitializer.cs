namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal sealed class RabbitMqInitializer(
    IConnection connection,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqInitializer> logger) : IHostedService
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        await channel.ExchangeDeclareAsync(
            _options.ExchangeName,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await channel.ExchangeDeclareAsync(
            _options.DeadLetterExchangeName,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await channel.QueueDeclareAsync(
            _options.DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await channel.QueueBindAsync(
            _options.DeadLetterQueueName,
            _options.DeadLetterExchangeName,
            routingKey: string.Empty,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        Dictionary<string, object?> queueArguments = new()
        {
            ["x-dead-letter-exchange"] = _options.DeadLetterExchangeName,
        };

        await channel.QueueDeclareAsync(
            _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArguments,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        foreach (string routingKey in _options.RoutingKeys)
        {
            await channel.QueueBindAsync(
                _options.QueueName,
                _options.ExchangeName,
                routingKey,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        RabbitMqInitializerLog.ExchangeDeclared(logger, _options.ExchangeName);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
