namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging.Abstractions;

public interface IIntegrationEvent
{
    Guid Id { get; }

    DateTime OccurredOnUtc { get; }
}
