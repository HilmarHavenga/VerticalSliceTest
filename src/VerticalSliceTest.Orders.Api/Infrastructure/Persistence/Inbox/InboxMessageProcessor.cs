namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Inbox;

internal sealed class InboxMessageProcessor(
    ApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IServiceProvider serviceProvider,
    ILogger<InboxMessageProcessor> logger) : IInboxMessageProcessor
{
    public async Task ProcessAsync(IntegrationEventEnvelope envelope, CancellationToken cancellationToken = default)
    {
        using Activity? activity = TelemetryActivitySource.StartActivity(TelemetryActivityNames.InboxProcess);
        activity?.SetTag(TelemetryTags.InboxMessageId, envelope.MessageId);
        activity?.SetTag(TelemetryTags.MessagingMessageType, envelope.EventType);

        InboxMessage? inboxMessage = await dbContext.Set<InboxMessage>()
            .FindAsync([envelope.MessageId], cancellationToken)
            .ConfigureAwait(false);

        if (inboxMessage?.ProcessedOnUtc is not null)
        {
            activity?.SetTag(TelemetryTags.InboxDuplicate, true);
            InboxMessageProcessorLog.SkippingAlreadyProcessed(logger, envelope.MessageId);
            return;
        }

        await dbContext.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
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

            IIntegrationEvent integrationEvent = DeserializeIntegrationEvent(envelope);

            await DispatchAsync(integrationEvent, cancellationToken).ConfigureAwait(false);

            inboxMessage.SetProcessedOnUtc(dateTimeProvider);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await dbContext.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await dbContext.RollbackTransactionAsync(CancellationToken.None).ConfigureAwait(false);
            throw;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity?.AddException(exception);
            await dbContext.RollbackTransactionAsync(CancellationToken.None).ConfigureAwait(false);
            await SaveFailedInboxMessageAsync(envelope, exception, cancellationToken).ConfigureAwait(false);
            throw;
        }
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

        return (IIntegrationEvent)MessageSerialization.Deserialize(envelope.Payload, eventType)!;
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

    private async Task SaveFailedInboxMessageAsync(
        IntegrationEventEnvelope envelope,
        Exception exception,
        CancellationToken cancellationToken)
    {
        dbContext.ChangeTracker.Clear();

        InboxMessage? inboxMessage = await dbContext.Set<InboxMessage>()
            .FindAsync([envelope.MessageId], cancellationToken)
            .ConfigureAwait(false);

        if (inboxMessage is null)
        {
            inboxMessage = new InboxMessage(
                envelope.MessageId,
                envelope.OccurredOnUtc,
                envelope.EventType,
                envelope.Payload);

            dbContext.Add(inboxMessage);
        }

        inboxMessage.SetError(exception.ToString());

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
