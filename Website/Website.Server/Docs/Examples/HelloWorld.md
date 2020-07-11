<if view>
##### HelloWorld.razor

```jsx
<VMContext VM="HelloWorldVM" TState="IHelloWorldState" OnStateChange="UpdateState">
@if (state != null)
{
    <Stylesheet Context="this">
        <section>
            <div>
                <label>First name:</label>
                <input type="text" class="form-control" @bind="state.FirstName">
            </div>
            <div>
                <label>Last name:</label>
                <input type="text" class="form-control" @bind="state.LastName">
            </div>
        </section>
        <div>
            Full name is <b>@state.FullName</b>
        </div>
    </Stylesheet>
}
</VMContext>

@code {
   private IHelloWorldState state;

   public interface IHelloWorldState
   {
       [Watch] string FirstName { get; set; }
       [Watch] string LastName { get; set; }
       string FullName { get; set; }
   }

   private void UpdateState(IHelloWorldState state)
   {
       this.state = state;
       StateHasChanged();
   }
}
```

</if>
<if viewmodel>
##### HelloWorld.cs

```csharp
public class HelloWorldVM : BaseVM
{
    private string _firstName = "Hello";
    private string _lastName = "World";

    public string FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value;
            Changed(nameof(FullName));
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            Changed(nameof(FullName));
        }
    }

    public string FullName => $"{FirstName} {LastName}";
}
```

</if>
