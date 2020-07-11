<if view>
##### Dashboard.razor

```jsx
<VMContext VM='Dashboard' TState='IDashboardState' OnStateChange='UpdateState'>
  @if (state == null)
  {<div class='spinner lds-hourglass' />}
  else
  {
    <d-panel>
      <d-panel horizontal='true' wrap='true'>
        <d-panel>
          <InfoCard Id='Download' Color='#1c8adb'></InfoCard>
        </d-panel>
        <d-panel>
          <InfoCard id='Upload' Color='#5cb85c'></InfoCard>
        </d-panel>
        <d-panel>
          <InfoCard id='Latency' Color='#f0ad4e'></InfoCard>
        </d-panel>
        <d-panel>
          <InfoCard id='Users' Color='#d9534f'></InfoCard>
        </d-panel>
      </d-panel>
      <d-panel>
        <d-card>
          <d-panel horizontal='true'>
            <d-panel flex='70%'>
              <h4>Network Traffic</h4>
              <d-line-chart id='Traffic' height='75px' streaming='true' />
            </d-panel>
            <d-panel flex='30%' css='min-width:256px'>
              <h4>Utilization</h4>
              <d-pie-chart id='Utilization' />
            </d-panel>
          </d-panel>
        </d-card>
        <d-panel horizontal='true'>
          <d-panel flex='40%' css='min-width:300px'>
            <ActivitiesCard Activities='@state?.RecentActivities' />
          </d-panel>
          <d-panel flex='60%'>
            <d-card>
              <h4>Server Usage</h4>
              <d-bar-chart id='ServerUsage' height='70px'></d-bar-chart>
            </d-card>
          </d-panel>
        </d-panel>
      </d-panel>
    </d-panel>
  }
</VMContext>
```

##### InfoCard.razor

```jsx
@inject IStylesheet Stylesheet

<d-element id="@Id" css="@getCss()">
    <d-card horizontal="true">
        <d-card-image>
            <i class="material-icons info-icon" slot="icon"></i>
        </d-card-image>
        <label slot="label"></label>
        <h3 slot="value"></h3>
    </d-card>
</d-element>

@code {
    [Parameter] public string Id { get; set; }
    [Parameter] public string Color { get; set; }

    public string getCss() => Stylesheet["InfoCard"].Replace("$icon-bg-color", Color);
}
```

##### ActivitiesCard.razor

```jsx
<d-card css="height: 100%">
    <d-panel>
        <h4>Activities</h4>
        @foreach (var activity in Activities)
        {
            <d-panel horizontal="true" css=@getCss(activity)>
                <div>
                    <span class="user-icon">@getInitial(activity.PersonName)</span>
                    @activity.PersonName
                </div>
                <d-panel right="true" middle="true">
                    <div>
                        @activity.Status
                        <span class="status-icon" />
                    </div>
                </d-panel>
            </d-panel>
        }
    </d-panel>
</d-card>

@code {
   [Parameter] public Dashboard.Activity[] Activities { get; set; }

   private static readonly string[] statusColors = new string[] { "", "silver", "limegreen", "red", "gray", "orange" };
   private static readonly string[] userIconColors = new string[] { "#00ce6f", "#a95df0", "#2ea7eb" };

   private char getInitial(string name) => name.ToUpper()[0];

   private string getCss(Dashboard.Activity activity)
   {
       string iconColor = userIconColors[getInitial(activity.PersonName) % 3];
       string statusColor = statusColors[activity.StatusId];

       return Stylesheet["ActivitiesCard"]
           .Replace("$icon-color", iconColor)
           .Replace("$status-color", statusColor);
   }
}
```

</if>
<if viewmodel>
##### Dashboard.cs

```csharp
public class Dashboard : BaseVM, IRoutable
{
   private IDisposable _subscription;

   public RoutingState RoutingState { get; set; } = new RoutingState();

   public Dashboard(ILiveDataService liveDataService)
   {
      AddProperty<string>("Download")
         .WithAttribute(new { Label = "Download", Icon = "cloud_download" })
         .SubscribeTo(liveDataService.Download);

      AddProperty<string>("Upload")
         .WithAttribute(new { Label = "Upload", Icon = "cloud_upload" })
         .SubscribeTo(liveDataService.Upload);

      AddProperty<string>("Latency")
         .WithAttribute(new { Label = "Latency", Icon = "network_check" })
         .SubscribeTo(liveDataService.Latency);

      AddProperty<int>("Users")
         .WithAttribute(new { Label = "Users", Icon = "face" })
         .SubscribeTo(liveDataService.Users);

      AddProperty<int[]>("Traffic").SubscribeTo(liveDataService.Traffic);

      AddProperty<int[]>("Utilization")
         .WithAttribute(new ChartAttribute { Labels = new string[] { "Memory", "Disk", "Network" } })
         .SubscribeTo(liveDataService.Utilization);

      AddProperty<int[]>("ServerUsage").SubscribeTo(liveDataService.ServerUsage)
         .WithAttribute(new ChartAttribute { Labels = new string[] { "dns", "sql", "nethst", "w2k", "ubnt", "uat", "ftp", "smtp", "exch", "demo" } });

      AddProperty<Activity[]>("RecentActivities")
         .SubscribeTo(liveDataService.RecentActivity.Select(value =>
         {
            var activities = new Queue<Activity>(Get<Activity[]>("RecentActivities")?.Reverse() ?? new Activity[] { });
            activities.Enqueue(value);
            if (activities.Count > 4)
               activities.Dequeue();

            return activities.Reverse().ToArray();
         }));

      // Regulate data update interval to no less than every 200 msecs.
      _subscription = Observable
         .Interval(TimeSpan.FromMilliseconds(200))
         .StartWith(0)
         .Subscribe(_ => PushUpdates());
   }

   public override void Dispose()
   {
      _subscription?.Dispose();
      base.Dispose();
   }
}
```

</if>
