using DotNetify;
using DotNetify.Pulse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Website.Server
{
   public class Startup
   {
      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddCors(options => options.AddPolicy(name: "CorsPolicy", builder => builder
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

         services.AddSignalR();
         services.AddDotNetify();
         services.AddDotNetifyPulse();

         services.AddLogging();
         services.AddTransient<ILiveDataService, MockLiveDataService>();
         services.AddScoped<ICustomerRepository, CustomerRepository>();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
      {
         app.UseCors("CorsPolicy");

         app.UseWebSockets();
         app.UseDotNetify(config =>
        {
           config.UseDeveloperLogging(log =>
           {
              if (!log.Contains("vmId=PulseVM"))
                 logger.LogInformation(log);
           });
           config.RegisterAssembly(GetType().Assembly);
           config.RegisterAssembly("DotNetify.DevApp.ViewModels");
        });

         // Real-time logging: http://localhost:8090/blazor/pulse.
         app.UseDotNetifyPulse();

         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
         }

         app.UsePathBase("/" + MainNav.PATH_BASE);
         app.UseBlazorFrameworkFiles();
         app.UseStaticFiles();

         app.UseRouting();
         app.UseEndpoints(endpoints =>
         {
            endpoints.MapHub<DotNetifyHub>("/dotnetify");
            endpoints.MapFallbackToFile("index.html");
         });
      }
   }
}