<if view>
##### ChatRoom.razor

```jsx
<VMContext VM="ChatRoomVM" TState="IChatRoomState" OnStateChange="UpdateState">
@if (state != null)
{
    <StyleSheet Context="this">
        <div>
            <div class="chatPanel">
                <nav>
                    @if (state.Users != null)
                    {
                        @foreach(var user in state.Users)
                        {
                            <p>
                                <b class="@(user.CorrelationId == state.CorrelationId ? "myself" : "")">@user.Name</b>
                                <span>@user.IpAddress</span>
                                <span>@user.Browser</span>
                            </p>
                        }
                    }
                </nav>
                <section>
                <div>
                    @if (state.Messages != null)
                    {
                        @foreach (var msg in state.Messages)
                        {
                            <div>
                                <div>
                                    <span>@(GetUserName(msg.UserId) ?? msg.UserName)</span>
                                    <span>@msg.Date.ToString("g")</span>
                                </div>
                                <div class="@(msg.Private ? "private" : "")">@msg.Text</div>
                            </div>
                        }
                    }
                        </div>
                <div style="float: left; clear: both;" />
                <input type="text" class="form-control" placeholder="Type your message here" @bind="message" @bind:event="oninput" @onkeypress="e => OnKeypress(e)" />
                </section>
            </div>
            <footer>
                <div>* Hint:</div>
                <ul>
                <li>type 'my name is ___' to introduce yourself</li>
                <li>type '&lt;username&gt;: ___' to send private message</li>
                </ul>
            </footer>
        </div>
    </StyleSheet> 
}
</VMContext>

@code {
    private IChatRoomState state;
    private string message;
    private string correlationId;

    public class User 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CorrelationId { get; set; }
        public string IpAddress { get; set; }
        public string Browser { get; set; }
    }

    public class Message
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Text { get; set; }
        public bool Private { get; set; }
    }

    public interface IChatRoomState
    {
        string CorrelationId { get; set; }
        List<User> Users { get; set;  }
        List<Message> Messages { get; set; }
        Message PrivateMessage { get; set; }

        void AddUser(string correlationId);
        void SendMessage(Message message);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (this.state != null && this.correlationId == null)
        {
            this.correlationId = new Random().Next().ToString();
            this.state.AddUser(this.correlationId);
        }

    }

    private string GetUserName(string userId) 
    {
        var user = this.state.Users?.FirstOrDefault(x => x.Id == userId);
        return user?.Name;
    }

    private void OnKeypress(KeyboardEventArgs e) 
    {
        if (e.Key == "Enter")
        {
            var match = Regex.Match(this.message, "name is ([A-z]+)", RegexOptions.IgnoreCase);
            this.state.SendMessage(new Message
            {
                Text = this.message,
                Date = DateTimeOffset.Now,
                UserName = match.Success ? match.Groups[1].Value : ""
            });
            this.message = "";
        }
    }

    private void UpdateState(IChatRoomState state)
    {
        this.state = state;

        if (this.state.PrivateMessage != null) 
        {
            var message = state.PrivateMessage;
            message.Text = "(private) " + message.Text;
            message.Private = true;
            this.state.Messages.Add(message);
            this.state.PrivateMessage = null;
        }

        StateHasChanged();
    }
}
```

</if>
<if viewmodel>
##### ChatRoom.cs

```csharp
public class ChatMessage
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Text { get; set; }
}

public class ChatUser
{
    private static int _counter = 0;

    public string Id { get; set; }
    public string CorrelationId { get; set; }
    public string Name { get; set; }
    public string IpAddress { get; set; }
    public string Browser { get; set; }

    public ChatUser(IConnectionContext connectionContext, string correlationId)
    {
        Id = connectionContext.ConnectionId;
        CorrelationId = correlationId;
        Name = $"user{Interlocked.Increment(ref _counter)}";
        IpAddress = connectionContext.HttpConnection.RemoteIpAddress.ToString();

        var browserInfo = Parser.GetDefault().Parse(connectionContext.HttpRequestHeaders.UserAgent);
        if (browserInfo != null)
            Browser = $"{browserInfo.UA.Family}/{browserInfo.OS.Family} {browserInfo.OS.Major}";
    }
}

public class ChatRoomVM : MulticastVM
{
    private readonly IConnectionContext _connectionContext;

    [ItemKey(nameof(ChatMessage.Id))]
    public List<ChatMessage> Messages { get; } = new List<ChatMessage>();

    [ItemKey(nameof(ChatUser.Id))]
    public List<ChatUser> Users { get; } = new List<ChatUser>();

    public ChatRoomVM(IConnectionContext connectionContext)
    {
        _connectionContext = connectionContext;
    }

    public void SendMessage(ChatMessage chat)
    {
        string userId = _connectionContext.ConnectionId;
        chat.Id = Messages.Count + 1;
        chat.UserId = userId;
        chat.UserName = UpdateUserName(userId, chat.UserName);

        var privateMessageUser = Users.FirstOrDefault(x => chat.Text.StartsWith($"{x.Name}:"));
        if (privateMessageUser != null)
            base.Send(new List<string> { privateMessageUser.Id, userId }, "PrivateMessage", chat);
        else
        {
            lock (Messages)
            {
                Messages.Add(chat);
                this.AddList(nameof(Messages), chat);
            }
        }
    }

    public void AddUser(string correlationId)
    {
        var user = new ChatUser(_connectionContext, correlationId);
        lock (Users)
        {
            Users.Add(user);
            this.AddList(nameof(Users), user);
        }
    }

    public void RemoveUser()
    {
        lock (Users)
        {
            var user = Users.FirstOrDefault(x => x.Id == _connectionContext.ConnectionId);
            if (user != null)
            {
                Users.Remove(user);
                this.RemoveList(nameof(Users), user.Id);
            }
        }
    }

    public override void Dispose()
    {
        RemoveUser();
        PushUpdates();
        base.Dispose();
    }

    private string UpdateUserName(string userId, string userName)
    {
        lock (Users)
        {
            var user = Users.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    user.Name = userName;
                    this.UpdateList(nameof(Users), user);
                }
                return user.Name;
            }
        }
        return userId;
    }
}
```

</if>
