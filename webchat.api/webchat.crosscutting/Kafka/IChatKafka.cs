using Confluent.Kafka;
using webchat.crosscutting.Domain;

namespace webchat.crosscutting.Kafka;

public interface IChatKafka
{
    Task ProduceAsync(ChatMessage chatMessage, CancellationToken cancellationToken);
    IConsumer<Ignore, string> GetConsumer();
    string GetTopicName();
}