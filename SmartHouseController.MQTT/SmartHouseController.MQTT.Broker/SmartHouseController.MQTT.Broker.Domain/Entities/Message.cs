namespace SmartHouseController.MQTT.Broker.Domain.Entities;

public class Message
{
    public string ClientId { get; set; }
    public string Payload { get; set; }
    public string Topic { get; set; }
    public int QoS { get; set; }
}