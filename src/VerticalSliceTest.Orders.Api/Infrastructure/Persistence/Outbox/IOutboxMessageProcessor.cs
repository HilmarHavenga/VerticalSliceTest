namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Outbox;

internal interface IOutboxMessageProcessor
{
    Task ProcessAsync(int batchSize, CancellationToken cancellationToken = default);
}
