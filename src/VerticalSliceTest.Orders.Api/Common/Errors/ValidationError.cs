namespace VerticalSliceTest.Orders.Api.Common.Errors;

public sealed record ValidationError(string PropertyName, string ErrorMessage);