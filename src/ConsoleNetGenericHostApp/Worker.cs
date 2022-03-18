using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ConsoleNetGenericHostApp
{
    public class Worker : BackgroundService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHostApplicationLifetime hostLifetime;
        private readonly ActivitySource activitySource;
        private readonly Counter<int> counter;
        private readonly ILogger<Worker> logger;

        public Worker(
            IHttpClientFactory httpClientFactory,
            IHostApplicationLifetime hostLifetime,
            ActivitySource activitySource,
            Counter<int> counter,
            ILogger<Worker> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.hostLifetime = hostLifetime;
            this.activitySource = activitySource;
            this.counter = counter;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Begin worker execution ...");

            var client = httpClientFactory.CreateClient("MyClient");
            while (!stoppingToken.IsCancellationRequested)
            {
                counter.Add(1);

                using (var activity = activitySource.StartActivity("my.console.woker.activity"))
                {
                    var activity_3 = Activity.Current;

                    activity?.SetTag("my.console.tag", 10);

                    var weather = await client.GetStringAsync("/WeatherForecast").ConfigureAwait(false);
                }

                await Task.Delay(1000);
            }

            logger.LogInformation("End worker execution ...");
            hostLifetime.StopApplication();
        }
    }
}
