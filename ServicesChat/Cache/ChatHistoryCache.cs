using Microsoft.AspNetCore.Mvc.TagHelpers;
using ServicesChat.Models;
using ServicesChat.Services;

namespace ServicesChat.Cache;

public class ChatHistoryCache
{
    private readonly ChatHistoryService _chatHistoryService;

    public ChatHistoryCache(ChatHistoryService chatHistoryService)
    {
        _chatHistoryService = chatHistoryService;
    }

    private List<Message> messages = new();

    public async Task<List<Message>> GetMessagesAsync()
    {
        if (messages.Count == 0)
        {
            var history = await _chatHistoryService.RequestChatHistory();
            messages = history ?? new List<Message>();
        }
        return messages;
    }

    public void AddMessage(Message message)
    {
        messages.Add(message);
    }
}