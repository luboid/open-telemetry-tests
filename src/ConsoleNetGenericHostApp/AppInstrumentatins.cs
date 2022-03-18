using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ConsoleNetGenericHostApp
{
    public static class AppInstrumentatins
    {
        public static void AddAppInstrumentatins(this IServiceCollection services, IConfiguration configuration)
        {
            var otelCollector = new OtelCollectorSettings();

            var numberCycles = Source.Meter.CreateCounter<int>(Source.ServiceName + "_cycles", "number", "Number of cycles.");

            services.AddSingleton(Source.ActivitySource);
            services.AddSingleton(Source.Meter);
            services.AddSingleton(numberCycles);

            // This is required if the collector doesn't expose an https endpoint as .NET by default
            // only allow http2 (required for gRPC) to secure endpoints
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            configuration.Bind(nameof(OtelCollectorSettings), otelCollector);

            services.AddOpenTelemetryTracing(builder =>
            {
                builder
                .AddSource(Source.ServiceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: Source.ServiceName, serviceVersion: Source.ServiceVersion))
                .AddHttpClientInstrumentation()
                .AddOtlpExporter((exporterOptions) =>
                {
                    exporterOptions.Endpoint = new Uri(otelCollector.Uri);
                    exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                });
            });

            services.AddOpenTelemetryMetrics(builder =>
            {
                builder
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
            });
        }
    }
}
