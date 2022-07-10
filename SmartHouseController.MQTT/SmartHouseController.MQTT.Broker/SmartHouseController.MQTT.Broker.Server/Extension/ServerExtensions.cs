using MQTTnet.Client;
using MQTTnet.Server;
using SmartHouseController.MQTT.Broker.Server.Handlers;

namespace SmartHouseController.MQTT.Broker.Server.Extension;

public static class ServerExtensions
{
    public static MqttServer WithOnMessageLogHandler(this MqttServer mqttServer)
    {
        mqttServer.InterceptingPublishAsync += 
            ClientActionHandlers.OnMessageLogAsync;

        return mqttServer;
    }
    public static MqttServer WithInterceptingPublishHandler(this MqttServer mqttServer, IMqttClient client)
    {
        mqttServer.InterceptingPublishAsync += e => ClientActionHandlers.OnInterceptPublishAsync(e, client);

        return mqttServer;
    }
    
    public static MqttServer WithDisconnectedHandler(this MqttServer mqttServer)
    {
        mqttServer.ClientDisconnectedAsync +=
            ClientActionHandlers.OnDisconnectedAsync;

        return mqttServer;
    }

    public static MqttServer WithValidationHandler(this MqttServer mqttServer)
    {
        mqttServer.ValidatingConnectionAsync +=
            ClientActionHandlers.OnConnectedValidateAsync;
        return mqttServer;
    }

    public static MqttServer WithConnectedHandler(this MqttServer mqttServer)
    {
        mqttServer.ClientConnectedAsync += 
            ClientActionHandlers.OnConnectedAsync;

        return mqttServer;
    }
}