<if view>
##### Form.razor

```jsx
 <VMContext VM="CustomerForm" TState="object" OnElementEvent="HandleElementEvent">
     <d-panel>
         <d-data-grid id="Contacts"></d-data-grid>
         <d-form id="_Form" @key="IsViewing" plaintext="@IsViewing">
             <d-panel>
                 <d-panel horizontal="true">
                     <d-panel horizontal="true">
                         <d-button id="_Edit" label="Edit" enable="@IsViewing"></d-button>
                         <d-button id="Submit" label="Update" submit="true" show="@IsEditing"></d-button>
                         <d-button id="_Cancel" label="Cancel" cancel="true" secondary="true" show="@IsEditing"></d-button>
                     </d-panel>
                     <d-panel right="true">
                         <d-button id="_New" label="New Customer" enable="@IsViewing"></d-button>
                     </d-panel>
                 </d-panel>
                 <FormTabs />
             </d-panel>
         </d-form>
         @if (showDialog)
         {
             <NewFormDialog OnClosed="@ToggleDialog"></NewFormDialog>
         }
     </d-panel>
 </VMContext>

@code {
   private bool editMode;
   private bool showDialog;

   private string IsEditing => editMode.ToString().ToLower();
   private string IsViewing => (!editMode).ToString().ToLower();

   private void HandleElementEvent(ElementEvent elementEvent)
   {
       if (elementEvent.TargetId == "_Edit" || elementEvent.TargetId == "_Cancel" || elementEvent.TargetId == "Submit")
           editMode = !editMode;
       else if (elementEvent.TargetId == "_New")
           ToggleDialog();

       StateHasChanged();
   }

   private void ToggleDialog()
   {
       showDialog = !showDialog;
       StateHasChanged();
   }
}
```

##### FormTabs.razor

```jsx
<d-tab margin="1.5rem 0">
  <d-tab-item itemkey="basic" label="Basic Info">
    <d-panel horizontal="true" nogap="true">
      <d-panel horizontal="true">
        <d-cell header="Person" flex="true">
          <PersonForm />
        </d-cell>
        <d-cell header="Phone" flex="1" borders="top, right, bottom">
          <PhoneForm />
        </d-cell>
      </d-panel>
    </d-panel>
  </d-tab-item>
  <d-tab-item itemkey="address" label="Address">
    <d-panel>
      <d-cell header="Primary Address">
        <AddressForm />
      </d-cell>
    </d-panel>
  </d-tab-item>
</d-tab>
```

##### PersonForm.razor

```jsx
<VMContext VM="PersonForm" TState="object">
  <d-form id="Person">
    <d-panel>
      <d-dropdown-list id="Prefix" horizontal="true"></d-dropdown-list>
      <d-text-field id="FirstName" horizontal="true"></d-text-field>
      <d-text-field id="MiddleName" horizontal="true"></d-text-field>
      <d-text-field id="LastName" horizontal="true"></d-text-field>
      <d-dropdown-list id="Suffix" horizontal="true"></d-dropdown-list>
    </d-panel>
  </d-form>
</VMContext>
```

##### PhoneForm.razor

```jsx
<VMContext VM="PhoneForm" TState="object">
  <d-form id="Phone">
    <d-panel>
      <d-text-field id="Work" horizontal="true"></d-text-field>
      <d-text-field id="Home" horizontal="true"></d-text-field>
      <d-text-field id="Mobile" horizontal="true"></d-text-field>
      <drop-down-list id="Primary" horizontal="true"></drop-down-list>
    </d-panel>
  </d-form>
</VMContext>
```

##### AddressForm.razor

```jsx
<VMContext VM="AddressForm" TState="object">
  <d-form id="Address">
    <d-panel>
      <d-text-field id="Address1" horizontal="true"></d-text-field>
      <d-text-field id="Address2" horizontal="true"></d-text-field>
      <d-text-field id="City" horizontal="true"></d-text-field>
      <d-dropdown-list id="State" horizontal="true"></d-dropdown-list>
      <d-number-field id="ZipCode" horizontal="true"></d-number-field>
    </d-panel>
  </d-form>
</VMContext>
```

##### NewFormDialog.razor

