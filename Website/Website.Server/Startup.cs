using System;
using System.Text;
using DotNetify;
using DotNetify.Pulse;
using DotNetify.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Website.Server
{
   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddCors(options => options.AddPolicy(name: "CorsPolicy", builder => builder
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

         // Add OpenID Connect server to produce JWT access tokens.
         services.AddAuthenticationServer();

         services.AddSignalR();
         services.AddDotNetify();
         services.AddDotNetifyPulse();

         // Configure intergration with an external websocket server.
         if (!string.IsNullOrWhiteSpace(Configuration["WSServer:ConnectionUrl"]))
            services.AddDotNetifyIntegrationWebApi(client => client.BaseAddress = new Uri(Configuration["WSServer:ConnectionUrl"]));

         services.AddMvc();
         services.AddLogging();
         services.AddScoped<ILiveDataService, MockLiveDataService>();
         services.AddScoped<ICustomerRepository, CustomerRepository>();
         services.AddScoped<IEmployeeRepository, EmployeeRepository>();
         services.AddScoped<IMovieService, MovieService>();
         services.AddScoped<IWebStoreService, WebStoreService>();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
      {
         app.UseForwardedHeaders(new ForwardedHeadersOptions
         {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
         });

         app.UseCors("CorsPolicy");
         app.UseAuthentication();

         app.UseWebSockets();
         app.UseDotNetify(config =>
        {
           var tokenValidationParameters = new TokenValidationParameters
           {
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthServer.SecretKey)),
              ValidateIssuerSigningKey = true,
              ValidateAudience = false,
              ValidateIssuer = false,
              ValidateLifetime = true,
              ClockSkew = TimeSpan.FromSeconds(0)
           };

           // Middleware to do authenticate token in incoming request headers.
           config.UseJwtBearerAuthentication(tokenValidationParameters);

           // Filter to check whether user has permission to access view models with [Authorize] attribute.
           config.UseFilter<AuthorizeFilter>();

           // Middleware to log incoming/outgoing message; default to Sytem.Diagnostic.Trace.
           config.UseDeveloperLogging(log =>
           {
              if (!log.Contains("vmId=PulseVM"))
                 logger.LogInformation(log);
           });

           // Demonstration middleware that extracts auth token from incoming request headers.
           config.UseMiddleware<ExtractAccessTokenMiddleware>(tokenValidationParameters);

           // Demonstration filter that passes access token from the middleware to the ViewModels.SecurePageVM class instance.
           config.UseFilter<SetAccessTokenFilter>();

           // Register all view models in this assembly.
           config.RegisterAssembly(GetType().Assembly);
        });

         // Real-time logging: <url>/pulse.
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
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
         });
      }
   }
}