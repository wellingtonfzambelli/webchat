using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using webchat.consumer.Jobs;
using webchat.consumer.SignalR;
using webchat.crosscutting.Domain;
using webchat.crosscutting.MessageBroker.Kafka;
using webchat.crosscutting.Settings;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

const string COMMUNICATION_TYPE_SETTINGS_NAME = "CommunicationTypeSettings";
builder.Services.Configure<CommunicationTypeSettings>(builder.Configuration.GetSection(COMMUNICATION_TYPE_SETTINGS_NAME));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<CommunicationTypeSettings>>().Value);
var communicationSettings = builder.Configuration.GetSection(COMMUNICATION_TYPE_SETTINGS_NAME).Get<CommunicationTypeSettings>();

if (communicationSettings.Type == CommunicationType.Kafka)
    builder.Services.AddHostedService<KafkaConsumerJob>();
else if (communicationSettings.Type == CommunicationType.RabbitMQ)
    builder.Services.AddHostedService<KafkaConsumerJob>();


builder.Services.AddTransient<IProducerSingalR>(s =>
    new ProducerSingalR(builder.Configuration["AddressHubConnection"] ?? throw new Exception())
);

builder.Services.AddTransient<IKafkaService>(p =>
    new KafkaService(
        builder.Configuration["kafkaConfig:TopicName"] ?? throw new Exception(),
        builder.Configuration["kafkaConfig:BootstrapServer"] ?? throw new Exception(),
        builder.Configuration["kafkaConfig:GroupId"] ?? throw new Exception(),
        p.GetService<ILogger<KafkaService>>()
    )
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();



using var host = builder.Build();
await host.RunAsync();