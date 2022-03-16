using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BackendWebApi
{
    public static class AppInstrumentatins
    {
        public static void AddAppInstrumentatins(this WebApplicationBuilder builder)
        {
            var otelCollector = new OtelCollectorSettings();

            var numberOfCalls = Source.Meter.CreateCounter<int>(Source.ServiceName.ToLowerInvariant() + "_calls", "number", "Number of API calls.");

            builder.Services.AddSingleton(Source.ActivitySource);
            builder.Services.AddSingleton(Source.Meter);
            builder.Services.AddSingleton(numberOfCalls);

            // This is required if the collector doesn't expose an https endpoint as .NET by default
            // only allow http2 (required for gRPC) to secure endpoints
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            builder.Configuration.Bind(nameof(OtelCollectorSettings), otelCollector);

            builder.Services.AddOpenTelemetryTracing(builder =>
            {
                builder
                .AddSource(Source.ServiceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: Source.ServiceName, serviceVersion: Source.ServiceVersion))
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter((exporterOptions) =>
                {
                    exporterOptions.Endpoint = new Uri(otelCollector.Uri);
                    exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                });
            });

            builder.Services.AddOpenTelemetryMetrics(builder =>
            {
                builder
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: Source.ServiceName, serviceVersion: Source.ServiceVersion))
                .AddMeter(Source.ServiceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                {
                    exporterOptions.Endpoint = new Uri(otelCollector.Uri);
                    exporterOptions.Protocol = OtlpExportProtocol.Grpc;

                    metricReaderOptions.MetricReaderType = MetricReaderType.Periodic;
                    metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
                    metricReaderOptions.Temporality = AggregationTemporality.Cumulative;
                });
            });

        }
    }
}
