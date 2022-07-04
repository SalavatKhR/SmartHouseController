using MQTTnet;
using MQTTnet.Client;

Console.WriteLine("Device is running...");

var mqttFactory = new MqttFactory();
using var mqttClient = mqttFactory.CreateMqttClient();
var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("7af26d6b37114f6b939d385420614ad0.s2.eu.hivemq.cloud")
    .WithClientId(Guid.NewGuid().ToString())
    .WithCredentials("user1", "user1234")
    .WithTls()
    .WithCleanSession()
    .Build();

var connectionResult =  await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
Console.WriteLine(connectionResult.ResultCode);
    
var applicationMessage = new MqttApplicationMessageBuilder()
    .WithTopic("samples/temperature/living_room")
    .WithPayload("19.5")
    .Build();
await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);


Console.ReadKey();