using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace webchat.crosscutting.MessageBroker.Kafka;

public sealed class KafkaService : IKafkaService
{
    private readonly ProducerConfig _producerConfig;
    private readonly ConsumerConfig _consumerConfig;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topicName;
    private readonly ILogger<KafkaService> _logger;

    public KafkaService
    (
        string topicName,
        string bootstrapServers,
        string groupId,
        ILogger<KafkaService> logger
    )
    {
        _topicName = topicName;

        _producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };

        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest // removes from the topic
        };
        _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();

        _logger = logger;
    }

    public async Task ProduceAsync(string message, CancellationToken ct)
    {
        using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();

        try
        {
            var deliveryResult = await producer.ProduceAsync
                (
                    topic: _topicName,
                    new Message<Null, string>
                    {
                        Value = message
                    },
                    ct
                );

            _logger.LogInformation($"Delivered message to {deliveryResult.Value}, Offset: {deliveryResult.Offset}");
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError($"Delivery failed: {e.Error.Reason}");
        }

        producer.Flush(ct);
    }

    public IConsumer<Ignore, string> GetConsumer() =>
        _consumer;

    public string GetTopicName() =>
        _topicName;
}