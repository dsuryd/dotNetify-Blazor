## Overview

DotNetify makes it super easy to connect your [Blazor WebAssembly](https://docs.microsoft.com/en-us/aspnet/core/blazor/) client app to the server in a declarative, real-time and reactive manner. It uses [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/) to communicate with the server through an MVVM-styled abstraction, but unlike Blazor Server, you get to choose what is sent over the network and when.

DotNetify is a good fit for building complex web applications that requires clear separation of concerns between client-side UI, presentation, and domain logic for long-term maintainability and extensibility. Plus, its integration with SignalR allows you to easily implement asynchronous data updates to multiple clients in real-time.

#### Features

DotNetify's design is based on the principle of maintaining strong separation of concerns that places business or data-driven logic firmly on the back-end, putting the front-end only in charge of UI/UX concerns. Some of the things dotNetify support:

- <b>Server-side view model</b>: don't let your client download too much code; keep most processing on the back-end and only send things that change.

- <b>Declarative state hydration</b>: eliminate the need to write data-fetching boilerplate services. Send data to the back-end by simply invoking an interface method.

- <b>Simple real-time abstraction</b>: push data to client in real-time from multiple classes with no coupling to low-level SignalR details.

- **[Scoped CSS](scopedcss)**: Blazor's native CSS isolation is still in the future, but don't let that stop you from enjoying it now!

- **[Web Components](https://dotnetify.net/elements?webcomponent)**: comes with a library of HTML native web components called `DotNetify-Elements` which makes it very convenient to implement layouts, online forms, charts, and more --- all supporting scoped CSS.

- **[Reusable with Javascript SPAs](https://github.com/dsuryd/dotNetify/tree/master/Demo)**: Can't always use Blazor? The same view models you write for Blazor can be reused with Javascript UI frameworks without change. DotNetify has full support for React and Vue, and can be made to work with Angular and others.

- **[Reusable with .NET desktop clients](https://github.com/dsuryd/dotNetify/tree/master/Demo/DotNetClient)**: reuse the same view models with .NET-based client apps using WPF or Avalonia.

- **[Multicasting](multicast)**: send real-time data to multiple clients at once; perfect for real-time collaboration / data synchronization.

- **[Reactive]()**: make your view model declarative with streaming, observable properties. Supports asynchronous programming.

- **[Dependency injection](di)**: inject dependency objects through the class constructor.

- **[Middlewares /](middleware) [filters](filter)**: build a pipeline to do all sorts of things before reaching the view models.

- **[Bearer token authentication](security)**: pass authentication header as payload instead of query string.

- **[Can switch to Web API](webapimode)**: don't need real-time and don't want to use SignalR? Keep your view model stateless and switch to use built-in Web API endpoint instead.
