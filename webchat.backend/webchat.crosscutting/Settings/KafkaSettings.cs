namespace webchat.crosscutting.Settings;

public class KafkaSettings
{
    public string BootstrapServer { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
}