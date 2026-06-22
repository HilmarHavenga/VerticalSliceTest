namespace VerticalSliceTest.Orders.Api.Common.Results.Failures;

public static class CommonFailures
{
    public static Failure NullValue => new("Common.NullValue", "Null value was provided.");
}
