namespace VerticalSliceTest.Orders.Api.Features.Orders.CreateOrder;

internal sealed class CreateOrderValidator : IRequestValidator<CreateOrderRequest>
{
    public IEnumerable<RequestValidationFailure> Validate(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            yield return new RequestValidationFailure(nameof(request.CustomerName), OrderFailures.CustomerNameRequired.Description);
        }

        if (request.CustomerName.Length > 200)
        {
            yield return new RequestValidationFailure(nameof(request.CustomerName), OrderFailures.CustomerNameTooLong.Description);
        }

        if (request.TotalAmount <= 0)
        {
            yield return new RequestValidationFailure(nameof(request.TotalAmount), OrderFailures.TotalAmountInvalid.Description);
        }
    }
}
