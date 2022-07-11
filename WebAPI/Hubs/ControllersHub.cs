using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Hubs;

public class ControllersHub : Hub
{
    private readonly MqttFactory _mqttFactory;
    private readonly IConnections _connections;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ControllersHub> _logger;
    private readonly Queue<MessageDto> _messages;
    private List<string> _subscriptions;
    
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
        _messages = new Queue<MessageDto>();
        _subscriptions = new List<string>();
    }

    public async Task GetUpdates(string token)
    {
        var cts = new CancellationTokenSource();
        
        var userId = new JwtSecurityTokenHandler()
            .ReadJwtToken(token).Claims
            .First(claim => claim.Type == "sub").Value;
        
        _logger.LogInformation($"{userId} has been connected");
        
        var subs = await _context.Subscriptions
            .Where(u => u.UserId == userId)
            .ToListAsync(cancellationToken: cts.Token);
        
        var mqttClient = _mqttFactory.CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var topic = e.ApplicationMessage.Topic;
            _messages.Enqueue(new MessageDto
            {
                Payload = payload,
                Topic = topic,
                Time = DateTime.Now
            });

            return Task.CompletedTask;
        };

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("0df1d148747b496d85f8ca59339e72a9.s2.eu.hivemq.cloud")
            .WithClientId(Context.ConnectionId)
            .WithCredentials("user1", "user1234")
            .WithTls()
            .WithCleanSession()
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, cts.Token);
        
        // создать сокет и подписаться на все топики пользователя
        foreach (var sub in subs)
        {
            var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic(sub.Topic); })
                .Build();
        
            await mqttClient.SubscribeAsync(mqttSubscribeOptions, cts.Token);
        }
        
        while (true)
        {
            if (_messages.Count > 0)
                await Clients.Client(Context.ConnectionId)
                    .SendAsync("GetUpdates", _messages.Dequeue(), cts.Token);
        }
    }
    
    public async Task SubscribeToTopic(string topic, string token)
    {
        var userId = new JwtSecurityTokenHandler()
            .ReadJwtToken(token).Claims
            .First(claim => claim.Type == "sub").Value;
        
        if (_connections[userId] != null)
        {
            var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic(topic); })
                .Build();
        
            await _connections[userId]
                .SubscribeAsync(mqttSubscribeOptions);
            
            _logger.LogInformation($"{userId} has subscribed to {topic}");
        }
    }
    
    public async Task UnsubscribeFromTopic(string topic, string token)
    {
        var userId = new JwtSecurityTokenHandler()
            .ReadJwtToken(token).Claims
            .First(claim => claim.Type == "sub").Value;
        
        if (_connections[userId] != null)
        {
            var mqttUnsubscribeOptions = _mqttFactory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();
        
            await _connections[userId]
                .UnsubscribeAsync(mqttUnsubscribeOptions);
            
            _logger.LogInformation($"{userId} has unsubscribed to {topic}");
        }
    }

    public Task OnDisconnectedAsync(Exception? exception, string token)
    {
        var userId = new JwtSecurityTokenHandler()
            .ReadJwtToken(token).Claims
            .First(claim => claim.Type == "sub").Value;

        _connections.RemoveConnection(userId);
        
        _logger.LogInformation($"{userId} has disconnected");
        
        return base.OnDisconnectedAsync(exception);
    }
}