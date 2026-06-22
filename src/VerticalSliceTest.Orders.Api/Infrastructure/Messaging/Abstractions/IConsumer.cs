namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging.Abstractions;

public interface IConsumer
{
    Task ConsumeAsync(
        Func<IntegrationEventEnvelope, CancellationToken, Task> handleMessageAsync,
        CancellationToken cancellationToken = default);
}
