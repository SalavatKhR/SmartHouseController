namespace MqttServer.Exceptions;

public class EmptyFieldMessageRequestException : Exception
{
    public EmptyFieldMessageRequestException(string field) : 
        base($"Empty field '{field}' value ") { }
}