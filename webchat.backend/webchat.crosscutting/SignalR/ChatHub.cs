using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace webchat.crosscutting.SignalR;

public sealed class ChatHub : Hub
{
    private static readonly HashSet<string> OnlineUsers = new();

    public override Task OnConnectedAsync()
    {
        if(Context.Features?.Get<IHttpContextFeature>()?.HttpContext 
            is var httpContext && httpContext is not null)
        {
            string json = ConvertJson(httpContext);

            OnlineUsers.Add(json);

            // Update to all connected clients
            Clients.All.SendAsync("UpdateOnlineUsers", OnlineUsers);
        }
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // Remove disconnected user
        var userId = Context.User?.Identity?.Name ?? Context.ConnectionId;
        OnlineUsers.Remove(userId);

        // Update to all connected clients
        Clients.All.SendAsync("UpdateOnlineUsers", OnlineUsers);

        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string order)
    {
        await Clients.All.SendAsync("ChatHub", order);
    }

    private string ConvertJson(HttpContext httpContext)
    {
        var userName = httpContext.Request.Query["userName"];
        var userId = httpContext.Request.Query["userId"];
        var avatarId = httpContext.Request.Query["avatarId"];

        var userInfo = new
        {
            UserName = userName,
            UserId = userId,
            AvatarId = avatarId
        };

        return JsonSerializer.Serialize(userInfo);
    }
}