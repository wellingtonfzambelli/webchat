using webchat.crosscutting.Domain;
using webchat.crosscutting.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IChatKafka>(p =>
    new ChatKafka(
        builder.Configuration["kafkaConfig:TopicName"],
        builder.Configuration["kafkaConfig:BootstrapServer"],
        builder.Configuration["kafkaConfig:GroupId"],
        p.GetService<ILogger<ChatKafka
        >>()
    )
);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/chat-message", async (ChatMessage request, IChatKafka chatKafka, CancellationToken cancellationToken) =>
{
    try
    {
        await chatKafka.ProduceAsync(request, cancellationToken);
        return Results.Ok($"Message produced succefully: {request.UserName}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error by producing the user: {ex.Message}");
    }
})
.WithOpenApi();

app.Run();