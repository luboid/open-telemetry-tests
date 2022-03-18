namespace ConsoleNetApp
{
    public static class WeatherForecast
    {
        public static async Task<string> GetAsync(CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7092");
                return await client.GetStringAsync("/WeatherForecast", cancellationToken).ConfigureAwait(false);
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
