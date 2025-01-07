using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using webchat.crosscutting.Domain;
using webchat.crosscutting.Kafka;

namespace webchat.consumer.Jobs;

internal class ChatConsumerJob : BackgroundService
{
    private readonly IConsumer<Ignore, string>? _kafkaConsumer;
    //private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<ChatConsumerJob> _logger;

    public ChatConsumerJob(IChatKafka userKafka/*, IHubContext<ChatHub> hubContext*/, ILogger<ChatConsumerJob> logger)
        : this(userKafka)
    {
        // _hubContext = hubContext;
        _logger = logger;
    }

    private ChatConsumerJob(IChatKafka userKafka)
    {
        _kafkaConsumer = userKafka.GetConsumer();
        _kafkaConsumer.Subscribe(userKafka.GetTopicName());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_kafkaConsumer.Consume(TimeSpan.FromSeconds(5)) // waiting when doesn't have any message
                is var consumeResult && consumeResult is null)
                    continue;

                _logger.LogInformation($"Getting message: {consumeResult.Message.Value}");

                ChatMessage? user = JsonSerializer.Deserialize<ChatMessage>(consumeResult.Message.Value);

                //await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, stoppingToken);

                _kafkaConsumer.Commit(consumeResult);
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

        if (_kafkaConsumer is not null)
        {
            _kafkaConsumer.Close(); // Close  the cosumer 
            _kafkaConsumer.Dispose(); // dispose the resources
        }

        return Task.CompletedTask;
    }
}