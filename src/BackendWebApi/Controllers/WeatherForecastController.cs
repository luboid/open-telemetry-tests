using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BackendWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Counter<int> _counter;
        private readonly ActivitySource _activitySource;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, Counter<int> counter, ActivitySource activitySource)
        {
            _logger = logger;
            _counter = counter;
            _activitySource = activitySource;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            // all this can be done in ActionFilters,GlobalFilters
            // increment calls
            _counter.Add(1);

            // follow to next service in the call chain
            Activity.Current?.AddBaggage("userid", "10101010");

            // add other instrumentation data
            if (Activity.Current?.IsAllDataRequested == true)
            {
                Activity.Current?.SetTag("tag1", 100);
                Activity.Current?.AddEvent(new ActivityEvent("event_100", DateTimeOffset.Now, new ActivityTagsCollection()
                {
                    { "tag1", "123456" },
                    { "tag2", "795867" }
                }));
            }

            using var activity = _activitySource.StartActivity("My_Weather_Activity");

            activity?.SetTag("MyWeatherTag", 100m);

            return Enumerable.Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}