namespace webchat.crosscutting.MessageBroker;

public interface IMessageService
{
    Task ProduceAsync(string message, CancellationToken ct);
}