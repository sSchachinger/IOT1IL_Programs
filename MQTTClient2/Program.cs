using MQTTnet;
using MQTTnet.Client;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace MQTTClient2
{
    internal class Program
    {
        public static state lampe;
        //Smarte Glühbirne
        public enum state
        {
            Ein,
            Aus
        }

        static void Main(string[] args)
        {
            Console.Title = "Glühbirne";
            int count = 2;
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
                //Subscribe auf die Cloud
                client.SubscribeAsync($"iot-device-3/state");
                client.PublishStringAsync($"iot-device-{count}/outbound/state", "online");
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
                if (e.ApplicationMessage.Topic.Contains("iot-device-3") && e.ApplicationMessage.ConvertPayloadToString() == state.Aus.ToString())
                    lampe = state.Aus;
                if (e.ApplicationMessage.Topic.Contains("iot-device-3") && e.ApplicationMessage.ConvertPayloadToString() == state.Ein.ToString())
                    lampe = state.Ein;
                return Task.CompletedTask;
            };
            client.ConnectAsync(options);


            Task t = new Task(() => showState(client, count));
            t.Start();


            Console.ReadLine();

            client.DisconnectAsync();

            Console.WriteLine("[Client] End");
            Console.ReadLine();
        }


        public static void showState(IMqttClient client, int count)
        {
            bool runVar = true;
            do
            {
                Thread.Sleep(1000);
                switch (lampe)
                {
                    case state.Ein:
                        Console.WriteLine("Licht eingeschaltet!");

                        break;
                    case state.Aus:
                        Console.WriteLine("Licht ausgeschaltet!");
                        break;
                }
                
                client.PublishStringAsync($"iot-device-{count}/outbound/state", lampe.ToString());
            } while (runVar);
        }
    }
}

