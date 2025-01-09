using webchat.crosscutting.Domain;

namespace webchat.consumer.SignalR;

internal interface IProducerSingalR
{
    Task SendMessageAsync(ChatMessage chatMessage);
}