using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using webchat.crosscutting.Domain;

namespace webchat.consumer.SignalR;

internal sealed class ProducerSingalR
{
    private readonly HubConnection _hubConnection;

    public ProducerSingalR()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/hub/chat")
            .Build();
    }

    public async Task SendMessageAsync(ChatMessage chatMessage)
    {
        await _hubConnection.StartAsync();
        await _hubConnection.SendAsync("SendMessage", JsonSerializer.Serialize(chatMessage));
        await _hubConnection.StopAsync();
    }
}