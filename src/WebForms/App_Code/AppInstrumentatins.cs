using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System;
using System.Configuration;

namespace WebForms.App_Code
{
    public static class AppInstrumentatins
    {
        private static IDisposable tracerProvider;
        private static IDisposable meterProvider;

        public static void Configure()
        {
            var otlpEndpoint = new Uri(ConfigurationManager.AppSettings["OtlpEndpoint"]);

            // This is required if the collector doesn't expose an https endpoint as .NET by default
            // only allow http2 (required for gRPC) to secure endpoints
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var tracerBuilder = Sdk.CreateTracerProviderBuilder()
                 .AddAspNetInstrumentation()
                 .AddHttpClientInstrumentation();

            tracerBuilder
                .AddSource(Source.ServiceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: Source.ServiceName, serviceVersion: Source.ServiceVersion))
                .AddHttpClientInstrumentation()
                .AddOtlpExporter((exporterOptions) =>
                {
                    exporterOptions.Endpoint = otlpEndpoint;
                    exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                });

            var meterBuilder = Sdk.CreateMeterProviderBuilder();
                 // .AddAspNetInstrumentation(); Not in shipped

            meterBuilder
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: Source.ServiceName, serviceVersion: Source.ServiceVersion))
                .AddMeter(Source.ServiceName)
                .AddHttpClientInstrumentation()
                .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                {
                    exporterOptions.Endpoint = otlpEndpoint;
                    exporterOptions.Protocol = OtlpExportProtocol.Grpc;

                    metricReaderOptions.MetricReaderType = MetricReaderType.Periodic;
                    metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
                    metricReaderOptions.Temporality = AggregationTemporality.Cumulative;
                });

            tracerProvider = tracerBuilder.Build();
            meterProvider = meterBuilder.Build();
        }

        public static void Release()
        {
            tracerProvider.Dispose();
            meterProvider.Dispose();
            tracerProvider = null;
            meterProvider = null;
        }
    }
}
