namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Outbox;

internal sealed class OutboxMessageProcessor(
    ApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IPublisher publisher,
    ILogger<OutboxMessageProcessor> logger) : IOutboxMessageProcessor
{
    public async Task ProcessAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        using Activity? activity = TelemetryActivitySource.StartActivity(TelemetryActivityNames.OutboxProcess);
        activity?.SetTag(TelemetryTags.OutboxBatchSize, batchSize);

        List<OutboxMessage> outboxMessages = await dbContext.Set<OutboxMessage>()
            .Where(outboxMessage => outboxMessage.ProcessedOnUtc == null)
            .OrderBy(outboxMessage => outboxMessage.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        activity?.SetTag(TelemetryTags.OutboxMessageCount, outboxMessages.Count);

        foreach (OutboxMessage outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                using Activity? messageActivity = TelemetryActivitySource.StartActivity(TelemetryActivityNames.OutboxPublishMessage);
                messageActivity?.SetTag(TelemetryTags.OutboxMessageId, outboxMessage.Id);
                messageActivity?.SetTag(TelemetryTags.MessagingMessageType, outboxMessage.Type);

                IIntegrationEvent integrationEvent = DeserializeIntegrationEvent(outboxMessage);

                await publisher.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);

                OutboxWorkerLog.PublishedEvent(logger, integrationEvent.GetType().Name);
            }
            catch (Exception caughtException)
            {
                activity?.SetStatus(ActivityStatusCode.Error, caughtException.Message);
                activity?.AddException(caughtException);
                OutboxWorkerLog.ProcessingFailed(logger, caughtException, outboxMessage.Id.ToString());
                exception = caughtException;
            }

            if (exception is null)
            {
                outboxMessage.SetProcessedOnUtc(dateTimeProvider);
            }
            else
            {
                outboxMessage.SetError(exception.ToString());
            }

            dbContext.Update(outboxMessage);
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private IIntegrationEvent DeserializeIntegrationEvent(OutboxMessage outboxMessage)
    {
        Type? concreteType = Type.GetType(outboxMessage.Type);

        if (concreteType is null)
        {
            throw new InvalidOperationException($"Unknown outbox event type '{outboxMessage.Type}'.");
        }

        if (!typeof(IIntegrationEvent).IsAssignableFrom(concreteType))
        {
            throw new InvalidOperationException($"Type '{concreteType.Name}' is not an integration event.");
        }

        return (IIntegrationEvent)MessageSerialization.Deserialize(outboxMessage.Content, concreteType)!;
    }
}
