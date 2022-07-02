using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;

namespace SmartHouseController.MQTT.Broker.Api.Controllers;

[Route("v1/api/")]
[ApiController]
public class MqttController : Controller
{
    [HttpGet]
    public async Task TestClient()
    {
        
        #region client

        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost")
            .WithCleanSession()
            .WithClientId(Guid.NewGuid().ToString())
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("samples/temperature/living_room")
            .WithPayload("19.5")
            .Build();

        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);


        #endregion

    }
}