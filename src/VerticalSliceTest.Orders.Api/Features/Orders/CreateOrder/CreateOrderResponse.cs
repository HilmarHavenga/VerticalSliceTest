namespace VerticalSliceTest.Orders.Api.Features.Orders.CreateOrder;

internal sealed record CreateOrderResponse(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedOnUtc);
