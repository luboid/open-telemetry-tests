using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ConsoleNetApp
{
    public static class AppInstrumentatins
    {
        public static TracerProvider TracerProvider { get; private set; }
        public static MeterProvider MeterProvider { get; private set; }

        public static void Configure()
        {

            var otelCollector = new OtelCollectorSettings()
            {
                Uri = "http://localhost:4317",
            };

            // This is required if the collector doesn't expose an https endpoint as .NET by default
            // only allow http2 (required for gRPC) to secure endpoints
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var tracerBuilder = Sdk.CreateTracerProviderBuilder();

            tracerBuilder.AddSource(Source.ServiceName)
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: Source.ServiceName, serviceVersion: Source.ServiceVersion))
            .AddHttpClientInstrumentation()
            .AddOtlpExporter((exporterOptions) =>
            {
                exporterOptions.Endpoint = new Uri(otelCollector.Uri);
                exporterOptions.Protocol = OtlpExportProtocol.Grpc;
            });

            var meterBuilder = Sdk.CreateMeterProviderBuilder();

            meterBuilder
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: Source.ServiceName, serviceVersion: Source.ServiceVersion))
            .AddMeter(Source.ServiceName)
            .AddHttpClientInstrumentation()
            .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
            {
                exporterOptions.Endpoint = new Uri(otelCollector.Uri);
                exporterOptions.Protocol = OtlpExportProtocol.Grpc;

                metricReaderOptions.MetricReaderType = MetricReaderType.Periodic;
                metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
                metricReaderOptions.Temporality = AggregationTemporality.Cumulative;
            });

            TracerProvider = tracerBuilder.Build();
            MeterProvider = meterBuilder.Build();
        }
    }
}
