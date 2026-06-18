namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Inbox;

internal sealed class InboxMessageProcessor(
    ApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IMessageSerializer messageSerializer,
    IServiceProvider serviceProvider,
    ILogger<InboxMessageProcessor> logger) : IInboxMessageProcessor
{
    public async Task ProcessAsync(IntegrationEventEnvelope envelope, CancellationToken cancellationToken = default)
    {
        InboxMessage? inboxMessage = await dbContext.Set<InboxMessage>()
            .FindAsync([envelope.MessageId], cancellationToken)
            .ConfigureAwait(false);

        if (inboxMessage?.ProcessedOnUtc is not null)
        {
            logger.LogInformation("Skipping already processed inbox message {MessageId}", envelope.MessageId);
            return;
        }

        if (inboxMessage is null)
        {
            inboxMessage = new InboxMessage(
                envelope.MessageId,
                envelope.OccurredOnUtc,
                envelope.EventType,
                envelope.Payload);

            dbContext.Add(inboxMessage);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        try
        {
            IIntegrationEvent integrationEvent = DeserializeIntegrationEvent(envelope);

            await DispatchAsync(integrationEvent, cancellationToken).ConfigureAwait(false);

            inboxMessage.SetProcessedOnUtc(dateTimeProvider);
        }
        catch (Exception exception)
        {
            inboxMessage.SetError(exception.ToString());
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            throw;
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private IIntegrationEvent DeserializeIntegrationEvent(IntegrationEventEnvelope envelope)
    {
        Type? eventType = Type.GetType(envelope.EventType);

        if (eventType is null)
        {
            throw new InvalidOperationException($"Unknown integration event type '{envelope.EventType}'.");
        }

        if (!typeof(IIntegrationEvent).IsAssignableFrom(eventType))
        {
            throw new InvalidOperationException($"Type '{eventType.Name}' is not an integration event.");
        }

        return (IIntegrationEvent)messageSerializer.Deserialize(envelope.Payload, eventType)!;
    }

    private async Task DispatchAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        Type handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(integrationEvent.GetType());
        Type enumerableHandlerType = typeof(IEnumerable<>).MakeGenericType(handlerType);

        IEnumerable handlers = (IEnumerable?)serviceProvider.GetService(enumerableHandlerType) ?? Array.Empty<object>();

        foreach (object handler in handlers)
        {
            Task task = (Task)handlerType
                .GetMethod(nameof(IIntegrationEventHandler<>.HandleAsync))!
                .Invoke(handler, [integrationEvent, cancellationToken])!;

            await task.ConfigureAwait(false);
        }
    }
}
