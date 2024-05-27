using MQTTnet;
using MQTTnet.Client;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace MQTTClient1
{
    internal class Program
    {
        public enum state
        {
            Ein,
            Aus
        }

        static void Main(string[] args)
        {
            //Smarte Schalter

            int count = 1;
            Console.Title = "Schalter";
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
                return Task.CompletedTask;
            };
            client.ConnectAsync(options);

            Task t = new Task(() => switchState(client, count));
            t.Start();


            bool runVar = true;
            while (runVar)
            {
                Thread.Sleep(100);
            }
            client.DisconnectAsync();

            Console.WriteLine("[Client] End");
            Console.ReadLine();
        }



        public static void switchState(IMqttClient client, int count)
        {
            state Schalter = state.Aus;
            string key = " ";
            bool runVar = true;
            do
            {
                Thread.Sleep(1000);
                switch (Schalter)
                {
                    case state.Ein:
                        Console.WriteLine("Licht ausschalten? (Y)");
                        key = Console.ReadLine();
                        if (key == "y" || key == "Y")
                            Schalter = state.Aus;
                        break;
                    case state.Aus:
                        Console.WriteLine("Licht einschalten? (Y)");
                        key = Console.ReadLine();
                        if (key == "y" || key == "Y")
                            Schalter = state.Ein;
                        break;
                }
                client.PublishStringAsync($"iot-device-{count}/outbound/state", Schalter.ToString());
            } while (runVar);
        }
    }
}
