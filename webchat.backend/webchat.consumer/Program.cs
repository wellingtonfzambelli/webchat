using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using webchat.consumer.Jobs;
using webchat.crosscutting.Kafka;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ChatConsumerJob>();
builder.Services.AddTransient<IChatKafka>(p =>
    new ChatKafka(
        builder.Configuration["kafkaConfig:TopicName"],
        builder.Configuration["kafkaConfig:BootstrapServer"],
        builder.Configuration["kafkaConfig:GroupId"],
        p.GetService<ILogger<ChatKafka>>()
    )
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();



using var host = builder.Build();
await host.RunAsync();