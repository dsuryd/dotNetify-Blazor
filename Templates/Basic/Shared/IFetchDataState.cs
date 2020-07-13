namespace Basic.Shared
{
   public interface IFetchDataState
   {
      WeatherForecast[] Forecasts { get; set; }
   }
}