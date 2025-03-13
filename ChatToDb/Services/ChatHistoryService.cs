using ChatToDb.Chat;
using ChatToDb.DataLayer;
using Grpc.Core;

namespace ChatToDb.Services;

public class ChatHistoryService : ChatHistory.ChatHistoryBase
{
    private readonly ChatRepository _chatRepository;

    public ChatHistoryService(ChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }
    
    public override async Task<Reply> RequestHistory(Empty request, ServerCallContext context)
    {
        var messages = await _chatRepository.GetMessages();
        
        var reply = new Reply();

        foreach (var message in messages)
        {
            reply.Messages.Add(new ChatMessage
            {
                Username = message.Username,
                Text = message.Text,
                Time = message.Time
            });
        }
        
        return await Task.FromResult(reply);
    }
}
