using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Serilog;
using SmartHouseController.MQTT.Broker.Server.Extension;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var mqttFactory = new MqttFactory();
var mqttServerOptions = new MqttServerOptionsBuilder()
    .WithDefaultEndpoint()
    .Build();
    
var mqttClient = mqttFactory.CreateMqttClient();
var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("0df1d148747b496d85f8ca59339e72a9.s2.eu.hivemq.cloud")
    .WithClientId("CONTROLLER")
    .WithCredentials("user1", "user1234")
    .WithTls()
    .WithCleanSession()
    .Build();

Log.Information("MQTT server is running at: {0}:{1}",
    mqttServerOptions.DefaultEndpointOptions.BoundInterNetworkAddress,
    mqttServerOptions.DefaultEndpointOptions.Port);
Log.Information("Press CTRL+C to stop");

using var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

mqttServer
    .WithValidationHandler()
    .WithDisconnectedHandler()
    .WithConnectedHandler()
    .WithInterceptingPublishHandler(mqttClient)
    .WithOnMessageLogHandler();

await mqttClient.ConnectAsync(mqttClientOptions);
await mqttServer.StartAsync();

while (true) { }