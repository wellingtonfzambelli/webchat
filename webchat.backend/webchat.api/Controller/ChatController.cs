using Microsoft.AspNetCore.Mvc;
using webchat.crosscutting.Domain;
using webchat.crosscutting.Kafka;
using webchat.crosscutting.SignalR;

namespace webchat.api.Controller;

public sealed class ChatController : ControllerBase
{
    private readonly IChatKafka _userKafka;
    private readonly ChatHubService _chatHubService;

    public ChatController(IChatKafka userKafka, ChatHubService chatHubService)
    {
        _userKafka = userKafka;
        _chatHubService = chatHubService;
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
            var type = CommunicationType.Directly;

            switch (type)
            {
                case CommunicationType.Kafka:
                    await _userKafka.ProduceAsync(request, cancellationToken);
                    break;
                case CommunicationType.RabbitMQ:
                    await _chatHubService.SendMessageAsync(request);
                    break;
                case CommunicationType.Directly:
                    await _chatHubService.SendMessageAsync(request);
                    break;
                default:
                    return BadRequest($"Unsupported communication type: {type}");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error by producing the user: {ex.Message}");
        }
    }
}