namespace VerticalSliceTest.Orders.Api.Infrastructure.Telemetry;

internal sealed class TelemetryOptions
{
    public const string SectionName = "Telemetry";

    public bool Enabled { get; init; }

    public string ServiceName { get; init; } = "VerticalSliceTest.Orders.Api";

    public string ServiceVersion { get; init; } = "1.0.0";

    public TelemetryExporter Exporter { get; init; } = TelemetryExporter.None;

    public string? OtlpEndpoint { get; init; }

    public string OtlpProtocol { get; init; } = "HttpProtobuf";

    public bool IncludeFormattedLogMessage { get; init; } = true;

    public bool IncludeLogScopes { get; init; } = true;

    public bool ExportLogs { get; init; } = true;

    public bool ExportTraces { get; init; } = true;

    public bool ExportMetrics { get; init; } = true;
}
