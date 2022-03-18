using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleFrameworkApp
{
    public static class WeatherForecast
    {
        public static async Task<string> GetAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7092");
                return await client.GetStringAsync("/WeatherForecast").ConfigureAwait(false);
            }
        }

        public static string Get()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7092");
                return client.GetStringAsync("/WeatherForecast").Result;
            }
        }
    }
}
