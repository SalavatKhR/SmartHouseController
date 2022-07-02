namespace SmartHouseController.MQTT.Broker.Server.Models;

public class MessageDto
{
    public string ClientId { get; set; }
    public string Payload { get; set; }
    public string Topic { get; set; }
    public int QoS { get; set; }
}