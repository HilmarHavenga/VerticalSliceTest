namespace VerticalSliceTest.Orders.Api.Infrastructure.Telemetry;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        TelemetryOptions options = builder.Configuration
            .GetSection(TelemetryOptions.SectionName)
            .Get<TelemetryOptions>() ?? new TelemetryOptions();

        if (!options.Enabled || options.Exporter == TelemetryExporter.None)
        {
            return builder;
        }

        builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection(TelemetryOptions.SectionName));

        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: options.ServiceName,
                serviceVersion: options.ServiceVersion);

        if (options.ExportLogs)
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.SetResourceBuilder(resourceBuilder);
                logging.IncludeFormattedMessage = options.IncludeFormattedLogMessage;
                logging.IncludeScopes = options.IncludeLogScopes;

                AddLogExporter(logging, options);
            });
        }

        OpenTelemetryBuilder openTelemetryBuilder = builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: options.ServiceName,
                    serviceVersion: options.ServiceVersion));

        if (options.ExportTraces)
        {
            openTelemetryBuilder.WithTracing(tracing =>
            {
                tracing
                    .AddSource(TelemetryActivitySource.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                AddTraceExporter(tracing, options);
            });
        }

        if (options.ExportMetrics)
        {
            openTelemetryBuilder.WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                AddMetricExporter(metrics, options);
            });
        }

        return builder;
    }

    private static void AddLogExporter(OpenTelemetryLoggerOptions logging, TelemetryOptions options)
    {
        if (options.Exporter == TelemetryExporter.Console)
        {
            logging.AddConsoleExporter();
            return;
        }

        if (options.Exporter == TelemetryExporter.Otlp)
        {
            logging.AddOtlpExporter(otlpOptions => ConfigureOtlpExporter(otlpOptions, options));
        }
    }

    private static void AddTraceExporter(TracerProviderBuilder tracing, TelemetryOptions options)
    {
        if (options.Exporter == TelemetryExporter.Console)
        {
            tracing.AddConsoleExporter();
            return;
        }

        if (options.Exporter == TelemetryExporter.Otlp)
        {
            tracing.AddOtlpExporter(otlpOptions => ConfigureOtlpExporter(otlpOptions, options));
        }
    }

    private static void AddMetricExporter(MeterProviderBuilder metrics, TelemetryOptions options)
    {
        if (options.Exporter == TelemetryExporter.Console)
        {
            metrics.AddConsoleExporter();
            return;
        }

        if (options.Exporter == TelemetryExporter.Otlp)
        {
            metrics.AddOtlpExporter(otlpOptions => ConfigureOtlpExporter(otlpOptions, options));
        }
    }

    private static void ConfigureOtlpExporter(OtlpExporterOptions otlpOptions, TelemetryOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint))
        {
            otlpOptions.Endpoint = new Uri(options.OtlpEndpoint);
        }

        otlpOptions.Protocol = options.OtlpProtocol.Equals("Grpc", StringComparison.OrdinalIgnoreCase)
            ? OtlpExportProtocol.Grpc
            : OtlpExportProtocol.HttpProtobuf;
    }
}
