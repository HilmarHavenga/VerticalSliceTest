namespace VerticalSliceTest.Orders.Api.Features.Orders.CreateOrder;

internal sealed class CreateOrderRequest
{
    public string CustomerName { get; init; } = string.Empty;

    public decimal TotalAmount { get; init; }
}
