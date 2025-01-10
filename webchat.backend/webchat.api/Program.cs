using webchat.crosscutting.Kafka;
using webchat.crosscutting.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IChatHubService, ChatHubService>();
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

// Enable Static frontend app
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToController("Index", "Fallback");


// CORS
app.UseCors("OriginsPolicy");

// SignalR
app.MapHub<ChatHubService>("/hub/chat");

app.Run();