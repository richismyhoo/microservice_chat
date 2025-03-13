using ChatToDb.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatToDb.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; }
}