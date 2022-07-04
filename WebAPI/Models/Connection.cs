namespace WebAPI.Models;

public class Connection : IConnection
{
    public string UserId { get; set; }
    public string Topic { get; set; }
}