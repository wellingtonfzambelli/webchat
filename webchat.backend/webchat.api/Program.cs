using webchat.crosscutting.Domain;
using webchat.crosscutting.Kafka;
using webchat.crosscutting.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IChatKafka>(p =>
    new ChatKafka(
        builder.Configuration["kafkaConfig:TopicName"],
        builder.Configuration["kafkaConfig:BootstrapServer"],
        builder.Configuration["kafkaConfig:GroupId"],
        p.GetService<ILogger<ChatKafka>>()
    )
);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("OriginsPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration["AllowOrigins"].ToString())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// SignalR
builder.Services.AddSignalR();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/chat", async (ChatMessage request, IChatKafka userKafka, CancellationToken cancellationToken) =>
{
    try
    {
        await userKafka.ProduceAsync(request, cancellationToken);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error by producing the user: {ex.Message}");
    }
})
.WithOpenApi();

// Enable Static frontend app
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToController("Index", "Fallback");


// CORS
app.UseCors("OriginsPolicy");

// SignalR
app.MapHub<ChatHub>("/hub/chat");

app.Run();