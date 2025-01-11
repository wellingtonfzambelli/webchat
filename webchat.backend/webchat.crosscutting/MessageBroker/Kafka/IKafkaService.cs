using Confluent.Kafka;

namespace webchat.crosscutting.MessageBroker.Kafka;

public interface IKafkaService : IMessageService
{
    IConsumer<Ignore, string> GetConsumer();
    string GetTopicName();
}