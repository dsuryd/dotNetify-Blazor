## Get Started

#### Server Setup

Download the NuGet package **DotNetify.SignalR**, then add the following in the _Startup.cs_:

```csharp
public void ConfigureServices(IServiceCollection services)
{
   services.AddSignalR();
   services.AddDotNetify();
   ...
}
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
   app.UseWebSockets();
   app.UseDotNetify();

   app.UseRouting();
   app.UseEndpoints(endpoints => endpoints.MapHub<DotNetifyHub>("/dotnetify"));
   ...
}
```

<br/>

The default configuration assumes all view model classes are in the web project, and uses built-in .NET dependency injection. To override the configuration:

```csharp
app.UseDotNetify(config => {
    config.RegisterAssembly(/* name of the assembly where the view model classes are located */);
    config.SetFactoryMethod((type, args) => /* let your favorite IoC library creates the view model instance */);
});
```

#### Client Setup

Download the NuGet package **DotNetify.Blazor**, then add the following to the `index.html`:

```html
<script src="https://unpkg.com/react@16/umd/react.production.min.js"></script>
<script src="https://unpkg.com/react-dom@16/umd/react-dom.production.min.js"></script>
<script src="https://unpkg.com/styled-components@4.1/dist/styled-components.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/@aspnet/signalr@1.1.2/dist/browser/signalr.min.js"></script>
<script src="https://unpkg.com/dotnetify@latest/dist/dotnetify-react.min.js"></script>
<script src="https://unpkg.com/dotnetify-elements@latest/lib/dotnetify-elements.bundle.js"></script>
<script src="_content/DotNetify.Blazor/dotnetify-blazor.js"></script>

<script src="_framework/blazor.webassembly.js"></script>
```
