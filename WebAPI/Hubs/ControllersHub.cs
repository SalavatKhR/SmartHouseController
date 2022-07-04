using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using MQTTnet;
using MQTTnet.Client;
using WebAPI.Models;

namespace WebAPI.Hubs;

// [Authorize]
public class ControllersHub : Hub
{
    private readonly IConnections _connections;
    private readonly MqttFactory _mqttFactory;
    private readonly Subscriptions _subscriptions;

    public ControllersHub(
        MqttFactory mqttFactory,
        IConnections connections,
        Subscriptions subscriptions)
    {
        _mqttFactory = mqttFactory;
        _connections = connections;
        _subscriptions = subscriptions;
    }

    public async Task SubscribeToTopic(string topic)
    {
        // var user = Context.User.Claims.First(claim => claim.Type == "email").Value;
        
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

        // if (_subscriptions.Collection[user] == null)
        //     _subscriptions.Collection[user] = new List<string> { topic };
        // else 
        //     _subscriptions.Collection[user].Add(topic);
        
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => { f.WithTopic(topic); })
            .Build();
        
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        
        _connections.AddConnection(Context.ConnectionId, (MqttClient) mqttClient);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.RemoveConnection(Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
    }
}