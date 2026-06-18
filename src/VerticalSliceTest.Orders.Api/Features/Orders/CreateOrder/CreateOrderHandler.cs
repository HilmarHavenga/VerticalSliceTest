namespace VerticalSliceTest.Orders.Api.Features.Orders.CreateOrder;

internal sealed class CreateOrderHandler(
    IDateTimeProvider dateTimeProvider,
    ApplicationDbContext db,
    IMessageSerializer messageSerializer)
    : IRequestHandler<CreateOrderRequest, Result<CreateOrderResponse>>
{
    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        DateTime createdOnUtc = dateTimeProvider.UtcNow;

        Order order = Order.Create(
            Guid.CreateVersion7(),
            request.CustomerName.Trim(),
            request.TotalAmount,
            createdOnUtc);

        OrderCreatedIntegrationEvent integrationEvent = new(
            Guid.CreateVersion7(),
            createdOnUtc,
            order.Id);

        Type eventType = integrationEvent.GetType();
        OutboxMessage outboxMessage = new(
            integrationEvent.Id,
            integrationEvent.OccurredOnUtc,
            eventType.AssemblyQualifiedName ?? eventType.FullName ?? eventType.Name,
            messageSerializer.Serialize(integrationEvent));

        db.Orders.Add(order);
        db.Add(outboxMessage);

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        CreateOrderResponse response = new(
            order.Id,
            order.CustomerName,
            order.TotalAmount,
            order.Status,
            order.CreatedOnUtc);

        return Result.Success(response);
    }
}
