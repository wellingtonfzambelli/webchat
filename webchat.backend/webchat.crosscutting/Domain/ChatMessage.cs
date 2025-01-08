namespace webchat.crosscutting.Domain;

public sealed class ChatMessage
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public int AvatarId { get; set; }
    public string Message { get; set; }
    public DateTime SentAt { get; set; } = DateTime.Now;
}