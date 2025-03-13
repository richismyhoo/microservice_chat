namespace ServicesChat.Models;

public class Message
{
    public string Username { get; set; }
    public string Text { get; set; }
    public string Time { get; set; }
    

    public Message(string username, string text, string time)
    {
        Username = username;
        Text = text;
        Time = time;
    }
}