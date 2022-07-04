using MQTTnet.Client;

namespace WebAPI.Models;

public interface IConnections
{
    void AddConnection(string userId, MqttClient client);
    void RemoveConnection(string userId);
}