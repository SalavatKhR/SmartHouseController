using System.Collections.Concurrent;
using MQTTnet.Client;

namespace WebAPI.Models;

public class Connections : IConnections
{
    private static ConcurrentDictionary<string, IMqttClient?> _connections = new (); 

    public void AddConnection(string userId, IMqttClient? client)
    {
        if (!_connections.ContainsKey(userId))
            _connections.TryAdd(userId, client);
        else
            _connections.TryAdd(userId, client);
    }

    public void RemoveConnection(string userId)
    {
        _connections[userId]?.Dispose();
        _connections.TryRemove(userId, out var value);
    }

    public IMqttClient? this[string id] => _connections[id];
}