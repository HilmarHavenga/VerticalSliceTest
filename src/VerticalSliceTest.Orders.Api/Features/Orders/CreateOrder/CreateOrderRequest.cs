namespace VerticalSliceTest.Orders.Api.Features.Orders.CreateOrder;

internal sealed class CreateOrderRequest : ICommand<Result<CreateOrderResponse>>
{
    public string CustomerName { get; init; } = string.Empty;

    public decimal TotalAmount { get; init; }
}
