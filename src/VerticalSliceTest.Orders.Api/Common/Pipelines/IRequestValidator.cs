namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

public interface IRequestValidator<in TRequest>
    where TRequest : notnull
{
    IEnumerable<RequestValidationFailure> Validate(TRequest request);
}
