using Microsoft.AspNetCore.SignalR;
using ServicesChat.Cache;
using ServicesChat.Models;
using ServicesChat.Services;

namespace ServicesChat.Chat;

public class ChatHub : Hub
{
    private readonly RabbitProducer _rabbitProducer;
    private readonly ChatHistoryCache _chatHistoryCache;

    public ChatHub(RabbitProducer rabbitProducer, ChatHistoryCache chatHistoryCache)
    {
        _rabbitProducer = rabbitProducer;
        _chatHistoryCache = chatHistoryCache;
    }
    
    public async Task Send(string message, string username)
    {
        var timestamp = DateTime.Now;
        
        await Clients.All.SendAsync("Receive", message, username, timestamp.ToString("HH:mm:ss"));
        await _rabbitProducer.SendMessage(message, username, timestamp);
        _chatHistoryCache.AddMessage(new Message(username, message, timestamp.ToString("HH:mm:ss")));
    }

    public async Task SendHistory()
    {
        Console.WriteLine("Sending history");

        var messages = await _chatHistoryCache.GetMessagesAsync();
        await Clients.Caller.SendAsync("ReceiveMessageHistory", messages);
    }

    public override async Task OnConnectedAsync()
    {
        await SendHistory();
        await base.OnConnectedAsync();
    }
}