using Microsoft.AspNetCore.SignalR.Client;
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

var hubConnection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5000/hub/chat")
    .Build();

try
{
    // Inicia a conexão
    await hubConnection.StartAsync();
    Console.WriteLine("Connected to SignalR Hub!");

    // Escuta mensagens enviadas pelo servidor
    hubConnection.On<string>("ChatHub", (message) =>
    {
        Console.WriteLine("Message from server: " + message);
    });

    // Envia uma mensagem para o hub
    await hubConnection.SendAsync("SendMessage", "Hello from Console App!");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao conectar: {ex.Message}");
}
finally
{
    // Certifique-se de parar a conexão quando terminar
    await hubConnection.StopAsync();
    Console.WriteLine("Conexão finalizada.");
}






builder.Logging.ClearProviders();
builder.Logging.AddConsole();

using var host = builder.Build();

await host.RunAsync();