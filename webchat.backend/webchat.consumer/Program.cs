using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using webchat.consumer.Jobs;
using webchat.consumer.SignalR;
using webchat.crosscutting.Kafka;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ChatConsumerJob>();
builder.Services.AddTransient<IProducerSingalR>(s =>
    new ProducerSingalR(builder.Configuration["AddressHubConnection"] ?? throw new Exception())
);

builder.Services.AddTransient<IChatKafka>(p =>
    new ChatKafka(
        builder.Configuration["kafkaConfig:TopicName"] ?? throw new Exception(),
        builder.Configuration["kafkaConfig:BootstrapServer"] ?? throw new Exception(),
        builder.Configuration["kafkaConfig:GroupId"] ?? throw new Exception(),
        p.GetService<ILogger<ChatKafka>>()
    )
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();



using var host = builder.Build();
await host.RunAsync();