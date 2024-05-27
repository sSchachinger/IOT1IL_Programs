using MQTTnet;
using MQTTnet.Client;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace MQTTClient3
{
    internal class Program
    {
        public enum state
        {
            Ein,
            Aus,
            Undefiniert
        }
        //Cloud
        static void Main(string[] args)
        {
            var Lampe = state.Undefiniert;
            var Schalter = state.Undefiniert;
            Console.Title = "Cloud";
            int count = 3;
            Console.WriteLine($"[Client{count}] Start");

            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            var builder = new MqttClientOptionsBuilder();
            builder.WithClientId($"iot-device-{count}");
            builder.WithCredentials($"iot-device-{count}-x", $"iot-device-{count}-y");
            builder.WithConnectionUri("mqtt://localhost:1883");

            var options = builder.Build();
            client.ConnectedAsync += e =>
            {
                Console.WriteLine($"[Client{count}] Connected");
                client.SubscribeAsync($"iot-device-2/outbound/state");
                client.SubscribeAsync($"iot-device-1/outbound/state");
                client.PublishStringAsync($"iot-device-{count}/outbound/state", state.Undefiniert.ToString());
                return Task.CompletedTask;
            };
            client.DisconnectedAsync += e =>
            {
                Console.WriteLine($"[Client{count}] Disconnected");
                return Task.CompletedTask;
            };
            client.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine($"[Client{count}] Application Message Received ({e.ApplicationMessage.Topic},{e.ApplicationMessage.ConvertPayloadToString()})");
                if (e.ApplicationMessage.Topic.Contains("iot-device-1") && e.ApplicationMessage.ConvertPayloadToString() == state.Aus.ToString())
                    Schalter = state.Aus;
                else if(e.ApplicationMessage.Topic.Contains("iot-device-1") && e.ApplicationMessage.ConvertPayloadToString() == state.Ein.ToString())
                    Schalter = state.Ein;

                if (e.ApplicationMessage.Topic.Contains("iot-device-2") && e.ApplicationMessage.ConvertPayloadToString() == state.Aus.ToString())
                    Lampe = state.Aus;
                else if(e.ApplicationMessage.Topic.Contains("iot-device-2") && e.ApplicationMessage.ConvertPayloadToString() == state.Ein.ToString())
                    Lampe = state.Ein;

                if (Schalter == state.Ein)
                    client.PublishStringAsync($"iot-device-{count}/outbound/state", state.Ein.ToString());
                else if (Schalter == state.Aus)
                    client.PublishStringAsync($"iot-device-{count}/outbound/state", state.Aus.ToString());
               
                return Task.CompletedTask;
            };
            client.ConnectAsync(options);

            Console.ReadLine();

            client.DisconnectAsync();

            Console.WriteLine("[Client] End");
            Console.ReadLine();
        }



    }
}
