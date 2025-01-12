using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using webchat.crosscutting.Domain;
using webchat.crosscutting.MessageBroker.Kafka;
using webchat.crosscutting.MessageBroker.RabbitMQ;
using webchat.crosscutting.Settings;
using webchat.crosscutting.SignalR;

namespace webchat.api.Controller;

public sealed class ChatController : ControllerBase
{
    private readonly IKafkaService _kafkaService;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IChatHubService _chatHubService;
    private readonly CommunicationType _communicationType;


    public ChatController
    (
        IKafkaService kafkaService,
        IRabbitMQService rabbitMQService,
        IChatHubService chatHubService,
        CommunicationTypeSettings communicationTypeSettings
    )
    {
        _kafkaService = kafkaService;
        _chatHubService = chatHubService;
        _communicationType = communicationTypeSettings.Type;

    }

    [HttpPost]
    [Route("chat")]
    public async Task<IActionResult> SendMessageAsync
    (
        [FromBody] ChatMessage request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            string message = JsonSerializer.Serialize(request);

            switch (_communicationType)
            {
                case CommunicationType.Kafka:
                    await _kafkaService.ProduceAsync(message, cancellationToken);
                    break;
                case CommunicationType.RabbitMQ:
                    await _rabbitMQService.ProduceAsync(message, cancellationToken);
                    break;
                case CommunicationType.Directly:
                    await _chatHubService.SendMessageAsync(request);
                    break;
                default:
                    return BadRequest($"Unsupported communication type: {_communicationType}");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error by producing the user: {ex.Message}");
        }
    }
}