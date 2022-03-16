using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ConsoleNetApp
{
    public static class AppInstrumentatins
    {
        public static void AddAppInstrumentatins(this IServiceCollection services)
        {
            services.AddSingleton(Source.ActivitySource);

            // This is required if the collector doesn't expose an https endpoint as .NET by default
            // only allow http2 (required for gRPC) to secure endpoints
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddOpenTelemetryTracing(builder =>
            {
                builder
                .AddSource(Source.ServiceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: Source.ServiceName, serviceVersion: Source.ServiceVersion))
                // .SetSampler(new AlwaysOnSampler())
                .AddHttpClientInstrumentation()
                .AddOtlpExporter((exporterOptions) =>
                {
                    exporterOptions.Endpoint = new Uri("http://localhost:4317");
                    exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                });
            });
        }
    }
}
