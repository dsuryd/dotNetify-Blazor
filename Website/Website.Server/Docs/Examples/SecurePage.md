<if view>
##### Login.razor

```jsx
@inject HttpClient Http
@inject IStylesheet Stylesheet

<d-panel css="@Stylesheet["Login"]">
    <div>
        @if (accessToken != null)
        {
            <div class="card logout">
                <button class="btn btn-primary" @onclick="SignOut">Sign Out</button>
            </div>
            <SecurePage AccessToken="@accessToken" OnExpiredAccess="SignOut"></SecurePage>
        }
        else
        {
            <div class="card">
                <div class="card-header">
                    <h4>Sign in</h4>
                </div>
                <div class="card-body">
                    <div>
                        <label>User name:</label>
                        <input type="text"
                               class="form-control"
                               placeholder="Type guest or admin"
                               @bind="userName" />
                        <b>@loginError</b>
                    </div>
                    <div>
                        <label>Password:</label>
                        <input type="password" class="form-control" @bind="password" />
                        <b>@loginError</b>
                    </div>
                </div>
                <div class="card-footer">
                    <button class="btn btn-primary" @onclick="Submit">Submit</button>
                </div>
            </div>
            <div class="card">
                <div class="card-body">
                    <h4>Not authenticated</h4>
                </div>
            </div>
        }
    </div>
</d-panel>

@code {
   private string userName;
   private string password = "dotnetify";
   private string accessToken;
   private string loginError;

   internal class TokenResponse
   {
       public string access_token { get; set; }
   }

   private async Task Submit()
   {
       var response = await FetchTokenAsync(Http, this.userName, this.password);
       if (response.IsSuccessStatusCode)
       {
           var result = await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync());
           this.accessToken = result.access_token;
           this.loginError = string.Empty;
       }
       else
           this.loginError = "Invalid user name or password";
       StateHasChanged();
   }

   private void SignOut()
   {
       accessToken = null;
       StateHasChanged();
   }

   internal static Task<HttpResponseMessage> FetchTokenAsync(HttpClient http, string userName, string password)
   {
       var content = new Dictionary<string, string>();
       content.Add("username", userName);
       content.Add("password", password);
       content.Add("grant_type", "password");
       content.Add("client_id", "dotnetifydemo");

       return http.PostAsync("/token", new FormUrlEncodedContent(content));
   }
}
```

##### SecurePage.razor

```jsx
<VMContext VM="SecurePageVM" Options="@connectOptions" TState="ISecurePageState" OnStateChange="UpdateState">
@if (state != null)
{
    <div class="card">
        <div class="card-header">
            <h4>@state.SecureCaption</h4>
        </div>
        <div class="card-body">
            <p>
                <b>@state.SecureData</b>
            </p>
            <AdminSecurePage AccessToken="@AccessToken" OnExpiredAccess="HandleExpiredAccess" />
        </div>
    </div>
}
</VMContext>

@code {
   [Parameter] public string AccessToken { get; set; }
   [Parameter] public EventCallback OnExpiredAccess { get; set; }

   private ISecurePageState state;
   private VMConnectOptions connectOptions;

   public interface ISecurePageState
   {
       string SecureCaption { get; set; }
       string SecureData { get; set; }
   }

   protected override void OnInitialized()
   {
       this.connectOptions = new VMConnectOptions
       {
           Headers = new Dictionary<string, object>
           {
               { "Authorization", "Bearer " + AccessToken }
           }
       };
   }

   private async Task HandleExpiredAccess()
   {
       await OnExpiredAccess.InvokeAsync(null);
   }

   private void UpdateState(ISecurePageState state)
   {
       this.state = state;
       StateHasChanged();
   }
}
```

##### AdminSecurePage.razor

```jsx
@inject HttpClient Http

<VMContext VM="AdminSecurePageVM" Options="connectOptions" TState="IAdminSecurePageState" OnStateChange="UpdateState">
@if (state != null)
{
    <section>
        <h5>Admin-only view:</h5>
        <div>@state.TokenIssuer</div>
        <div>@state.TokenValidFrom</div>
        <div>@state.TokenValidTo</div>
        <br />
        <button class="btn btn-secondary" @onclick="HandleRefreshAsync">Refresh Token</button>
    </section>
}
</VMContext>

@code {
   [Parameter] public string AccessToken { get; set; }
   [Parameter] public EventCallback OnExpiredAccess { get; set; }

   private IAdminSecurePageState state;
   private VMConnectOptions connectOptions;

   public interface IAdminSecurePageState
   {
       string TokenIssuer { get; set; }
       string TokenValidFrom { get; set; }
       string TokenValidTo { get; set; }

       void Dispatch(Dictionary<string, object> properties);
   }

   protected override void OnInitialized()
   {
       this.connectOptions = new VMConnectOptions
       {
           Headers = new Dictionary<string, object>
           {
               { "Authorization", "Bearer " + AccessToken }
           }
       };
   }

   private async Task HandleRefreshAsync(MouseEventArgs e)
   {
       var response = await Login.FetchTokenAsync(Http, "admin", "dotnetify");
       if (response.IsSuccessStatusCode)
       {
           var result = await JsonSerializer.DeserializeAsync<Login.TokenResponse>(await response.Content.ReadAsStreamAsync());
           this.state.Dispatch(new Dictionary<string, object>
           {
               { "$headers", new { Authorization = "Bearer " + result.access_token } },
               { "Refresh", true }
           });
       }
   }

   private void UpdateState(IAdminSecurePageState state)
   {
       this.state = state;
       StateHasChanged();
   }
}
```

