using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace Mqtt.Server
{
    internal class Program
    {
        static Task HandleStartedAsync(EventArgs e)
        {
            Console.WriteLine("[Server] Started");
            return Task.CompletedTask;
        }
        static Task HandleStoppedAsync(EventArgs e)
        {
            Console.WriteLine("[Server] Stopped");
            return Task.CompletedTask;
        }

        static Task HandleValidatingConnectionAsync(ValidatingConnectionEventArgs e)
        {
            Console.WriteLine($"[Server] Validating Connection: {e.ClientId} {e.UserName} {e.Password}");
            // Check username and password
            if (!e.UserName.Equals("my-user-name") || !e.Password.Equals("my-pass-word"))
            {
                e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            }
            return Task.CompletedTask;
        }

        static Task HandleClientConnectedAsync(ClientConnectedEventArgs e)
        {
            Console.WriteLine($"[Server] Client Connected: {e.ClientId}, {e.UserName}");
            return Task.CompletedTask;
        }
        static Task HandleClientDisconnectedAsync(ClientDisconnectedEventArgs e)
        {
            Console.WriteLine($"[Server] Client Disconnected: {e.ClientId}");
            return Task.CompletedTask;
        }
        static Task HandleClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs e)
        {
            Console.WriteLine($"[Server] Client Subscribed Topic: {e.ClientId} {e.TopicFilter.Topic} {e.TopicFilter.QualityOfServiceLevel}");
            return Task.CompletedTask;
        }
        static Task HandleClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs e)
        {
            Console.WriteLine($"[Server] Client Unsubscribed Topic: {e.ClientId} {e.TopicFilter}");
            return Task.CompletedTask;
        }
        static Task HandleInterceptionPublishAsnyc(InterceptingPublishEventArgs e)
        {
            Console.WriteLine($"[Server] Interception Publish: {e.ClientId} {e.ApplicationMessage.Topic} {e.ApplicationMessage.Payload}");
            if (e.ClientId.Equals("iot-device-"))
            {
                if (e.ApplicationMessage.Topic.StartsWith($"/{e.ClientId}/outbound"))
                    e.ProcessPublish = true;
                else
                    e.ProcessPublish = false;
            }
            else if (e.ClientId.Equals("master"))
            {
                if (e.ApplicationMessage.Topic.Contains($"/inbound"))
                    e.ProcessPublish = true;
                else
                {
                    e.ProcessPublish = false;
                    e.CloseConnection = true;
                }
                e.ProcessPublish = true;
            }
            else
            {
                e.ProcessPublish = false;
                e.CloseConnection = true;
            }

            return Task.CompletedTask;
        }
        static Task HandleInterceptionSubscriptionAsnyc(InterceptingSubscriptionEventArgs e)
        {
            Console.WriteLine($"[Server] Interception Subscription: {e.ClientId} {e.TopicFilter.Topic}");

            if (e.ClientId.Equals("iot-device-"))
            {
                if (e.TopicFilter.Topic.StartsWith($"/{e.ClientId}/inbound"))
                    e.ProcessSubscription = true;
                else
                    e.ProcessSubscription = false;
            }
            else if (e.ClientId.Equals("master"))
            {
                if (e.TopicFilter.Topic.Contains($"/outbound"))
                    e.ProcessSubscription = true;
                else
                {
                    e.ProcessSubscription = false;
                    e.CloseConnection = true;
                }
                e.ProcessSubscription = true;
            }
            else
            {
                e.ProcessSubscription = false;
                e.CloseConnection = true;
            }
            return Task.CompletedTask;
        }
        static Task HandleInterceptionUnsubscriptionAsnyc(InterceptingUnsubscriptionEventArgs e)
        {
            Console.WriteLine($"[Server] Interception UnSubscription: {e.ClientId} {e.Topic}");
            e.ProcessUnsubscription = true;
            return Task.CompletedTask;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("[Server] Start");

            var builder = new MqttServerOptionsBuilder();
            builder.WithDefaultEndpoint();

            var options = builder.Build();

            var factory = new MqttFactory();

            var server = factory.CreateMqttServer(options);

            server.StartedAsync += HandleStartedAsync;
            server.StoppedAsync += HandleStoppedAsync;

            server.ValidatingConnectionAsync += HandleValidatingConnectionAsync;

            server.ClientConnectedAsync += HandleClientConnectedAsync;
            server.ClientDisconnectedAsync += HandleClientDisconnectedAsync;
            server.ClientSubscribedTopicAsync += HandleClientSubscribedTopicAsync;
            server.ClientUnsubscribedTopicAsync += HandleClientUnsubscribedTopicAsync;

            server.InterceptingPublishAsync += HandleInterceptionPublishAsnyc;
            server.InterceptingSubscriptionAsync += HandleInterceptionSubscriptionAsnyc;
            server.InterceptingUnsubscriptionAsync += HandleInterceptionUnsubscriptionAsnyc;

            server.StartAsync();

            Console.ReadLine();

            server.StopAsync();

            Console.WriteLine("[Server] End");
            Console.ReadLine();
        }
    }
}