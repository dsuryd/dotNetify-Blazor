<if view>
##### SimpleList.razor

```jsx
@inject IStylesheet Stylesheet
@inject IJSRuntime JSRuntime

<VMContext VM="SimpleListVM" TState="ISimpleListState" OnStateChange="UpdateState">
@if (state != null)
{
    <Stylesheet Context="this">
        <d-alert info="true">
            <i class="material-icons">info_outlined</i>
            This is a multicast list. Your edits will appear on all other browser views in real-time.
        </d-alert>
        <header>
            <input type="text" class="form-control" placeholder="First name" @bind="firstName" />
            <input type="text" class="form-control" placeholder="Last name" @bind="lastName" />
            <button type="button" class="btn btn-primary" @onclick="OnAdd">Add</button>
        </header>
        <table>
            <thead>
                <tr>
                    <th>First Name</th>
                    <th>Last Name</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                @foreach (var employee in state.Employees)
                {
                    <tr>
                        <td>
                            <div>
                                @if (editFirstNameId != employee.Id)
                                {
                                    <span class="editable"
                                          @onclick="_ => OnEditFirstName(employee.Id)">
                                        @employee.FirstName
                                    </span>
                                }
                                else
                                {
                                    <input id="input-first-name"
                                           type="text"
                                           class="form-control"
                                           value="@employee.FirstName"
                                           @onblur="_ => { editFirstNameId = null; StateHasChanged(); }"
                                           @onchange="e => OnChangeFirstName(e, employee)" />
                                }
                            </div>
                        </td>
                        <td>
                            <div>
                                @if (editLastNameId != employee.Id)
                                {
                                    <span class="editable"
                                          @onclick="_ => OnEditLastName(employee.Id)">
                                        @employee.LastName
                                    </span>
                                }
                                else
                                {
                                    <input id="input-last-name"
                                           type="text"
                                           class="form-control"
                                           value="@employee.LastName"
                                           @onblur="_ => { editLastNameId = null; StateHasChanged(); }"
                                           @onchange="e => OnChangeLastName(e, employee)"  />
                                }
                            </div>
                        </td>
                        <td>
                            <div @onclick="_ => state.Remove(employee.Id)">
                                <i class="material-icons">clear</i>
                            </div>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </Stylesheet>
}
</VMContext>

@code {
   private ISimpleListState state;
   private string firstName;
   private string lastName;
   private int? editFirstNameId;
   private int? editLastNameId;

   public interface ISimpleListState
   {
       IEnumerable<Employee> Employees { get; set; }
       void Add(string fullName);
       void Update(dynamic employee);
       void Remove(int Id);
   }

   public class Employee
   {
       public int Id { get; set; }
       public string FirstName { get; set; }
       public string LastName { get; set; }
   }

   protected override void OnAfterRender(bool firstRender)
   {
       if (editFirstNameId != null)
           JSRuntime.InvokeVoidAsync("app.setFocus", "input-first-name");
       else if (editLastNameId != null)
           JSRuntime.InvokeVoidAsync("app.setFocus", "input-last-name");
   }

   private void UpdateState(ISimpleListState state)
   {
       this.state = state;
       StateHasChanged();
   }

   private void OnAdd()
   {
       string fullName = $"{this.firstName} {this.lastName}";
       if (!string.IsNullOrWhiteSpace(fullName))
           state.Add(fullName);

       firstName = lastName = string.Empty;
       StateHasChanged();
   }

   private void OnEditFirstName(int employeeId)
   {
       editFirstNameId = employeeId;
       editLastNameId = null;
       StateHasChanged();
   }

   private void OnEditLastName(int employeeId)
   {
       editLastNameId = employeeId;
       editFirstNameId = null;
       StateHasChanged();
   }

   private void OnChangeFirstName(ChangeEventArgs e, Employee employee)
   {
       employee.FirstName = e.Value.ToString();
       state.Update(new { Id = employee.Id, FirstName = employee.FirstName });
       editFirstNameId = null;
       StateHasChanged();
   }

   private void OnChangeLastName(ChangeEventArgs e, Employee employee)
   {
       employee.LastName = e.Value.ToString();
       state.Update(new { Id = employee.Id, LastName = employee.LastName });
       editLastNameId = null;
       StateHasChanged();
   }
}
```

</if>
<if viewmodel>
##### SimpleList.cs

```csharp
public class SimpleListVM : MulticastVM
{
   private readonly IEmployeeRepository _repository;
   private readonly IConnectionContext _connectionContext;

   public class EmployeeInfo
   {
      public int Id { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
   }

   // If you use CRUD methods on a list, you must set the item key prop name of that list with ItemKey attribute.
   [ItemKey(nameof(Employee.Id))]
   public IEnumerable<EmployeeInfo> Employees { get; private set; }

   // Clients from the same IP address will share the same VM instance.
   public override string GroupName => _connectionContext.HttpConnection.RemoteIpAddress.ToString();

   public SimpleListVM(IEmployeeRepository repository, IConnectionContext connectionContext)
   {
      _repository = repository;
      _connectionContext = connectionContext;
   }

   public override async Task OnCreatedAsync()
   {
      Employees = (await _repository.GetAllAsync(7))
         .Select(i => new EmployeeInfo { Id = i.Id, FirstName = i.FirstName, LastName = i.LastName });
   }

   public async Task Add(string fullName)
   {
      var names = fullName.Split(new char[] { ' ' }, 2);
      var employee = new Employee
      {
         FirstName = names.First(),
         LastName = names.Length > 1 ? names.Last() : ""
      };

      // Use CRUD base method to add the list item on the client.
      this.AddList("Employees", new EmployeeInfo
      {
         Id = await _repository.AddAsync(employee),
         FirstName = employee.FirstName,
         LastName = employee.LastName
      });
   }

   public async Task Update(EmployeeInfo employeeInfo)
   {
      var employee = await _repository.GetAsync(employeeInfo.Id);
      if (employee != null)
      {
         employee.FirstName = employeeInfo.FirstName ?? employee.FirstName;
         employee.LastName = employeeInfo.LastName ?? employee.LastName;
         await _repository.UpdateAsync(employee);

         this.UpdateList(nameof(Employees), new EmployeeInfo
         {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName
         });
      }
   }

   public async Task Remove(int id)
   {
      await _repository.RemoveAsync(id);

      // Use CRUD base method to remove the list item on the client.
      this.RemoveList(nameof(Employees), id);
   }
}
```

</if>
