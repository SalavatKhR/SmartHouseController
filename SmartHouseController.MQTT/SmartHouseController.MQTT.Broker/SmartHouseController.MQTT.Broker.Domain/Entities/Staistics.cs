using System.ComponentModel.DataAnnotations;

namespace SmartHouseController.MQTT.Broker.Domain.Entities;

public class Staistics
{
    public DateOnly date { get; set; }
    public string topic { get; set; }
    public string payload { get; set; }
}