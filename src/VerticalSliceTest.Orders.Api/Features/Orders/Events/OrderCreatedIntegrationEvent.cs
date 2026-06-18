namespace VerticalSliceTest.Orders.Api.Features.Orders.Events;

internal sealed record OrderCreatedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid OrderId) : IIntegrationEvent;