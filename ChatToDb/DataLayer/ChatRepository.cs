using ChatToDb.Context;
using ChatToDb.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatToDb.DataLayer;

public class ChatRepository
{
    private readonly ApplicationDbContext _context;

    public ChatRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task AddMessage(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Message>> GetMessages()
    {
        return await _context.Messages.ToListAsync();
    }
}