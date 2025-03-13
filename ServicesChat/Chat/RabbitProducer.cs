using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using ServicesChat.Models;

namespace ServicesChat.Chat;

public class RabbitProducer
{
    private readonly RabbitConnectionService _rabbitConnectionService;

    public RabbitProducer(RabbitConnectionService rabbitConnectionService)
    {
        _rabbitConnectionService = rabbitConnectionService;
    }
    public async Task SendMessage(string message, string username, DateTime time)
    {
        var channel = _rabbitConnectionService.Channel;

        var chatMessage = new Message(username, message, time.ToString("HH:mm:ss"));
        var jsonMessage = JsonSerializer.Serialize(chatMessage);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        await channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: "chat",
            body: body);
        
        Console.WriteLine($" [x] Sent {chatMessage.Username}: {chatMessage.Text}");
    }
}