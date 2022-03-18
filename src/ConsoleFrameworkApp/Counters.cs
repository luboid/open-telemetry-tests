using System.Diagnostics.Metrics;

namespace ConsoleFrameworkApp
{
    public static class Counters
    {
        public static readonly Counter<int> NumberCycles
            = Source.Meter.CreateCounter<int>(Source.ServiceName + "_cycles", "number", "Number of cycles.");
    }
}
