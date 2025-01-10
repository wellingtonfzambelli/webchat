using webchat.crosscutting.Domain;

namespace webchat.crosscutting.SignalR;

public interface IChatHubService
{
    Task SendMessageAsync(ChatMessage chatMessage);
}