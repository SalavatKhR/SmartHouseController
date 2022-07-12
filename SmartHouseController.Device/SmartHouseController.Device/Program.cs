using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Text;
using System.Text.Json;
using SmartHouseController.Device.Messages;

namespace SmartHouseController.Device
{
    class Program
    {
        private static IMqttClient _client;
        
        private const string CommandOn = "{\"state\": \"ON\", \"update\": { \"state\": \"available\"}}";
        private const string CommandOff = "{\"state\": \"OFF\", \"update\": { \"state\": \"available\"}}";

        private static readonly Dictionary<string, string> Topics = new Dictionary<string, string>()
        {
            {"plug", "0x04cf8cdf3c79d013"},
            {"thermometer", "0x00158d000704feca"}
        };
        
        private static void Main()
        {
            Console.WriteLine("Device is running...");

            var factory = new MqttFactory();
           _client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("Controller_ID")
                .WithTcpServer("localhost", 1883)
                .WithCleanSession()
                .Build();


            _client.UseConnectedHandler(e =>
            {
                Console.WriteLine("Connected successfully with MQTT Brokers.");
                _client.SubscribeAsync(new TopicFilterBuilder().WithTopic($"zigbee2mqtt/{Topics["plug"]}").Build()).Wait();
                _client.SubscribeAsync(new TopicFilterBuilder().WithTopic($"zigbee2mqtt/{Topics["thermometer"]}").Build()).Wait();
            });
            _client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from MQTT Brokers.");
            });

            _client.UseApplicationMessageReceivedHandler(e =>
            {
                try
                {
                    var topic = e.ApplicationMessage.Topic;
                    if (string.IsNullOrWhiteSpace(topic) == false)
                    {
                        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                        Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
                        if (topic == $"zigbee2mqtt/{Topics["thermometer"]}")
                        {
                            var message = JsonSerializer.Deserialize<Thermometer>(payload);
                            if (message.temperature >= 30)
                            {
                                SendMessage(CommandOn, Topics["plug"]);
                                Thread.Sleep(10000);
                                SendMessage(CommandOff, Topics["plug"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            });

            _client.ConnectAsync(options).Wait();
            Console.ReadLine();

            while (true)
            {
                SendMessage(CommandOn, Topics["plug"]);
                Thread.Sleep(2000);
                SendMessage(CommandOff, Topics["plug"]);
                Thread.Sleep(2000);
            }
        }

        static void SendMessage(string jsonCommand, string topic)
        {
            var testMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"zigbee2mqtt/{topic}/set")
                .WithPayload(jsonCommand)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            if (_client.IsConnected)
            {
                Console.WriteLine($"publishing at {DateTime.UtcNow}");
                _client.PublishAsync(testMessage);
            }
        }
    }
}
