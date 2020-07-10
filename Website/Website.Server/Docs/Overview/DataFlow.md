## Data Flow Pattern

#### Single View Model

This is the simplest pattern that we've used so far, in which the root component is connected to a single back-end view model, serving the initial state and occasionally receiving updates to be persisted to the database. This works great for app with shallow component tree, or if integrating with client-side stores.

<d-image src="/assets/SingleVMPattern.svg" css="width:350px;display:flex;margin:2rem auto;align-items:center;justify-content:center"></d-image>

But you can make the view model do more. It can host any data-driven logic, such as selecting an item in the master list triggers new data being loaded into the details view. As demonstrated in the [Composite View Example](compositeview), with a single action-reaction cycle, the view model can process the selection changed event, load the data, and push the new state to your App view.

#### Siloed View Model

When the component tree is deeper, you may choose to connect some components to their own back-end view model. These view models are isolated from each other, and taken individually, they behave the same way as the above pattern.

<d-image src="/assets/SiloedVMPattern.svg" css="width:350px;display:flex;margin:2rem auto;align-items:center;justify-content:center"></d-image>

This pattern is well-suited for building an SPA, where each page is a stand-alone component. And it also works well with container components.

#### Scoped View Model

For a more complex app with many deeply-layered components that are emitting various events that trigger data updates on various other components, it's easy for your UI components to get bogged down with handling all the business logic. This pattern helps you to pull those logic out of your views and put them in the view models.

In this pattern, the view models are no longer isolated, but can be arranged hierarchically, in which a parent has access to its children and sets the context for them. Given this way, we can imagine the app as being composed of independent modules that are capable of interacting with one another through orchestration by a higher-order module, where each module is composed of front-end components and back-end view models that are working as a unit.

<d-image src="/assets/ScopedVMPattern.svg" css="width:400px;display:flex;margin:2rem auto;align-items:center;justify-content:center"></d-image>

#### Master-Details

To illustrate the scoped view model pattern, consider a master-detail view consisting of _Master_ and _Details_ components. When the selection changes in the _Master_, the _Details_ updates its data. There are many ways to implement this, such as having a container component that listens to the selection changed event and trigger updates on _Details_, or the components themselves emit and subscribe to events.

When the interaction grows more complex and it's getting harder to keep your components from getting bloated, adhere to single responsibility and maintain good decoupling, consider using this pattern and offload the logic into the view models. Here's a simple example on how it can be done:

<d-tab>
<d-tab-item itemkey="view" label="View">

<h5>MasterDetails.razor</h4>

```jsx
<VMContext VM='MasterDetails' TState='object'>
  <div style='display:flex'>
    <Master />
    <Details />
  </div>
</VMContext>
```

<h5>Master.razor</h4>

```jsx
<VMContext VM="Master" OnStateChange="(IMasterState state) => UpdateState(state)">
@if(state != null)
{
    foreach(var item in state?.ListItems)
    {
        <li key={item.Id} style="@GetItemStyle(item.Id)"
            @onclick="() => HandleSelect(item.Id)">@item.Title</li>
    }
}
</VMContext>

@code {
   private IMasterState state;
   private string selectedId;

   public interface IMasterState
   {
       ListItem[] ListItems { get; set; }
       void Select(string id);
   }

   public class ListItem
   {
       public string Id { get; set; }
       public string Title { get; set; }
   }

   private string GetItemStyle(string id)
   {
       var color = id == this.selectedId ? "#eee" : "none";
       return $"cursor:pointer; background:{color}";
   }

   private void HandleSelect(string id)
   {
       this.selectedId = id;
       this.state.Select(id);
   }

   private void UpdateState(IMasterState state)
   {
       this.state = state;
       this.selectedId = state.ListItems[0].Id;
       StateHasChanged();
   }
}
```

<h5>Details.razor</h4>

```jsx
<VMContext VM="Details" OnStateChange="(IDetailsState state) => UpdateState(state)">
@if (state != null)
{
    <img src={state.ItemImageUrl} style="margin:0 1rem" />;
}
</VMContext>

@code {
   private IDetailsState state;

   public interface IDetailsState
   {
       string ItemImageUrl { get; set; }
   }

   private void UpdateState(IDetailsState state)
   {
       this.state = state;
       StateHasChanged();
   }
}
```

</d-tab-item>
<d-tab-item itemkey="viewmodel" label="View Model">

```csharp
public class MasterDetails : BaseVM
{
   private readonly IWebStoreService _webStoreService;

   private event EventHandler<int> SelectedItem;

   public MasterDetail(IWebStoreService webStoreService)
   {
      _webStoreService = webStoreService;
   }

   public override void OnSubVMCreated(BaseVM vm)
   {
      if (vm is Master)
      {
         var masterList = vm as Master;
         masterList.Selected += (sender, id) => SelectedItem?.Invoke(this, id);
      }
      else if (vm is Details)
      {
         var details = vm as Details;
         SelectedItem += async (sender, id) => details.SetData(await _webStoreService.GetBookByIdAsync(id));
      }
   }
}
```

```csharp
public class Master : BaseVM
{
   private readonly IWebStoreService _webStoreService;

   public IEnumerable<WebStoreRecord> ListItems
   {
      get => Get<IEnumerable<WebStoreRecord>>();
      set => Set(value);
   }

   public event EventHandler<int> Selected;

   public Master(IWebStoreService webStoreService)
   {
      _webStoreService = webStoreService;
   }

   public override async Task OnCreatedAsync()
   {
      ListItems = await _webStoreService.GetAllBooksAsync();
   }

   public void Select(int id) => Selected?.Invoke(this, id);
}
```

```csharp
public class Details : BaseVM
{
   public string ItemImageUrl
   {
      get => Get<string>();
      set => Set(value);
   }

   public void SetData(WebStoreRecord data) => ItemImageUrl = data.ImageUrl;
}
```

<br/>

Notice that in the UI view, the components are connecting to their own view models to get their state, and nothing is passed between components. It's their view models that are doing that on the back-end, and pushing the state change to the front-end.

By nesting a component with a _VMContext_ under a parent _VMContext_, the connect name of that component will be qualified with the parent view model name. This allows the parent view model to intercept the lifecycle of its children. In the case above, the BookStore parent receives a call right after a child view model instance is created, which allows it to subscribe to the _Selected_ event of the _Master_, so that it can in turn update the Details.

In general, the parent view model serves as a light orchestrator of the view models under its scope. This is the same uni-directional data flow as in the front-end, and it keeps the child view models stay decoupled from one another. The parent too can be decoupled from the children, by using interfaces instead of concrete types.

The APIs that provide the lifecycle hooks for the parent view model are:

- **GetSubVM**(vmType): give the parent the opportunity to create its own instance of a child view model.
- **OnSubVMCreated**(vmObject): pass a newly created child view model instance to the parent.
- **OnSubVMDestroyed**(vmObject): pass the child view model instance that will soon be disposed to the parent.

This pattern helps make both the views and their view models highly reusable. It gives a potential to package the components and their view models in modules and in different assemblies, which will make it easier for multiple teams towards working autonomously. And to take it even further, given the right orchestration middleware it's possible to go full-stack microservice on very complex apps, where the app is composed of autonomous modules on their own databases and server processes. For a more fleshed-out example, see the [Composite View Example](/core/examples/compositeview).
