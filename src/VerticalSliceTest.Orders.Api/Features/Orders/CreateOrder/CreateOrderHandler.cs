namespace VerticalSliceTest.Orders.Api.Features.Orders.CreateOrder;

internal sealed class CreateOrderHandler(
    IDateTimeProvider dateTimeProvider,
    ApplicationDbContext db)
    : ICommandHandler<CreateOrderRequest, Result<CreateOrderResponse>>
{
    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        DateTime createdOnUtc = dateTimeProvider.UtcNow;

        Order order = Order.Create(
            Guid.CreateVersion7(),
            request.CustomerName.Trim(),
            request.TotalAmount,
            createdOnUtc);

        OrderCreatedIntegrationEvent integrationEvent = new(Guid.CreateVersion7(), createdOnUtc, order.Id);

        db.Orders.Add(order);
        db.Add(OutboxMessage.FromIntegrationEvent(integrationEvent));

        CreateOrderResponse response = CreateOrderResponse.CreateFromOrder(order);

        return Result.Success(response);
    }
}
