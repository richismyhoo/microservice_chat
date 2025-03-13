using Grpc.Net.Client;
using ServicesChat;
using ServicesChat.Cache;
using ServicesChat.Chat;
using ServicesChat.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyMethod() 
            .AllowAnyHeader()
            .AllowCredentials(); 
    });
});

builder.Services.AddSignalR();

builder.Services.AddSingleton(provider =>
{
    var rabbitService = RabbitConnectionService.CreateAsync("localhost").GetAwaiter().GetResult();
    return rabbitService;
});

builder.Services.AddSingleton<RabbitProducer>();
builder.Services.AddGrpcClient<ChatHistory.ChatHistoryClient>(o =>
{
    o.Address = new Uri("http://localhost:5230");
}).ConfigureChannel(options =>
{
    options.HttpHandler = new HttpClientHandler
    {
        MaxResponseHeadersLength = 1024,
    };
});

builder.Services.AddSingleton<ChatHistoryService>();
builder.Services.AddSingleton<ChatHistoryCache>();

var app = builder.Build();

app.UseCors("AllowLocalhost");

app.MapHub<ChatHub>("/chat");

app.Run();