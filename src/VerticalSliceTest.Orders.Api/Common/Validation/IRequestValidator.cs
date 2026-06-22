namespace VerticalSliceTest.Orders.Api.Common.Validation;

public interface IRequestValidator<in TRequest>
    where TRequest : notnull
{
    IEnumerable<RequestValidationFailure> Validate(TRequest request);
}
