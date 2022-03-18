using MvcNetCoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.ServiceModel;
using OpenTelemetry.Contrib.Instrumentation.Wcf;

namespace MvcNetCoreApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ActivitySource _activitySource;
        private readonly Counter<int> _counter;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, ActivitySource activitySource, Counter<int> counter, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _activitySource = activitySource;
            _counter = counter;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> IndexAsync()
        {
            // this things can be done with ActionFilter or Global Filters
            _counter.Add(1);

            // this things can be done with ActionFilter or Global Filters
            if (Activity.Current?.IsAllDataRequested == true)
            {
                Activity.Current?.AddTag("Tag1", 1.2f);
            }

            await Task.Delay(1000);
            using var activity1 = _activitySource.StartActivity("MyActivity 2", ActivityKind.Internal);

            if (activity1?.IsAllDataRequested == true)
            {
                activity1?.AddEvent(new ActivityEvent("My event 2...", default, new ActivityTagsCollection() { { "tag1", "tag val 11" } }));
            }

            await Task.Delay(500);

            return View();
        }

        public async Task<IActionResult> PrivacyAsync()
        {
            // this things can be done with ActionFilter or Global Filters
            if (Activity.Current?.IsAllDataRequested == true)
            {
                Activity.Current?.AddTag("Tag2", "test tag 2 value");
            }

            var client = _httpClientFactory.CreateClient("MyClient");
            var weather = await client.GetStringAsync("/WeatherForecast").ConfigureAwait(false);

            using var activity1 = _activitySource.StartActivity("MyActivity", ActivityKind.Internal);

            activity1?.AddEvent(new ActivityEvent("My event 1...", DateTimeOffset.Now, new ActivityTagsCollection() { { "tag1", "tag val 11" } }));

            var wcfClient = new ServiceReference1.TestServiceClient(
                new BasicHttpsBinding(BasicHttpsSecurityMode.Transport),
                new EndpointAddress("https://localhost:44322/TestService.svc"));
            wcfClient.Endpoint.EndpointBehaviors.Add(new TelemetryEndpointBehavior());

            await wcfClient.GetDataAsync(100).ConfigureAwait(false);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}