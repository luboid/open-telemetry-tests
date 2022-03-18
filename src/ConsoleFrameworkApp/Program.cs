using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleFrameworkApp
{
    internal static class Program
    {
        static Program()
        {
            AppInstrumentatins.Configure();
        }

        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                cts.Cancel();
            };

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    Counters.NumberCycles.Add(1);

                    using (var activity = Source.ActivitySource.StartActivity("my.console.woker.activity"))
                    {
                        activity?.SetTag("my.console.tag", 10);

                        await WeatherForecast.GetAsync();
                    }

                    await Task.Delay(1000, cts.Token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press Enter to exit ...");
            Console.ReadLine();
        }
    }
}
