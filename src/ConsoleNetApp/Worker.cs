using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ConsoleNetApp
{
    public class Worker : BackgroundService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHostApplicationLifetime hostLifetime;
        private readonly ActivitySource activitySource;
        private readonly ILogger<Worker> logger;

        public Worker(
            IHttpClientFactory httpClientFactory,
            IHostApplicationLifetime hostLifetime,
            ActivitySource activitySource,
            ILogger<Worker> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.hostLifetime = hostLifetime;
            this.activitySource = activitySource;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Begin worker execution ...");

            // if you wait some time here - first call to StartActivity will return Activity
            // if not, result of call to the StartActivity is null
            // await Task.Delay(1000);

            var client = httpClientFactory.CreateClient("MyClient");
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var activity = activitySource.StartActivity("my.console.woker.activity"))
                {
                    var currentActivity = Activity.Current;

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
