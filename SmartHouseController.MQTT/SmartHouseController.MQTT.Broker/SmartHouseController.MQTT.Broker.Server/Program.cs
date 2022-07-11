using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Serilog;
using SmartHouseController.MQTT.Broker.Domain.Data;
using SmartHouseController.MQTT.Broker.Domain.Entities;
using SmartHouseController.MQTT.Broker.Server.Extension;



using var db = new ApplicationDbContext();

var topics = new List<string>()
{
    "zigbee2mqtt/0x00158d0006d51e9a",
    "zigbee2mqtt/0x04cf8cdf3c79d013",
    "zigbee2mqtt/0x00158d000704feca",
};

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

Log.Information(Convert.ToDateTime("11.05.2022").ToString());

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

mqttServer.InterceptingPublishAsync += e =>
{
    var topic = e.ApplicationMessage.Topic;
    if (!topics.Contains(topic)) return Task.CompletedTask;
    var payload = Encoding.UTF8.GetString(e.ApplicationMessage!.Payload);
    var date = DateOnly.FromDateTime(DateTime.Now);
    var record = db.Statistics.FirstOrDefault(r => r.date == date && r.topic == topic);
    if (record != null)
        record.payload = payload;
    else
        db.Add(new Staistics
        {
            date = date,
            topic = topic,
            payload = payload
        });

    var oldRecords = db.Statistics.Where(r =>
        r.date.AddMonths(1) <= date
    ).ToList();
    
    db.RemoveRange(oldRecords);
    
    db.SaveChanges();
    
    return Task.CompletedTask;
};

while (true) { }