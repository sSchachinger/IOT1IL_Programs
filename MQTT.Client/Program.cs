using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;

namespace MQTT.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Client] Start");
            var factory = new MqttFactory();
            var builder = new MqttClientOptionsBuilder();

            builder.WithConnectionUri("mqtt://localhost:1883");
            builder.WithClientId("iot-device-1");
            builder.WithCredentials("my-user-name", "my-pass-word");
            builder.WithWillTopic("iot-device-1/outbound/state");
            builder.WithWillPayload("offline");
            builder.WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce);
            builder.WithWillRetain(true);

            var options = builder.Build();
            var client = factory.CreateMqttClient();

            client.ConnectedAsync += async e =>
            {
                await client.SubscribeAsync("iot-device-1/inbound/#", MqttQualityOfServiceLevel.AtLeastOnce);
                await client.PublishStringAsync("iot-device-1/outbound/state", "online", MqttQualityOfServiceLevel.AtLeastOnce, true);

                Console.WriteLine("[Client] Connected");
            };
            client.DisconnectedAsync += async e =>
            {
                Console.WriteLine("[Client] Disconnected");
            };
            client.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.ConvertPayloadToString();
                Console.WriteLine($"[Client] Application message recieved:{topic}, {payload}");
                await client.PublishStringAsync("iot-device-1/outbound/response", "ok");
            };
            client.ConnectAsync(options);
            Console.ReadLine();

            client.PublishStringAsync("iot-device-1/outbound/state", "offline", MqttQualityOfServiceLevel.AtLeastOnce, true);
            client.DisconnectAsync();

            Console.WriteLine("[Client] End");
        }
    }
}
