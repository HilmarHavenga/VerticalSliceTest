namespace VerticalSliceTest.Orders.Api.Features.Orders.CreateOrder;

internal sealed record CreateOrderResponse(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedOnUtc)
{
    public static CreateOrderResponse CreateFromOrder(Order order)
        => new(order.Id, order.CustomerName, order.TotalAmount, order.Status, order.CreatedOnUtc);
}
