namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Inbox;

internal interface IInboxMessageProcessor
{
    Task ProcessAsync(IntegrationEventEnvelope envelope, CancellationToken cancellationToken = default);
}
