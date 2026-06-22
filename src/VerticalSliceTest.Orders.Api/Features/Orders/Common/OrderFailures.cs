namespace VerticalSliceTest.Orders.Api.Features.Orders.Common;

public static class OrderFailures
{
    public static Failure CustomerNameRequired => new("Order.CustomerNameRequired", "Customer name is required.");

    public static Failure CustomerNameTooLong => new("Order.CustomerNameTooLong", "Customer name cannot exceed 200 characters.");

    public static Failure TotalAmountInvalid => new("Order.TotalAmountInvalid", "Total amount must be greater than zero.");
}
