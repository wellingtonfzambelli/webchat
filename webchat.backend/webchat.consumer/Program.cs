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

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<KafkaSettings>>().Value);

builder.Services.Configure<CommunicationTypeSettings>(builder.Configuration.GetSection("CommunicationTypeSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<CommunicationTypeSettings>>().Value);
var communicationType = builder.Configuration.GetSection("CommunicationTypeSettings").Get<CommunicationTypeSettings>()?.Type;

if (communicationType == CommunicationType.Kafka)
    builder.Services.AddHostedService<KafkaConsumerJob>();
else if (communicationType == CommunicationType.RabbitMQ)
    builder.Services.AddHostedService<KafkaConsumerJob>();


builder.Services.AddTransient<IProducerSingalR>(s =>
    new ProducerSingalR(builder.Configuration["AddressHubConnection"] ?? throw new Exception())
);

builder.Services.AddTransient<IKafkaService>(p =>
    new KafkaService(
        p.GetService<KafkaSettings>(),
        p.GetService<ILogger<KafkaService>>()
    )
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();



using var host = builder.Build();
await host.RunAsync();