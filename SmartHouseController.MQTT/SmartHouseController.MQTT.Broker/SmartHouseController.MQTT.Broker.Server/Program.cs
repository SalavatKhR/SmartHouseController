using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Serilog;
using SmartHouseController.MQTT.Broker.Server.Handlers;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger(); 

var mqttFactory = new MqttFactory();
var mqttServerOptions = new MqttServerOptionsBuilder()
    .WithDefaultEndpoint()
    .Build();

Log.Information("MQTT server is running at: {0}:{1}",
    mqttServerOptions.DefaultEndpointOptions.BoundInterNetworkAddress,
    mqttServerOptions.DefaultEndpointOptions.Port);
Log.Information("Press key to close the server");

using var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

mqttServer.ValidatingConnectionAsync +=
    ClientActionHandlers.OnConnectedValidateAsync;

mqttServer.ClientConnectedAsync += 
    ClientActionHandlers.OnConnectedAsync;

mqttServer.ClientDisconnectedAsync +=
    ClientActionHandlers.OnDisconnectedAsync;

mqttServer.InterceptingPublishAsync += 
    ClientActionHandlers.OnMessageAsync;

await mqttServer.StartAsync();

#region client

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

Console.ReadKey();

await mqttServer.StopAsync();