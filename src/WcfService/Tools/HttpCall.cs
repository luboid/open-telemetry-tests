using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WcfService.Tools
{
    public static class HttpCall
    {
        public static string GetWeatherForecast()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7092");
                return client.GetStringAsync("/WeatherForecast").Result;
            }
        }

        public static async Task<string> GetWeatherForecastAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7092");
                return await client.GetStringAsync("/WeatherForecast").ConfigureAwait(false);
            }
        }

        public static void RequestGoogleHomPageViaHttpClient()
        {
            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync("http://www.google.com").Result)
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public static void RequestGoogleHomPageViaHttpWebRequestLegacySync()
        {
            var request = WebRequest.Create("http://www.google.com/?sync");
            using (var response = request.GetResponse()) { }
        }
    }
}
