using System.Text;
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
    
    public async Task GetUpdates()
    {
        var cts = new CancellationTokenSource();
        _logger.LogInformation($"{userId} has been connected");
        
        var subs = await _context.Subscriptions
            .Where(u => u.UserId == userId)
            .ToListAsync(cancellationToken: cts.Token);

        var res = new
        {
            userId = userId,
            topics = subs
        };
        
        await Clients.Client(Context.ConnectionId).SendAsync(res.ToString(), cancellationToken: cts.Token);
        
        var mqttClient = _mqttFactory.CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            _messages.Enqueue(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

            return Task.CompletedTask;
        };

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("0df1d148747b496d85f8ca59339e72a9.s2.eu.hivemq.cloud")
            .WithClientId(Context.ConnectionId)
            .WithCredentials("user1", "user1234")
            .WithTls()
            .WithCleanSession()
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions);
        
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
            if (_messages.TryDequeue(out var message))
            {
                Console.WriteLine(message);
                await Clients.Client(Context.ConnectionId).SendAsync("GetUpdates", message, cancellationToken: cts.Token);
            }
        }
    }
    
    public async Task SubscribeToTopic(string topic, CancellationToken ct)
    {
        if (_connections[Context.User.Identity.Name] != null)
        {
            var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic(topic); })
                .Build();
        
            await _connections[Context.User.Identity.Name]
                .SubscribeAsync(mqttSubscribeOptions, ct);
            
            _logger.LogInformation($"{userId} has subscribed to {topic}");
        }
    }
    
    public async Task UnsubscribeFromTopic(string topic, CancellationToken ct)
    {
        if (_connections[Context.User.Identity.Name] != null)
        {
            var mqttUnsubscribeOptions = _mqttFactory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();
        
            await _connections[Context.User.Identity.Name]
                .UnsubscribeAsync(mqttUnsubscribeOptions, ct);
            
            _logger.LogInformation($"{userId} has unsubscribed to {topic}");
        }
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"{userId} has been disconnected");
        
        return base.OnDisconnectedAsync(exception);
    }
}