using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using webchat.crosscutting.Settings;

namespace webchat.crosscutting.MessageBroker.Kafka;

public sealed class KafkaService : IKafkaService
{
    private readonly ProducerConfig _producerConfig;
    private readonly ConsumerConfig _consumerConfig;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<KafkaService> _logger;

    public KafkaService
    (
        KafkaSettings kafkaSettings,
        ILogger<KafkaService> logger
    )
    {
        _kafkaSettings = kafkaSettings;

        _producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServer,
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };

        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServer,
            GroupId = kafkaSettings.GroupId,
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
                    topic: _kafkaSettings.TopicName,
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
        _kafkaSettings.TopicName;
}