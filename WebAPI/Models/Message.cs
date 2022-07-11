namespace WebAPI.Models;

public class MessageDto
{
    public string Payload { get; set; }
    public string Topic { get; set; }
    public DateTime Time { get; set; }
}