```jsx
<VMContext VM="NewCustomerForm" TState="object" OnElementEvent="HandleElementEvent">
    <d-modal id="_Dialog" header="New Customer" open="@isOpen" large="true" style="position: fixed">
        <d-panel css="min-height: 22rem">
            <d-tab>
                <d-tab-item itemkey="Person" label="Person">
                    <PersonForm />
                </d-tab-item>
                <d-tab-item itemkey="Phone" label="Phone">
                    <PhoneForm />
                </d-tab-item>
                <d-tab-item itemkey="Address" label="Address">
                    <AddressForm />
                </d-tab-item>
            </d-tab>
        </d-panel>
        <footer>
            <d-panel horizontal="true" right="true">
                <d-button id="_Cancel" label="Cancel" cancel="true" secondary="true"></d-button>
                <d-button id="Submit" label="Submit" submit="true"></d-button>
            </d-panel>
        </footer>
    </d-modal>
</VMContext>

@code {
   [Parameter] public EventCallback OnClosed { get; set; }

   private string isOpen = "true";

   private void HandleElementEvent(ElementEvent e)
   {
       if (e.TargetId == "_Cancel" || e.TargetId == "Submit")
       {
           isOpen = "false";
           StateHasChanged();
           _ = Task.Delay(250 /* wait for closing animation*/).ContinueWith(_ => OnClosed.InvokeAsync(EventCallback.Empty));
       }
   }
}
```

</if>
<if viewmodel>
##### CustomerForm.cs

```csharp
public partial class CustomerForm : BaseVM
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ReactiveProperty<int> _selectedContact;

    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }

    public CustomerForm(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;

        _selectedContact = AddProperty<int>("SelectedContact", 1);

        AddProperty("Contacts", customerRepository.GetAll().Select(customer => ToContact(customer)))
            .WithItemKey(nameof(Contact.Id))
            .WithAttribute(new DataGridAttribute
            {
                RowKey = nameof(Contact.Id),
                Columns = new DataGridColumn[] {
                    new DataGridColumn(nameof(Contact.Name), "Name") { Sortable = true },
                    new DataGridColumn(nameof(Contact.Phone), "Phone") { Sortable = true },
                    new DataGridColumn(nameof(Contact.Address), "Address") { Sortable = true },
                    new DataGridColumn(nameof(Contact.City), "City") { Sortable = true },
                    new DataGridColumn(nameof(Contact.ZipCode), "ZipCode") { Sortable = true }
                },
                Rows = 5
            }.CanSelect(DataGridAttribute.Selection.Single, _selectedContact));

        AddInternalProperty<CustomerFormData>("Submit")
            .SubscribedBy(AddProperty<bool>("SubmitSuccess"), formData => Save(formData));
    }

    public override void OnSubVMCreated(BaseVM subVM)
    {
        // Have sub-forms with 'Customer' property subscribe to the customer data grid's selection changed event.
        var customerPropInfo = subVM.GetType().GetProperty(nameof(Customer));
        if (typeof(ReactiveProperty<Customer>).IsAssignableFrom(customerPropInfo?.PropertyType))
            _selectedContact.SubscribedBy(
                customerPropInfo.GetValue(subVM) as ReactiveProperty<Customer>,
                id => _customerRepository.Get(id)
            );

        if (subVM is NewCustomerForm)
            (subVM as NewCustomerForm).NewCustomer.Subscribe(customer => UpdateContact(customer));
    }

    private bool Save(CustomerFormData formData)
    {
        var id = (int)_selectedContact.Value;
        var customer = _customerRepository.Update(id, formData);

        this.UpdateList("Contacts", ToContact(customer));
        _selectedContact.Value = id;
        return true;
    }

    private Contact ToContact(Customer customer) => new Contact
    {
        Id = customer.Id,
        Name = customer.Name.FullName,
        Address = customer.Address.StreetAddress,
        City = customer.Address.City,
        ZipCode = customer.Address.ZipCode,
        Phone = customer.Phone.PrimaryNumber
    };

    private void UpdateContact(Customer newCustomer)
    {
        this.AddList("Contacts", ToContact(newCustomer));
        _selectedContact.OnNext(newCustomer.Id);
    }
}
```

##### PersonForm.cs

