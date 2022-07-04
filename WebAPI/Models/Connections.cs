using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MQTTnet.Client;

namespace WebAPI.Models;

public class Connections : IConnections
{
    private static Dictionary<string, MqttClient> _connections;

    public static Dictionary<string, MqttClient> Retrieve
    {
        get
        {
            if (_connections == null)
                _connections = new Dictionary<string, MqttClient>();

            return _connections;
        }
    }
    
    public void AddConnection(string userId, MqttClient client)
    {
        _connections.Add(userId, client);
    }

    public void RemoveConnection(string userId)
    {
        _connections[userId].Dispose();
        _connections.Remove(userId);
    }
}