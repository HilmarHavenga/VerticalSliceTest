namespace VerticalSliceTest.Orders.Api.Common.Validation;

public sealed record RequestValidationFailure(string PropertyName, string Message);