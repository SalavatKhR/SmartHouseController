using MQTTnet.AspNetCore;
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
    .WithDefaultCommunicationTimeout(TimeSpan.FromMilliseconds(1000))
    .Build();

builder.Services
    .AddHostedMqttServer(optionBuilder)
    .AddMqttConnectionHandler()
    .AddConnections()
    .AddMqttTcpServerAdapter();

builder.Services.AddMqttConnectionHandler();
builder.Services.AddMqttWebSocketServerAdapter();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();