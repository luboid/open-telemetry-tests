using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ConsoleNetApp
{
    public static class Source
    {
        public static readonly string ServiceName = ApplicationInformation.Name;
        public static readonly string ServiceVersion = ApplicationInformation.Version.ToString();

        public static readonly ActivitySource ActivitySource = new ActivitySource(ServiceName, ServiceVersion);
        public static readonly Meter Meter = new Meter(ServiceName, ServiceVersion);
    }
}
