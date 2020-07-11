<if view>
##### ControlTypes.razor

```jsx
@inject IStylesheet Stylesheet

<VMContext VM="ControlTypesVM" OnStateChange="(IControlTypesState state) => UpdateState(state)">
@if (state != null)
{
    <Stylesheet Name="ControlTypes">
        <table>
            <tbody>
                <tr>
                    <td>
                        Text box:
                        <label>(updates on losing focus)</label>
                    </td>
                    <td>
                        <input type="text"
                                class="form-control"
                                @bind="state.TextBoxValue"
                                placeholder="@state.TextBoxPlaceHolder">
                        <b>@state.TextBoxResult</b>
                    </td>
                </tr>
                <tr>
                    <td>
                        Search box:
                        <label>(updates on keystroke)</label>
                    </td>
                    <td>
                        <input type="text"
                                class="form-control"
                                @bind="state.SearchBox"
                                @bind:event="oninput"
                                placeholder="@state.SearchBoxPlaceHolder">
                        <ul class="list-group">
                            @foreach (var result in state.SearchResults)
                            {
                                <li class="list-group-item"
                                    @key="result"
                                    @onclick="_ => { state.SearchBox = result; }">
                                    @result
                                </li>
                            }
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>Check box:</td>
                    <td>
                        <label>
                            <input type="checkbox" @bind="state.ShowMeCheckBox">
                            <span>Show me</span>
                        </label>
                        <label>
                            <input type="checkbox" @bind="state.EnableMeCheckBox">
                            <span>Enable me</span>
                        </label>
                        @if (state.ShowMeCheckBox)
                        {
                            <button class="btn btn-secondary"
                                    disabled="@(!state.EnableMeCheckBox)">
                                @state.CheckBoxResult
                            </button>
                        }
                    </td>
                </tr>
                <tr>
                    <td>Simple drop-down list:</td>
                    <td>
                        <select class="form-control" @bind="state.SimpleDropDownValue">
                            <option value="" disabled>Choose...</option>
                            @foreach (var option in state.SimpleDropDownOptions)
                            {
                                <option @key="option" value="@option">@option</option>
                            }

                        </select>
                        <b>@state.SimpleDropDownResult</b>
                    </td>
                </tr>
                <tr>
                    <td>Drop-down list:</td>
                    <td>
                        <select class="form-control" @bind="state.DropDownValue">
                            <option value="0" disabled>@state.DropDownCaption</option>
                            @foreach(var option in state.DropDownOptions)
                            {
                                <option @key="option.Id" value="@option.Id">@option.Text</option>
                            }
                        </select>
                        <b>@state.DropDownResult</b>
                    </td>
                </tr>
                <tr>
                    <td>Radio button:</td>
                    <td>
                        <label>
                            <input type="radio"
                                    value="green"
                                    checked="@state.RadioButtonValue.Equals("green")"
                                    @onchange="@(_ => state.RadioButtonValue = "green")">
                            <span>Green</span>
                        </label>
                        <label>
                            <input type="radio"
                                    value="yellow"
                                    checked="@state.RadioButtonValue.Equals("yellow")"
                                    @onchange="@(_ => state.RadioButtonValue = "yellow")">
                            <span>Yellow</span>
                        </label>
                        <button class="btn @state.RadioButtonStyle">Result</button>
                    </td>
                </tr>
                <tr>
                    <td>Button:</td>
                    <td>
                        <button class="btn btn-secondary"
                                type="button"
                                @onclick="_ => state.ButtonClicked(true)">
                            Click me
                        </button>
                        @if (state.ClickCount > 0)
                        {
                        <span style="margin-left: 2rem">
                            You clicked me
                            <b>@state.ClickCount</b>&nbsp;times!
                        </span>
                        }
                    </td>
                </tr>
            </tbody>
        </table>
    </Stylesheet>
}
</VMContext>

@code {
   private IControlTypesState state;

   public interface IControlTypesState
   {
       [Watch] string TextBoxValue { get; set; }
       string TextBoxPlaceHolder { get; set; }
       string TextBoxResult { get; set; }

       [Watch] string SearchBox { get; set; }
       string SearchBoxPlaceHolder { get; set; }
       List<string> SearchResults { get; set; }

       [Watch] bool ShowMeCheckBox { get; set; }
       [Watch] bool EnableMeCheckBox { get; set; }
       string CheckBoxResult { get; set; }

       [Watch] string SimpleDropDownValue { get; set; }
       List<string> SimpleDropDownOptions { get; set; }
       string SimpleDropDownResult { get; set; }

       [Watch] int? DropDownValue { get; set; }
       string DropDownCaption { get; set; }
       List<DropDownItem> DropDownOptions { get; set; }
       string DropDownResult { get; set; }

       [Watch] string RadioButtonValue { get; set; }
       string RadioButtonStyle { get; set; }

       int ClickCount { get; set; }
       void ButtonClicked(bool dummy);
   }

   public class DropDownItem
   {
       public int Id { get; set; }
       public string Text { get; set; }
   }

   private void UpdateState(IControlTypesState state)
   {
       this.state = state;
       StateHasChanged();
   }
}
```

