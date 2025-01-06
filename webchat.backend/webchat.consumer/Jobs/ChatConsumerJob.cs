using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using webchat.crosscutting.Domain;
using webchat.crosscutting.Kafka;

namespace webchat.consumer.Jobs;

internal sealed class ChatConsumerJob : BackgroundService
{
    private readonly ILogger<ChatConsumerJob> _logger;
    private readonly IConsumer<Ignore, string>? _consumer;

    public ChatConsumerJob(IChatKafka userKafka, ILogger<ChatConsumerJob> logger)
        : this(userKafka) =>
        _logger = logger;

    private ChatConsumerJob(IChatKafka userKafka)
    {
        _consumer = userKafka.GetConsumer();
        _consumer.Subscribe(userKafka.GetTopicName());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_consumer.Consume(TimeSpan.FromSeconds(5)) // waiting when doesn't have any message
                is var consumeResult && consumeResult is null)
                    continue;

                _logger.LogInformation($"Getting message: {consumeResult.Message.Value}");

                ChatMessage? user = JsonSerializer.Deserialize<ChatMessage>(consumeResult.Message.Value);

                _consumer.Commit(consumeResult);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"UserConsumerJob error: {ex.Message}");
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Application has finished");

        if (_consumer is not null)
        {
            _consumer.Close(); // Close  the cosumer 
            _consumer.Dispose(); // dispose the resources
        }

        return Task.CompletedTask;
    }
}