namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal sealed class IntegrationEventConsumerService(
    IConsumer consumer,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<IntegrationEventConsumerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await consumer.ConsumeAsync(ProcessMessageAsync, stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "RabbitMQ consumer stopped unexpectedly.");
            throw;
        }
    }

    private async Task ProcessMessageAsync(IntegrationEventEnvelope envelope, CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IInboxMessageProcessor processor = scope.ServiceProvider.GetRequiredService<IInboxMessageProcessor>();

        await processor.ProcessAsync(envelope, cancellationToken).ConfigureAwait(false);
    }
}
