using Microsoft.Extensions.Options;
using webchat.crosscutting.MessageBroker.Kafka;
using webchat.crosscutting.MessageBroker.RabbitMQ;
using webchat.crosscutting.Settings;
using webchat.crosscutting.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CommunicationTypeSettings>(builder.Configuration.GetSection("CommunicationTypeSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<CommunicationTypeSettings>>().Value);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<KafkaSettings>>().Value);


builder.Services.AddTransient<IRabbitMQService, RabbitMQService>();
builder.Services.AddSingleton<IChatHubService, ChatHubService>();
builder.Services.AddTransient<IKafkaService, KafkaService>();



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