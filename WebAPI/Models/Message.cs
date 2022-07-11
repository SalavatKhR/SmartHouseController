namespace WebAPI.Models;

public class Message
{
    public string Payload { get; set; }
    public string Topic { get; set; }
    public DateTime Time { get; set; }
}