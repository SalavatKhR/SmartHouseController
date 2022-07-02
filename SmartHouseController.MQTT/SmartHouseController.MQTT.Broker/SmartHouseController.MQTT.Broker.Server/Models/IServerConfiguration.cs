namespace SmartHouseController.MQTT.Broker.Server.Models;

public interface IServerConfiguration
{
    string Host { get; set; }
    int Port { get; set; }
}