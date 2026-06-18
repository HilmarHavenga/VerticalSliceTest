namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging.Abstractions;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : IIntegrationEvent;
}
