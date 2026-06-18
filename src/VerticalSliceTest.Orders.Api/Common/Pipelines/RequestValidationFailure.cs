namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

public sealed record RequestValidationFailure(string PropertyName, string Message);