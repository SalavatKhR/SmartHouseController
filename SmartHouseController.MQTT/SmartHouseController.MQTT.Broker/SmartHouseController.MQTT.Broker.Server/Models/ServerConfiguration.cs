namespace SmartHouseController.MQTT.Broker.Server.Models;

public class Configuration : IServerConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; }
}