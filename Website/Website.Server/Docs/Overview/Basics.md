## Basics

In the most basic form, you can just use dotNetify to quickly fetch from the server the initial state of your Blazor component. You do this by nesting your component's HTML inside a **VMContext** component, and specify its **VM** attribute value with the name of the C# view model class that will provide the state. Add a public interface to define what that state is.

```jsx
<VMContext VM="HelloWorld" OnStateChange="(IHelloWorldState state) => UpdateState(state)">
    <div>@state?.Greetings</div>
</VMContext>

@code {
    private IHelloWorldState state;

    public interface IHelloWorldState
    {
        string Greetings { get; set; }
    }

    private void UpdateState(IHelloWorldState state)
    {
      this.state = state;
      StateHasChanged();
    }
```

```csharp
public class HelloWorld : BaseVM
{
   public string Greetings => "Hello World!";
}
```

Write a C# class that inherits from **BaseVM** in your ASP.NET project, and add public properties matching the state interface you defined. When the connection is established, the class instance will be serialized and sent as the initial state for the component.

#### Real-Time Push

With very little effort, you can make your app gets real-time data update from the server:

```jsx
<VMContext VM="HelloWorld" OnStateChange="(IHelloWorldState state) => UpdateState(state)">
    <div>
      <p>@state?.Greetings</p>
      <p>Server time is: @state?ServerTime</p>
    </div>
</VMContext>

@code {
    private IHelloWorldState state;

    public interface IHelloWorldState
    {
        string Greetings { get; set; }
        string ServerTime { get; set; }
    }

    private void UpdateState(IHelloWorldState state)
    {
      this.state = state;
      StateHasChanged();
    }
}
```

```csharp
public class HelloWorld : BaseVM
{
   private Timer _timer;
   public string Greetings => "Hello World!";
   public DateTime ServerTime => DateTime.Now;

   public HelloWorld()
   {
      _timer = new Timer(state =>
      {
         Changed(nameof(ServerTime));
         PushUpdates();
      }, null, 0, 1000); // every 1000 ms.
   }
   public override void Dispose() => _timer.Dispose();
}
```

[inset]
We added two new server APIs, **Changed** that accepts the name of the property we want to update, and **PushUpdates** to do the actual push of all changed properties to the front-end.

#### Server Update

At some point in your app, you probably want to send data back to the server to be persisted. Let's add to this example something to submit:

```jsx
<VMContext VM="ServerUpdate" OnStateChange="(IServerUpdateState state) => UpdateState(state)">
    <div>
        <p>@state?.Greetings</p>
        <input type="text" bind="@person.FirstName" />
        <input type="text" bind="@person.LastName" />
        <button onclick="@HandleSubmit">Submit</button>
    </div>
</VMContext>

@code {
    private IServerUpdateState state;
    private Person person = new Person();

    public interface IServerUpdateState
    {
        string Greetings { get; set; }
        void Submit(Person person);
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    private void HandleSubmit() => state.Submit(person);

    private void UpdateState(IServerUpdateState state)
    {
      this.state = state;
      StateHasChanged();
    }
}
```

```csharp
public class HelloWorld : BaseVM
{
   public class Person
   {
      public string FirstName { get; set; }
      public string LastName { get; set; }
   }

   public string Greetings { get; set; } = "Hello World!";
   public Action<Person> Submit => person =>
   {
      Greetings = $"Hello {person.FirstName} {person.LastName}!";
      Changed(nameof(Greetings));
   };
}
```

Notice the `Submit` method on the client-side interface, which serves as proxy to the method of the same name in the view model. When that method is called, the argument will be dispatched to the server and used to invoke the server method. After the action modifies the Greetings property, it marks it as changed so that the new text will get sent to the front-end.

Notice that we don't need to use **PushUpdates** to get the new greetings sent. That's because dotNetify employs something similar to the request-response cycle, but in this case it's _action-reaction cycle_: the front-end initiates an action that mutates the state, the server processes the action and then sends back to the front-end any other states that mutate as the reaction to that action.

#### Object Lifetime

The server classes are referred to as view models. Whereas a model represents your app's business domain, a view model is an abstraction of a view, which embodies data and behavior necessary for the view to fulfill its use cases.

View-models gets their model data from the back-end service layer and shape them to meet the needs of the views. This enforces separation of concerns, where you can keep the views dealing only with UI presentation and leave all data-driven operations to the view models. It's great for testing too, because you can test your use case independent of any UI concerns.

View model objects stay alive on the server until the browser page is closed, reloaded, navigated away, or the session times out.

#### Two-Way Binding

To keep the properties in the client in sync with the server without explicit method call, add the **[Watch]** attribute on the interface property:

```jsx
<VMContext VM="HelloWorld" OnStateChange="(IHelloWorldState state) => UpdateState(state)">
  <div>
    <p>@state?.Greetings</p>
    <input type="text" bind="@state.Name" />
  </div>
</VMContext>

@code {
    private IHelloWorldState state;
    private Person person = new Person();

    public interface IHelloWorldState
    {
        string Greetings { get; set; }
        [Watch] string Name { get; set; }
    }

    private void UpdateState(IServerUpdateState state)
    {
      this.state = state;
      StateHasChanged();
    }
```

```csharp
public class HelloWorld : BaseVM
{
  public string Greetings => $"Hello {Name}";

  private string _name = "World";
  public string Name
  {
    get => _name;
    set
    {
      _name = value;
      Changed(nameof(Greetings));
    }
  }
}
```

[inset]

#### Scoped CSS

Blazor native support for component-scoped CSS isn't here yet, but these steps will allow you to have it:

1. Create a separate css file for you component, e.g. _MyComponent.razor.css_.
2. Set the _Build Action_ property of that file to <b>Embedded Resource</b>.
3. Inside your component's razor file, inject **IStylesheet** service from _DotNetify.Blazor_ namespace.
4. Nest the component's HTML under a `d-panel` web component from `dotNetify-Elements`.
5. Use the indexer of _IStylesheet_ to access the css content of the file by its file name (it uses submatching, so name can be partial), and pass the string to the _css_ attribute of `d-panel`.

```jsx
@inject IStylesheet Stylesheet

<VMContext VM="HelloWorld">
  <d-panel css='@Stylesheet["HelloWorld"]'>
    @state.Greetings
  </d-panel>
</VMContext>
```

At runtime you will see that a CSS class with a random name is added to the `<head>` tag, and set on `d-panel` element, which makes the style exclusive to that element and its children. Furthermore, the light CSS preprocessor used to generate it supports nesting syntax. For example: `& { .some-class { &:hover { color: blue; } } }`.

#### API Essentials

##### Client APIs

**VMContext** component attributes:

- **VM**: the name of the view model type to connect to.
- **Options**: connect options, used for passing initial values, authentication headers, or switching to Web API mode.
- **OnStateChange**: callback when a new state is received from the server.
- **OnElementEvent**: event callback when using web components from _Elements_.

**IStylesheet**: use its indexer to access the CSS embedded resource string.

##### Server APIs

On the server, inherit from **BaseVM** and use:

- **Changed**(_propName_)
- **PushUpdates**(): for real-time push.

Optionally, use these property accessor/mutator helper **Get/Set** to make your code more concise:

```csharp
public string Greetings
{
   get => Get<string>();
   set => Set(value);
}
// Above property is equivalent to this:
private string _greetings;
public string Greetings
{
   get { return _greetings; }
   set { _greetings = value; Changed(nameof(Greetings)); }
}
```
