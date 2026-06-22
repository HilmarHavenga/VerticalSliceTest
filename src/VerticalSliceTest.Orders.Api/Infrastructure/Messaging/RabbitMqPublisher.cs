namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal sealed class RabbitMqPublisher(
    IMessageSerializer messageSerializer,
    IConnection connection,
    IOptions<RabbitMqOptions> options) : IPublisher
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        Type eventType = integrationEvent.GetType();
        string payload = messageSerializer.Serialize(integrationEvent);

        IntegrationEventEnvelope envelope = new(
            integrationEvent.Id,
            eventType.AssemblyQualifiedName ?? eventType.FullName ?? eventType.Name,
            payload,
            integrationEvent.OccurredOnUtc);

        string envelopePayload = messageSerializer.Serialize(envelope);
        byte[] body = Encoding.UTF8.GetBytes(envelopePayload);

        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        await channel.ExchangeDeclareAsync(
            _options.ExchangeName,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        BasicProperties properties = new()
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = envelope.MessageId.ToString(),
            Type = envelope.EventType,
        };

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: eventType.Name,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
