using MQTTnet.Server;
using SmartHouseController.MQTT.Broker.Server.Handlers;

namespace SmartHouseController.MQTT.Broker.Server.Extension;

public static class ServerExtensions
{
    public static MqttServer Setup(this MqttServer mqttServer)
    {
        mqttServer.ValidatingConnectionAsync +=
            ClientActionHandlers.OnConnectedValidateAsync;

        mqttServer.ClientConnectedAsync += 
            ClientActionHandlers.OnConnectedAsync;

        mqttServer.ClientDisconnectedAsync +=
            ClientActionHandlers.OnDisconnectedAsync;

        mqttServer.InterceptingPublishAsync += 
            ClientActionHandlers.OnMessageAsync;

        return mqttServer;
    }
}