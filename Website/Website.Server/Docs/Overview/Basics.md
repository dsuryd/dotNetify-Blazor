## Basics

In the most basic form, you can just use dotNetify to quickly fetch from the server the initial state of your Blazor component. You do this by nesting your component's HTML inside a **VMContext** component. Use its **VM** attribute value to specify the name of the C# view model class that will provide the state. Add a public interface to define what that state is, and assign the type to the **TState** atribute.

```jsx
<VMContext VM="HelloWorld" TState="IHelloWorldState" OnStateChange="UpdateState">
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
<VMContext VM="HelloWorld" TState="IHelloWorldState" OnStateChange="UpdateState">
    <div>
      <p>@state?.Greetings</p>
      <p>Server time is: @state?.ServerTime</p>
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

We added two new server APIs, **Changed** that accepts the name of the property we want to update, and **PushUpdates** to do the actual push of all changed properties to the front-end.

#### Server Update

At some point in your app, you probably want to send data back to the server to be persisted. Let's add to this example something to submit:

```jsx
<VMContext VM="ServerUpdate" TState="IServerUpdateState" OnStateChange="UpdateState">
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
   public void Submit(Person person)
   {
      Greetings = $"Hello {person.FirstName} {person.LastName}!";
      Changed(nameof(Greetings));
   };
}
```

Notice the `Submit` method on the client-side interface, which serves as proxy to the method of the same name in the view model. When that method is called, the argument will be dispatched to the server and used to invoke the server method. After the action modifies the Greetings property, it marks it as changed so that the new text will get sent to the front-end.

Notice that we don't need to use **PushUpdates** to get the new greetings sent. That's because dotNetify employs something similar to the request-response cycle, but in this case it's _action-reaction cycle_: the front-end initiates an action that mutates the state, the server processes the action and then sends back to the front-end any other states that mutate as the reaction to that action.

#### Asynchronous Methods

If you need to do an asynchronous method call in order to initialize the view model, override the **OnCreatedAsync** method and await the call there:

```csharp
public class HelloWorld: BaseVM
{
   public string Greetings { get; set; }

   public override async Task OnCreatedAsync()
   {
      Greetings = await BuildGreetingsAsync();
   }
}
```

Action methods like the _Submit_ method in the previous example can also be made awaitable by changing the return type to _Task_:

```csharp
   public async Task Submit(Person person)
   {
      await SomeAsyncMethod(person);
   }
```

#### Object Lifetime

The server classes are referred to as view models. Whereas a model represents your app's business domain, a view model is an abstraction of a view, which embodies data and behavior necessary for the view to fulfill its use cases.

View-models gets their model data from the back-end service layer and shape them to meet the needs of the views. This enforces separation of concerns, where you can keep the views dealing only with UI presentation and leave all data-driven operations to the view models. It's great for testing too, because you can test your use case independent of any UI concerns.

View model objects stay alive on the server until the browser page is closed, reloaded, navigated away, or the session times out.

#### Two-Way Binding

To keep the properties in the client in sync with the server without explicit method call, add the **[Watch]** attribute on the interface property:

```jsx
<VMContext VM="HelloWorld" TState="IHelloWorldState" OnStateChange="UpdateState">
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
