using System.Diagnostics.Metrics;

namespace WebForms.App_Code
{
    public static class Counters
    {
        public static readonly Counter<int> NumberEnters
            = Source.Meter.CreateCounter<int>(Source.ServiceName + "_enter", "number", "Number of enters.");
    }
}
