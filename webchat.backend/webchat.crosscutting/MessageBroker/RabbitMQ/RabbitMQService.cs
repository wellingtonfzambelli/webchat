namespace webchat.crosscutting.MessageBroker.RabbitMQ;

public sealed class RabbitMQService : IRabbitMQService
{
    public async Task ProduceAsync(string message, CancellationToken ct)
    {
        throw new NotImplementedException("RabbitMQ");
    }
}