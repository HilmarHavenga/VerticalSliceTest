namespace VerticalSliceTest.Orders.Api.Common.OpenApi;

public static class Versions
{
    public static int V1 => 1;

    public static int V2 => 2;

    public static IReadOnlyCollection<int> All => [V1, V2];

    public static IReadOnlyCollection<string> AllAsStrings => [.. All.Select(verionNumber => $"v{verionNumber}")];
}