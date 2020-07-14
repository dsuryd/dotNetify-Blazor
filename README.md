<p align="center"><img width="300px" src="http://dotnetify.net/content/images/dotnetify-logo.png"></p>

[![NuGet version](https://badge.fury.io/nu/DotNetify.Blazor.svg)](https://badge.fury.io/nu/DotNetify.Blazor)

**DotNetify-Blazor** is a free, open source project that lets you create real-time, reactive, cross-platform apps with Blazor WebAssembly.

## Features

- <b>Server-side view model</b>: don't let your client download too much code; keep most processing on the back-end and only send things that change.
- <b>Declarative state hydration</b>: eliminate the need to write data-fetching boilerplate services. Send data to the back-end by simply invoking an interface method.
- <b>Simple real-time abstraction</b>: push data to client in real-time from multiple classes with no coupling to low-level SignalR details.
- **Can switch to Web API**: don't need real-time/SignalR? Keep your view model stateless and switch to Web API endpoint instead.
- **Scoped CSS**: Blazor's native CSS isolation is still in the future, but don't let that stop you from enjoying it now!
- **[Web Components](https://dotnetify.net/elements?webcomponent)**: comes with a library of HTML native web components to implement layouts, online forms, charts, and more --- all supporting CSS isolation. Usage is optional!
- **Reusable with Javascript SPAs**: Can't always use Blazor? The same view models you write for Blazor can be reused with Javascript UI frameworks without change. DotNetify has full support for React and Vue, and can be made to work with Angular and others.
- **Reusable with .NET desktop clients**: reuse the same view models with .NET-based client apps (WPF/Avalonia).
- **Multicasting**: send real-time data to multiple clients at once; perfect for real-time collaboration/data sync.
- **Reactive**: make your view model declarative with streaming, observable properties + asynch programming.
- **Dependency injection**: inject dependency objects through the class constructor.
- **Middlewares/filters**: build a pipeline to do all sorts of things before reaching the view models.
- **Bearer token authentication**: pass authentication header as payload instead of query string.

## Install

```
dotnet add package DotNetify.Blazor
```

## Documentation

Documentation and live demo can be found at [https://dotnetify.net/blazor](https://dotnetify.net/blazor).

DotNetify core repo: [https://github.com/dsuryd/dotNetify](https://github.com/dsuryd/dotNetify).

## License

Licensed under the Apache License, Version 2.0.

## Contributing

All contribution is welcome: star this project, let others know about it, report issues, submit pull requests!

_Logo design by [area55git](https://github.com/area55git)._