</if>
<if viewmodel>
##### SecurePage.cs

```csharp
public class SetAccessTokenAttribute : Attribute { }

[Authorize]
[SetAccessToken]
public class SecurePageVM : BaseVM
{
   private Timer _timer;
   private SecurityToken _accessToken;
   private readonly IPrincipalAccessor _principalAccessor;

   private int AccessExpireTime => (int) (_accessToken.ValidTo - DateTime.UtcNow).TotalSeconds;

   public string SecureCaption { get; set; }
   public string SecureData { get; set; }

   public SecurePageVM(IPrincipalAccessor principalAccessor)
   {
      _principalAccessor = principalAccessor;
   }

   public override void Dispose() => _timer?.Dispose();

   public void SetAccessToken(SecurityToken accessToken)
   {
      if (_accessToken?.ValidTo != accessToken.ValidTo)
      {
         _accessToken = accessToken;
         SecureCaption = $"Authenticated user: \"{_principalAccessor.Principal?.Identity.Name}\"";
         Changed(nameof(SecureCaption));

         // IMPORTANT: Create new timer if access token changes to make sure the timer thread uses
         // the new hub caller context with the updated claims principal from the new token.
         _timer?.Dispose();
         _timer = new Timer(state =>
         {
            SecureData = _accessToken != null ? $"Access token will expire in {AccessExpireTime} seconds" : null;
            Changed(nameof(SecureData));
            PushUpdates();
         }, null, 0, 1000);
      }
   }
}
```

##### AdminSecurePage.cs

```csharp
[Authorize(Role = "admin")]
[SetAccessToken]
public class AdminSecurePageVM : BaseVM
{
   public string TokenIssuer { get; set; }
   public string TokenValidFrom { get; set; }
   public string TokenValidTo { get; set; }

   public void Refresh()
   {
      /* no op */
   }

   public void SetAccessToken(SecurityToken accessToken)
   {
      TokenIssuer = $"Token issuer: \"{accessToken.Issuer}\"";
      TokenValidFrom = $"Valid from: {accessToken.ValidFrom:R}";
      TokenValidTo = $"Valid to: {accessToken.ValidTo:R}";

      Changed(nameof(TokenIssuer));
      Changed(nameof(TokenValidFrom));
      Changed(nameof(TokenValidTo));
   }
}
```

##### ExtractAccessTokenMiddleware.cs

```csharp
// Middleware to extract access token from the authentication header and put it in the pipeline data.
public class ExtractAccessTokenMiddleware : JwtBearerAuthenticationMiddleware
{
   public ExtractAccessTokenMiddleware(TokenValidationParameters tokenValidationParameters) : base(tokenValidationParameters)
   {
   }

   public override Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
   {
      if (hubContext.Headers != null)
      {
         try
         {
            ValidateBearerToken(ParseHeaders<HeaderData>(hubContext.Headers), out SecurityToken validatedToken);
            if (validatedToken != null)
               hubContext.PipelineData.Add("AccessToken", validatedToken);
         }
         catch (Exception)
         {
         }
      }

      return next(hubContext);
   }
}
```

##### SetAccessTokenFilter.cs

```csharp
// Custom filter to set JWT access token to any view model that has AccessToken property.
public class SetAccessTokenFilter : IVMFilter<SetAccessTokenAttribute>
{
   public Task Invoke(SetAccessTokenAttribute attr, VMContext vmContext, NextFilterDelegate next)
   {
      var methodInfo = vmContext.Instance.GetType().GetTypeInfo().GetMethod("SetAccessToken");
      var accessToken = vmContext.HubContext.PipelineData.ContainsKey("AccessToken") ? vmContext.HubContext.PipelineData["AccessToken"] : null;

      if (methodInfo != null && accessToken != null)
         methodInfo.Invoke(vmContext.Instance, new object[] { accessToken as SecurityToken });

      return next(vmContext);
   }
}
```

</if>
