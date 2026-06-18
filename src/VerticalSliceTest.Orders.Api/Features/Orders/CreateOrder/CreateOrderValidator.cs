namespace VerticalSliceTest.Orders.Api.Features.Orders.CreateOrder;

internal sealed class CreateOrderValidator : IRequestValidator<CreateOrderRequest>
{
    public IEnumerable<RequestValidationFailure> Validate(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            yield return new RequestValidationFailure(nameof(request.CustomerName), "Customer name is required.");
        }

        if (request.CustomerName.Length > 200)
        {
            yield return new RequestValidationFailure(nameof(request.CustomerName), "Customer name cannot exceed 200 characters.");
        }

        if (request.TotalAmount <= 0)
        {
            yield return new RequestValidationFailure(nameof(request.TotalAmount), "Total amount must be greater than zero.");
        }
    }
}
