namespace SmartHouseController.MQTT.Broker.Server.Exceptions;

public class EmptyFieldMessageRequestException : Exception
{
    public EmptyFieldMessageRequestException(string field) : 
        base($"Empty field '{field}' value ") { }
}