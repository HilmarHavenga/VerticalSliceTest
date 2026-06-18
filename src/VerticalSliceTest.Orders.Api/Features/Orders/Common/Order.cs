namespace VerticalSliceTest.Orders.Api.Features.Orders.Common;

public sealed class Order
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Order()
    {
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    private Order(Guid id, string customerName, decimal totalAmount, DateTime createdOnUtc)
    {
        Id = id;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        CreatedOnUtc = createdOnUtc;
        Status = OrderStatus.Pending;
    }

    public static Order Create(Guid id, string customerName, decimal totalAmount, DateTime createdOnUtc)
        => new(id, customerName, totalAmount, createdOnUtc);

    public Guid Id { get; private set; }

    public string CustomerName { get; private set; }

    public decimal TotalAmount { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }
}