using Restaurant.Shared;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Restaurant.Infrastructure.Grpc.Services
{
    public class WeatherService : WeatherForecasts.WeatherForecastsBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public override Task<WeatherReply> GetWeather(WeatherForecast request, ServerCallContext context)
        {
            var reply = new WeatherReply();
            var rng = new Random();

            reply.Forecasts.Add(Enumerable.Range(1, 10).Select(index => new WeatherForecast
            {
                Date = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(index)),
                TemperatureC = rng.Next(20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }));

            return Task.FromResult(reply);
        }
    }
}