```csharp
public class PersonForm : BaseVM
{
    public ReactiveProperty<Customer> Customer { get; } = new ReactiveProperty<Customer>();

    public PersonForm()
    {
        AddProperty<string>(nameof(NameInfo.FullName))
            .WithAttribute(new TextFieldAttribute { Label = "Name:" })
            .SubscribeTo(Customer.Select(x => x.Name.FullName));

        AddProperty<NamePrefix>(nameof(NameInfo.Prefix))
            .WithAttribute(new DropdownListAttribute { Label = "Prefix:", Options = typeof(NamePrefix).ToDescriptions() })
            .SubscribeTo(Customer.Select(x => x.Name.Prefix));

        AddProperty<string>(nameof(NameInfo.FirstName))
            .WithAttribute(new TextFieldAttribute { Label = "First Name:", MaxLength = 35 })
            .WithRequiredValidation()
            .SubscribeTo(Customer.Select(x => x.Name.FirstName));

        AddProperty<string>(nameof(NameInfo.MiddleName))
            .WithAttribute(new TextFieldAttribute { Label = "Middle Name:", MaxLength = 35 })
            .SubscribeTo(Customer.Select(x => x.Name.MiddleName));

        AddProperty<string>(nameof(NameInfo.LastName))
            .WithAttribute(new TextFieldAttribute { Label = "Last Name:", MaxLength = 35 })
            .WithRequiredValidation()
            .SubscribeTo(Customer.Select(x => x.Name.LastName));

        AddProperty<NameSuffix>(nameof(NameInfo.Suffix))
            .WithAttribute(new DropdownListAttribute { Label = "Suffix:", Options = typeof(NameSuffix).ToDescriptions() })
            .SubscribeTo(Customer.Select(x => x.Name.Suffix));
    }
}
```

##### PhoneForm.cs

```csharp
public class PhoneForm : BaseVM
{
    public ReactiveProperty<Customer> Customer { get; } = new ReactiveProperty<Customer>();

    public PhoneForm()
    {
        AddProperty<string>(nameof(PhoneInfo.Work))
            .WithAttribute(new TextFieldAttribute { Label = "Work:", Mask = "(999) 999-9999" })
            .WithPatternValidation(Pattern.USPhoneNumber)
            .SubscribeTo(Customer.Select(x => x.Phone.Work));

        AddProperty<string>(nameof(PhoneInfo.Home))
            .WithAttribute(new TextFieldAttribute { Label = "Home:", Mask = "(999) 999-9999" })
            .WithPatternValidation(Pattern.USPhoneNumber)
            .SubscribeTo(Customer.Select(x => x.Phone.Home));

        AddProperty<string>(nameof(PhoneInfo.Mobile))
            .WithAttribute(new TextFieldAttribute { Label = "Mobile:", Mask = "(999) 999-9999" })
            .WithPatternValidation(Pattern.USPhoneNumber)
            .SubscribeTo(Customer.Select(x => x.Phone.Mobile));

        AddProperty<PrimaryPhone>(nameof(PhoneInfo.Primary))
            .WithAttribute(new DropdownListAttribute { Label = "Primary Phone:", Options = typeof(PrimaryPhone).ToDescriptions() })
            .SubscribeTo(Customer.Select(x => x.Phone.Primary));
    }
}
```

##### AddressForm.cs

```csharp
public class AddressForm : BaseVM
{
    public ReactiveProperty<Customer> Customer { get; } = new ReactiveProperty<Customer>();

    public AddressForm()
    {
        AddProperty<string>(nameof(AddressInfo.Address1))
            .WithAttribute(new TextFieldAttribute { Label = "Address 1:" })
            .SubscribeTo(Customer.Select(x => x.Address.Address1));

        AddProperty<string>(nameof(AddressInfo.Address2))
            .WithAttribute(new TextFieldAttribute { Label = "Address 2:" })
            .SubscribeTo(Customer.Select(x => x.Address.Address2));

        AddProperty<string>(nameof(AddressInfo.City))
            .WithAttribute(new TextFieldAttribute { Label = "City:" })
            .SubscribeTo(Customer.Select(x => x.Address.City));

        AddProperty<State>(nameof(AddressInfo.State))
            .WithAttribute(new DropdownListAttribute
            {
                Label = "State:",
                Options = typeof(State).ToDescriptions()
            })
            .SubscribeTo(Customer.Select(x => x.Address.State));

        AddProperty<string>(nameof(AddressInfo.ZipCode))
            .WithAttribute(new TextFieldAttribute { Label = "Zip Code:" })
            .SubscribeTo(Customer.Select(x => x.Address.ZipCode));
    }
}
```

##### NewCustomerForm.cs

```csharp
public class NewCustomerForm : BaseVM
{
    private readonly ICustomerRepository _customerRepository;

    public ReactiveProperty<Customer> NewCustomer { get; } = new ReactiveProperty<Customer>();

    public NewCustomerForm(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;

        AddInternalProperty<CustomerFormData>("Submit")
            .SubscribedBy(NewCustomer, formData => Save(formData));
    }

    public Customer Save(CustomerFormData formData)
    {
        return _customerRepository.Add(formData);
    }
}
```

##### CustomerFormData.cs

```csharp
using StringDictionary = Dictionary<string, string>;

public class CustomerFormData
{
    public StringDictionary Person { get; set; }
    public StringDictionary Phone { get; set; }
    public StringDictionary Address { get; set; }
}
```

</if>
