using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Serilog;
using SmartHouseController.MQTT.Broker.Server.Exceptions;
using SmartHouseController.MQTT.Broker.Server.Models;

namespace SmartHouseController.MQTT.Broker.Server.Handlers;

public static class ClientActionHandlers
{
    public static Task OnConnectedValidateAsync(ValidatingConnectionEventArgs e)
    {

        // TODO: validation
        
        return Task.CompletedTask;
    }

    public static Task OnConnectedAsync(ClientConnectedEventArgs e)
    {
        Log.Information($"id: '{e.ClientId}' has connected");

        return Task.CompletedTask;
    }

    public static Task OnDisconnectedAsync(ClientDisconnectedEventArgs e)
    {
        Log.Information($"{e.ClientId} has disconnected ({e.DisconnectType})");

        return Task.CompletedTask;
    }

    public static Task OnMessageLogAsync(InterceptingPublishEventArgs e)
    {
        var message = new MessageDto
            {
                ClientId = e.ClientId,
                Payload = e.ApplicationMessage?.Payload == null 
                    ? throw new EmptyFieldMessageRequestException("Payload")
                    : Encoding.UTF8.GetString(e.ApplicationMessage!.Payload),
                Topic = e.ApplicationMessage.Topic ?? throw new EmptyFieldMessageRequestException("Topic"),
            };
        
        Log.Logger.Information($"topic: {message.Topic}, payload: {message.Payload}");

        return Task.CompletedTask;
    }

    public static async Task OnInterceptPublishAsync(InterceptingPublishEventArgs e, IMqttClient mqttClient)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(e.ApplicationMessage.Topic)
            .WithPayload(e.ApplicationMessage.Payload)
            .Build();
        
        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
    }
}