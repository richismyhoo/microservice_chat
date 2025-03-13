using RabbitMQ.Client;

namespace ServicesChat.Chat;

public class RabbitConnectionService : IDisposable
{
    private IConnection _connection;
    private IChannel _channel;
    
    private RabbitConnectionService() { }

    public static async Task<RabbitConnectionService> CreateAsync(string hostName)
    {
        var service = new RabbitConnectionService();
        await service.ConnectAsync(hostName);
        return service;
    }
    
    private async Task ConnectAsync(string hostName)
    {
        var factory = new ConnectionFactory() { HostName = hostName };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await DeclareQueue("chat");
        await DeclareQueue("chatHistory");
    }

    public IChannel Channel => _channel;
    
    private async Task DeclareQueue(string queueName)
    {
        await _channel.QueueDeclareAsync(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}