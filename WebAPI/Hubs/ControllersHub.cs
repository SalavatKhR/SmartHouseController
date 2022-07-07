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
    private string _lastMessage;
    private string _lastSentMessage;
    
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
        _lastMessage = string.Empty;
        _lastSentMessage = string.Empty;
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
            _lastMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

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
            if (!string.IsNullOrEmpty(_lastMessage) & _lastMessage != _lastSentMessage)
            {
                Console.WriteLine(_lastMessage);
                _lastSentMessage = _lastMessage;
                await Clients.Client(Context.ConnectionId).SendAsync("GetUpdates", _lastMessage);
            }

            await Task.Delay(5000);
        }
    }
    
    public async Task SubscribeToTopic(string topic)
    {
        var mqttClient = _mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost")
            .WithClientId(userId)
            .WithCredentials("user1", "user1234")
            .WithCleanSession()
            .Build();
        
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Clients.Caller.SendAsync(JsonSerializer.Serialize(e));
            
            return Task.CompletedTask;
        };

        _context.Subscriptions.Add(new Subscription
        {
            UserId = userId,
            Topic = topic
        });

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
    
        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => { f.WithTopic(topic); })
            .Build();
        
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    }
    
    public Task UnsubscribeFromTopic(string topic)
    {
        var record = _context.Subscriptions
            .FirstOrDefaultAsync(u => u.Topic == topic & u.UserId == userId);

        _context.Remove(record);

        _context.SaveChanges();

        return Task.CompletedTask;
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.RemoveConnection(userId);
        
        _logger.LogInformation($"{userId} has been disconnected");
        
        return base.OnDisconnectedAsync(exception);
    }
}