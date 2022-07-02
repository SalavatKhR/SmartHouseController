namespace SmartHouseController.MQTT.Broker.Server.Exceptions;

public class ServerInstanceException : Exception
{
    public ServerInstanceException() : 
        base($"Server instance already created") { }
}