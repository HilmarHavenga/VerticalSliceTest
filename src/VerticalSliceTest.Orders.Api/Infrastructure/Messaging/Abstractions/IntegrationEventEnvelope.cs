namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging.Abstractions;

public sealed record IntegrationEventEnvelope(
    Guid MessageId,
    string EventType,
    string Payload,
    DateTime OccurredOnUtc,
    IReadOnlyDictionary<string, string>? Headers = null);
