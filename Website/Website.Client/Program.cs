using System;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetify.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Website.Client
{
   public class Program
   {
      public static async Task Main(string[] args)
      {
         var builder = WebAssemblyHostBuilder.CreateDefault(args);

         builder.Services
            .AddDotNetifyBlazor(config =>
            {
#if DEBUG
               config.Debug = true;
#endif
            })
            .AddTransient(_ => new HttpClient
            {
               BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

         builder.RootComponents.Add<App>("app");
         await builder.Build().RunAsync();
      }
   }
}