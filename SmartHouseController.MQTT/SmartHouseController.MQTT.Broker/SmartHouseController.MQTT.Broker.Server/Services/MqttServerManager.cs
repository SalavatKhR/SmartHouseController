using MQTTnet;
using MQTTnet.Server;
using SmartHouseController.MQTT.Broker.Server.Exceptions;
using SmartHouseController.MQTT.Broker.Server.Extension;

namespace SmartHouseController.MQTT.Broker.Server.Services;

public class MqttServerManager
{
    private static MqttFactory _factory;
    private static MqttServer _server;
    private static MqttServerManager _manager;
    private MqttServerManager()
    {
        _factory = new MqttFactory();
    }

    public MqttServer CreateServer(MqttServerOptions options)
    {
        if (_manager == null)
        {
            _manager = this;
            _server = _factory.CreateMqttServer(options);
            _server.Setup();   
        }

        return _server;
    }

    public async Task StartAsync()
    { 
        await _server.StartAsync();
    }
}