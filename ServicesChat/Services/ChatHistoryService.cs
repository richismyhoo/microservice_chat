using Grpc.Net.Client;
using ServicesChat.Models;

namespace ServicesChat.Services;

public class ChatHistoryService
{
    private readonly ChatHistory.ChatHistoryClient _client;

    public ChatHistoryService(ChatHistory.ChatHistoryClient client)
    {
        _client = client;
    }

    public async Task<List<Message>> RequestChatHistory()
    {
        List<Message> messages = new();
        
        var reply = await _client.RequestHistoryAsync(new Empty());

        foreach (var msg in reply.Messages)
        {
            messages.Add(new Message(msg.Username, msg.Text, msg.Time));
        }

        return messages;
    }
}