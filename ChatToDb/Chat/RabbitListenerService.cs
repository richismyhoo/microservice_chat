using System.Text;
using System.Text.Json;
using ChatToDb.Chat;
using ChatToDb.DataLayer;
using ChatToDb.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitListenerService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly RabbitConnectionService _rabbitConnectionService;

    public RabbitListenerService(IServiceScopeFactory serviceScopeFactory, RabbitConnectionService rabbitConnectionService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _rabbitConnectionService = rabbitConnectionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _rabbitConnectionService.Channel;

        Console.WriteLine(" [*] Waiting for messages...");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var codedMessage = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<Message>(codedMessage);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var chatRepository = scope.ServiceProvider.GetRequiredService<ChatRepository>();
                Console.WriteLine($" [x] Received {message.Username} {message.Text}");
                await chatRepository.AddMessage(message);
            }
            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync("chat", autoAck: true, consumer: consumer, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}