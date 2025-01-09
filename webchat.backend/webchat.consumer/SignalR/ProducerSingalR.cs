using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using webchat.crosscutting.Domain;

namespace webchat.consumer.SignalR;

internal sealed class ProducerSingalR : IProducerSingalR
{
    private readonly HubConnection _hubConnection;

    public ProducerSingalR(string hubAddress)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubAddress)
            .Build();
    }

    public async Task SendMessageAsync(ChatMessage chatMessage)
    {
        await _hubConnection.StartAsync();
        await _hubConnection.SendAsync("SendMessage", JsonSerializer.Serialize(chatMessage));
        await _hubConnection.StopAsync();
    }
}