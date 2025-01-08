using Microsoft.AspNetCore.SignalR;

namespace webchat.crosscutting.SignalR;

public sealed class ChatHub : Hub
{
    private static readonly HashSet<string> OnlineUsers = new();

    public override Task OnConnectedAsync()
    {
        // Ad connected user 
        var userId = Context.User?.Identity?.Name ?? Context.ConnectionId;
        OnlineUsers.Add(userId);

        // Update to all connected clients
        Clients.All.SendAsync("UpdateOnlineUsers", OnlineUsers);
        
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
}