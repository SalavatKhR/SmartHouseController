using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Hubs;

// [Authorize]
public class ControllersHub : Hub
{
    private readonly MqttFactory _mqttFactory;
    private readonly IConnections _connections;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ControllersHub> _logger;
    private readonly Queue<string?> _messages;

    private readonly string userId = "120877ed-84b9-4ed5-9b87-d78965fc4fe0";
    public ControllersHub(
        MqttFactory mqttFactory,
        IConnections connections,
        ApplicationDbContext context,
        ILogger<ControllersHub> logger)
    {
        _mqttFactory = mqttFactory;
        _connections = connections;
        _context = context;
        _logger = logger;
        _messages = new Queue<string?>();
    }
    
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"{userId} has been connected");
        
        var subs = await _context.Subscriptions
            .Where(u => u.UserId == userId)
            .ToListAsync();

        var res = new
        {
            userId = userId,
            topics = subs
        };
        
        await Clients.Client(Context.ConnectionId).SendAsync(res.ToString());
        
        var mqttClient = _mqttFactory.CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            _messages.Enqueue(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

             return Task.CompletedTask;
        };

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost")
            .WithClientId(Context.ConnectionId)
            .WithCleanSession()
            .Build();
        
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        
        // создать сокет и подписаться на все топики пользователя
        foreach (var sub in subs)
        {
            var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic(sub.Topic); })
                .Build();
        
            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        }
        
        _connections.AddConnection(userId, mqttClient);

        await GetUpdates();
    }

    private async Task GetUpdates()
    {
        while (true)
        {
            if (_messages.TryDequeue(out var message))
            {
                Console.WriteLine(message);
                await Clients.Client(Context.ConnectionId).SendAsync("GetUpdates", message);
            }
        }
    }
    
    public async Task SubscribeToTopic(string topic)
    {
        _context.Subscriptions.Add(new Subscription
        {
            UserId = userId,
            Topic = topic
        });

        await _context.SaveChangesAsync();
    }
    
    public async Task UnsubscribeFromTopic(string topic)
    {
        var record = _context.Subscriptions
            .FirstOrDefaultAsync(u => u.Topic == topic & u.UserId == userId);

        _context.Remove(record);

        await _context.SaveChangesAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.RemoveConnection(userId);
        
        _logger.LogInformation($"{userId} has been disconnected");
        
        return base.OnDisconnectedAsync(exception);
    }
}