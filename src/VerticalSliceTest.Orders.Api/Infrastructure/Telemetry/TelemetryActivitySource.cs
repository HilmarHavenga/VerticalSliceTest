namespace VerticalSliceTest.Orders.Api.Infrastructure.Telemetry;

internal static class TelemetryActivitySource
{
    public const string Name = "VerticalSliceTest.Orders.Api";

    public static readonly ActivitySource Instance = new(Name);

    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
        => Instance.StartActivity(name, kind);
}
