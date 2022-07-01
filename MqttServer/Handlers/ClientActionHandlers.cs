﻿using System.Net.Mime;
using System.Text;
using System.Text.Json;
using MQTTnet.Server;
using MqttServer.Exceptions;
using MqttServer.Models;
using Serilog;

namespace MqttServer.Handlers;

public static class ClientActionHandlers
{
    public static Task OnConnectedValidateAsync(ValidatingConnectionEventArgs e)
    {

        // TODO: valdiation
        
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

    public static Task OnMessageAsync(InterceptingPublishEventArgs e)
    {
        var message = new Message
            {
                ClientId = e.ClientId,
                Payload = e.ApplicationMessage?.Payload == null 
                    ? throw new EmptyFieldMessageRequestException("Payload")
                    : Encoding.UTF8.GetString(e.ApplicationMessage!.Payload),
                Topic = e.ApplicationMessage.Topic ?? throw new EmptyFieldMessageRequestException("Topic"),
                QoS = (int) e.ApplicationMessage.QualityOfServiceLevel
            };
        
        Log.Logger.Information(JsonSerializer.Serialize(message));

        return Task.CompletedTask;
    }
}