using System.Diagnostics.Metrics;

namespace WcfService.Tools
{
    public static class Counters
    {
        public static readonly Counter<int> GetDataCalls
            = Source.Meter.CreateCounter<int>(Source.ServiceName + "_GetData", "number", "Number of GetDatas.");
    }
}
