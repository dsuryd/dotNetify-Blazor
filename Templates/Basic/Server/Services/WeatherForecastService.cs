using System;
using System.Linq;
using System.Threading.Tasks;
using Basic.Shared;

namespace Basic.Server
{
   public class WeatherForecastService
   {
      private static readonly string[] Summaries = new[]
      {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

      public Task<WeatherForecast[]> GetAsync()
      {
         var rng = new Random();
         var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
         {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
         })
         .ToArray();

         return Task.FromResult(weatherForecasts);
      }
   }
}