using System;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetify.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Basic.Client
{
   public class Program
   {
      public static async Task Main(string[] args)
      {
         var builder = WebAssemblyHostBuilder.CreateDefault(args);
         builder.RootComponents.Add<App>("app");

         builder.Services.AddDotNetifyBlazor(config => config.Debug = true);
         builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

         await builder.Build().RunAsync();
      }
   }
}