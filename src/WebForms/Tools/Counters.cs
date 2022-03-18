using System.Diagnostics.Metrics;

namespace WebForms.Tools
{
    public static class Counters
    {
        public static readonly Counter<int> NumberAbout
            = Source.Meter.CreateCounter<int>(Source.ServiceName + "_about", "number", "Number of abouts.");
    }
}
