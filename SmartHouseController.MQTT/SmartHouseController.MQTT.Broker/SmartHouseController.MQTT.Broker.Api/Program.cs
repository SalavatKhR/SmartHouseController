using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.Client;
using MQTTnet.Server;
using Serilog;
using Serilog.Events;
using SmartHouseController.MQTT.Broker.Server.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseSerilog((_, lc) => lc
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var optionBuilder = new MqttServerOptionsBuilder()
    .WithDefaultEndpoint()
    .Build();

builder.Services
    .AddHostedMqttServer(optionBuilder)
    .AddMqttConnectionHandler()
    .AddConnections()
    .AddMqttTcpServerAdapter();;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMqttServer(server =>
{
    server.Setup();
});

app.MapMqtt("/data");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

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
