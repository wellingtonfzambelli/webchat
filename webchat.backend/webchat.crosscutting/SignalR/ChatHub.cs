using Microsoft.AspNetCore.SignalR;

namespace webchat.crosscutting.SignalR;

public sealed class ChatHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public async Task SendMessage(string order)
    {
        await Clients.All.SendAsync("ChatHub", order);
    }
}