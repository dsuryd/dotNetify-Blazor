using System.Threading.Tasks;
using Basic.Shared;
using DotNetify;

namespace Basic.Server
{
   public class FetchData : BaseVM, IFetchDataState
   {
      private readonly WeatherForecastService weatherForecastService;

      public WeatherForecast[] Forecasts { get; set; }

      public FetchData(WeatherForecastService weatherForecastService)
      {
         this.weatherForecastService = weatherForecastService;
      }

      public override async Task OnCreatedAsync()
      {
         Forecasts = await weatherForecastService.GetAsync();
      }
   }
}