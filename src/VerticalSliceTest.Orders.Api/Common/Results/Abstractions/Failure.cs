namespace VerticalSliceTest.Orders.Api.Common.Results.Abstractions;

public record Failure(string Code, string Description)
{
    public static Failure None => new(string.Empty, string.Empty);
}