</if>
<if viewmodel>
##### ControlTypes.cs

```csharp
public class ControlTypesVM : BaseVM
{
   // Text Box

   public string TextBoxValue
   {
      get => Get<string>() ?? "";
      set
      {
         Set(value);
         Changed(() => TextBoxResult);
      }
   }

   public string TextBoxPlaceHolder => "Type something here";
   public string TextBoxResult => !string.IsNullOrEmpty(TextBoxValue) ? $"You typed: {TextBoxValue}" : null;

   // Search Box

   private List<string> Planets = new List<string> { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Neptune", "Uranus" };

   public string SearchBox
   {
      get => Get<string>() ?? "";
      set
      {
         Set(value);
         Changed(() => SearchResults);
      }
   }

   public string SearchBoxPlaceHolder => "Type a planet";

   public IEnumerable<string> SearchResults => Planets.Where(i => !string.IsNullOrEmpty(SearchBox)
     && i.ToLower().StartsWith(SearchBox.ToLower())
     && i.ToLower() != SearchBox.ToLower());

   // Check Box

   public bool ShowMeCheckBox
   {
      get => Get<bool?>() ?? true;
      set
      {
         Set(value);
         Changed(() => CheckBoxResult);
      }
   }

   public bool EnableMeCheckBox
   {
      get => Get<bool?>() ?? true;
      set
      {
         Set(value);
         Changed(() => CheckBoxResult);
      }
   }

   public string CheckBoxResult => EnableMeCheckBox ? "Enabled" : "Disabled";

   // Simple Drop-down

   public List<string> SimpleDropDownOptions => new List<string> { "One", "Two", "Three", "Four" };

   public string SimpleDropDownValue
   {
      get => Get<string>() ?? "";
      set
      {
         Set(value);
         Changed(() => SimpleDropDownResult);
      }
   }

   public string SimpleDropDownResult => !string.IsNullOrEmpty(SimpleDropDownValue) ? $"You selected: {SimpleDropDownValue}" : null;

   // Drop Down Objects

   public class DropDownItem
   {
      public int Id { get; set; }
      public string Text { get; set; }
   }

   public string DropDownCaption => "Select an item ...";

   public List<DropDownItem> DropDownOptions
   {
      get => new List<DropDownItem>
      {
         new DropDownItem { Id = 1, Text = "Object One" },
         new DropDownItem { Id = 2, Text = "Object Two" },
         new DropDownItem { Id = 3, Text = "Object Three" },
         new DropDownItem { Id = 4, Text = "Object Four" }
      };
   }

   public int DropDownValue
   {
      get => Get<int>();
      set
      {
         Set(value);
         Changed(() => DropDownResult);
      }
   }

   public string DropDownResult => DropDownValue > 0 ? $"You selected: {DropDownOptions.First(i => i.Id == DropDownValue).Text}" : null;

   // Radio Buttons

   public string RadioButtonValue
   {
      get => Get<string>() ?? "green";
      set
      {
         Set(value);
         Changed(() => RadioButtonStyle);
      }
   }

   public string RadioButtonStyle => RadioButtonValue == "green" ? "label-success" : "label-warning";

   // Button

   public Action<bool> ButtonClicked => _ => ClickCount++;

   public int ClickCount
   {
      get => Get<int>();
      set => Set(value);
   }
}
```

</if>
