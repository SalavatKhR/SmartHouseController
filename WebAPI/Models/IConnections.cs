using MQTTnet.Client;

namespace WebAPI.Models;

public interface IConnections
{
    void AddConnection(string userId, IMqttClient? client);
    void RemoveConnection(string userId);
}