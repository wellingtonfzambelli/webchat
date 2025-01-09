using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace webchat.crosscutting.SignalR;

public sealed class ChatHub : Hub
{
    private static readonly HashSet<string> OnlineUsers = new HashSet<string>();

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
        if (Context.Features?.Get<IHttpContextFeature>()?.HttpContext
           is var httpContext && httpContext is not null)
        {
            var userId = httpContext.Request.Query["userId"];

            RemoveUserById(userId);

            // Update to all connected clients
            Clients.All.SendAsync("UpdateOnlineUsers", OnlineUsers);
        }

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

    private void RemoveUserById(string userId)
    {        
        var userToRemove = string.Empty;
        foreach (var userJson in OnlineUsers)
        {
            if (userJson.Contains($"\"UserId\":[\"{userId}\"]"))
            {
                userToRemove = userJson;
                break;
            }
        }
        
        if (!string.IsNullOrEmpty(userToRemove))
            OnlineUsers.Remove(userToRemove);
    }
}