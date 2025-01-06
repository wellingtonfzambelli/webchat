using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using webchat.crosscutting.Domain;

namespace webchat.crosscutting.Kafka;

public sealed class ChatKafka : IChatKafka
{
    private readonly ProducerConfig _producerConfig;

    private readonly ConsumerConfig _consumerConfig;
    private readonly IConsumer<Ignore, string> _consumer;

    private readonly string _topicName;
    private readonly ILogger<ChatKafka> _logger;

    public ChatKafka
    (
        string topicName,
        string bootstrapServers,
        string groupId,
        ILogger<ChatKafka> logger
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

    public async Task ProduceAsync(ChatMessage chatMessage, CancellationToken cancellationToken)
    {
        using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();

        try
        {
            var deliveryResult = await producer.ProduceAsync
                (
                    topic: _topicName,
                    new Message<Null, string>
                    {
                        Value = JsonSerializer.Serialize(chatMessage)
                    },
                    cancellationToken
                );

            _logger.LogInformation($"Delivered message to {deliveryResult.Value}, Offset: {deliveryResult.Offset}");
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError($"Delivery failed: {e.Error.Reason}");
        }

        producer.Flush(cancellationToken);
    }

    public IConsumer<Ignore, string> GetConsumer() =>
        _consumer;

    public string GetTopicName() =>
        _topicName;
}