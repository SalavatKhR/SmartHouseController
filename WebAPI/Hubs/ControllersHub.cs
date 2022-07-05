using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Hubs;

[Authorize]
public class ControllersHub : Hub
{
    private readonly MqttFactory _mqttFactory;
    private readonly IConnections _connections;
    private readonly ApplicationDbContext _context;
    public ControllersHub(
        MqttFactory mqttFactory,
        IConnections connections,
        ApplicationDbContext context)
    {
        _mqttFactory = mqttFactory;
        _connections = connections;
        _context = context;
    }
    
    public override async Task OnConnectedAsync()
    {
        var subs = await _context.Subscriptions
            .Where(u => u.UserId == Context.User.Identity.Name)
            .ToListAsync();
        
        var res = JsonSerializer.Serialize(new
        {
            userId = Context.User.Identity.Name,
            topics = subs
        });
        
        var mqttClient = _mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("0df1d148747b496d85f8ca59339e72a9.s2.eu.hivemq.cloud")
            .WithClientId(Context.ConnectionId)
            .WithCredentials("user1", "user1234")
            .WithTls()
            .WithCleanSession()
            .Build();
        
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Clients.Caller.SendAsync(JsonSerializer.Serialize(e));
            
            return Task.CompletedTask;
        };
    
        // создать сокет и подписаться на все топики пользователя
        foreach (var sub in subs)
        {
            var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic(sub.Topic); })
                .Build();
        
            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        }
        
        _connections.AddConnection(Context.User.Identity.Name, mqttClient);
        
        await Clients.Caller.SendAsync(res);
    }
    
    public async Task SubscribeToTopic(string topic)
    {
        var mqttClient = _mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("0df1d148747b496d85f8ca59339e72a9.s2.eu.hivemq.cloud")
            .WithClientId(Context.ConnectionId)
            .WithCredentials("user1", "user1234")
            .WithTls()
            .WithCleanSession()
            .Build();
        
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Clients.Caller.SendAsync(JsonSerializer.Serialize(e));
            
            return Task.CompletedTask;
        };

        _context.Subscriptions.Add(new Subscription
        {
            UserId = Context.User.Identity.Name,
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
            .FirstOrDefaultAsync(u => u.Topic == topic & u.UserId == Context.User.Identity.Name);

        _context.Remove(record);

        _context.SaveChanges();

        return Task.CompletedTask;
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.RemoveConnection(Context.ConnectionId);
        
        return base.OnDisconnectedAsync(exception);
    }
}