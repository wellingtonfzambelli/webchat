using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using webchat.consumer.SignalR;

namespace webchat.consumer.Jobs;

internal sealed class RabbitMQConsumerJob : BackgroundService
{
    private readonly IProducerSingalR _producerSignalR;
    private readonly ILogger<KafkaConsumerJob> _logger;

    public RabbitMQConsumerJob
    (
        IProducerSingalR producerSignalR,
        ILogger<KafkaConsumerJob> logger
    )
    {
        _producerSignalR = producerSignalR;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KafkaConsumerJob has started");

        throw new NotImplementedException();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Application has finished");


        return Task.CompletedTask;
    }
}