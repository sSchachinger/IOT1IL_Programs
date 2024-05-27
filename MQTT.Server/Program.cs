using System;
using System.Drawing;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace MQTT.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title=("Server");
            Console.WriteLine("[Server] Start");

            var factory = new MqttFactory();

            var builder = new MqttServerOptionsBuilder();
            builder.WithDefaultEndpoint();

            var options = builder.Build();

            var server = factory.CreateMqttServer(options);

            // Started/Stopped Ereignis

            server.StartedAsync += e =>
            {
                Console.WriteLine("[Server] Started");
                return Task.CompletedTask;
            };
            server.StoppedAsync += e =>
            {
                Console.WriteLine("[Server] Stopped");
                return Task.CompletedTask;
            };

            // Validating Ereignis

            server.ValidatingConnectionAsync += e =>
            {
                Console.WriteLine($"[Server] Validating connection ({e.ClientId}, {e.UserName}, {e.Password})");
                if (e.UserName == null || e.Password == null)
                {
                    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                }
                else if (e.ClientId.Equals("rule-engine"))
                {
                    if (!e.UserName.Equals("rule-engine-x") || !e.Password.Equals("rule-engine-y"))
                    {
                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                }
                else if (e.ClientId.StartsWith("iot-device-"))
                {
                    if (!e.UserName.Equals($"{e.ClientId}-x") || !e.Password.Equals($"{e.ClientId}-y"))
                    {
                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                }
                else
                {
                    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                }
                return Task.CompletedTask;
            };

            // Client Ereignisse

            server.ClientConnectedAsync += e =>
            {
                Console.WriteLine($"[Server] Client connected ({e.ClientId}, {e.UserName})");
                return Task.CompletedTask;
            };
            server.ClientDisconnectedAsync += e =>
            {
                Console.WriteLine($"[Server] Client disconnected ({e.ClientId}, {e.DisconnectType})");
                return Task.CompletedTask;
            };
            server.ClientSubscribedTopicAsync += e =>
            {
                Console.WriteLine($"[Server] Client subscribed topic ({e.ClientId}, {e.TopicFilter.Topic})");
                return Task.CompletedTask;
            };
            server.ClientUnsubscribedTopicAsync += e =>
            {
                Console.WriteLine($"[Server] Client unsubscribed topic ({e.ClientId}, {e.TopicFilter})");
                return Task.CompletedTask;
            };

            // Intercepting Ereignisse

            server.InterceptingPublishAsync += e =>
            {
                Console.WriteLine($"[Server] Intercepting publish ({e.ClientId}, {e.ApplicationMessage.Topic}, {e.ApplicationMessage.Payload})");
                if (e.ClientId.StartsWith("iot-device-"))
                {
                    if (!e.ApplicationMessage.Topic.StartsWith($"{e.ClientId}/outbound/"))
                    {
                        
                        // iot-device-<n>/outbound/sensor1
                        // iot-device-<n>/outbound/sensor2
                        e.ProcessPublish = false;
                        e.CloseConnection = true;
                    }
                }
                else if (e.ClientId.Equals("rule-engine"))
                {
                    var parts = e.ApplicationMessage.Topic.Split('/');
                    if (parts.Length < 3 || !parts[0].StartsWith("iot-device-") || !parts[1].Equals("inbound"))
                    {
                        // iot-device-<n>/inbound/actor1
                        // iot-device-<n>/inbound/actor2
                        e.ProcessPublish = false;
                        e.CloseConnection = true;
                    }
                }
                else
                {
                    e.ProcessPublish = false;
                    e.CloseConnection = true;
                }
                return Task.CompletedTask;
            };
            server.InterceptingSubscriptionAsync += e =>
            {
                Console.WriteLine($"[Server] Intercepting subscription ({e.ClientId}, {e.TopicFilter.Topic}, {e.TopicFilter.QualityOfServiceLevel})");
                if (e.ClientId.StartsWith("iot-device-"))
                {
                    // TODO: only own inbound topics
                }
                else if (e.ClientId.Equals("rule-engine"))
                {
                    // TODO: only iot-device outbound topics
                }
                else
                {
                    e.ProcessSubscription = false;
                    e.CloseConnection = true;
                }
                return Task.CompletedTask;
            };
            server.InterceptingUnsubscriptionAsync += e =>
            {
                Console.WriteLine($"[Server] Intercepting unsubscription ({e.ClientId}, {e.Topic})");
                return Task.CompletedTask;
            };

            server.StartAsync();

            Console.ReadLine();

            server.StopAsync();

            Console.WriteLine("[Server] End");
            Console.ReadLine();
        }
    }
}







//        //static Task HandleStartedAsync(EventArgs e)
//        //{
//        //    Console.WriteLine("[Server] Started");
//        //    return Task.CompletedTask;
//        //}
//        //static Task HandleStoppedAsync(EventArgs e)
//        //{
//        //    Console.WriteLine("[Server] Stopped");
//        //    return Task.CompletedTask;
//        //}
//        //static Task HandleValidatingConnectionAsync(ValidatingConnectionEventArgs e)
//        //{
//        //    Console.WriteLine($"[Server] Validating Connection: {e.ClientId} {e.UserName} {e.Password}");
//        //    // Check username and password
//        //    if (!e.UserName.Equals("my-user-name") || !e.Password.Equals("my-pass-word"))
//        //    {
//        //        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
//        //    }
//        //    return Task.CompletedTask;
//        //}
//        //static Task HandleClientConnectedAsync(ClientConnectedEventArgs e)
//        //{
//        //    Console.WriteLine($"[Server] Client Connected: {e.ClientId}, {e.UserName}");
//        //    return Task.CompletedTask;
//        //}
//        //static Task HandleClientDisconnectedAsync(ClientDisconnectedEventArgs e)
//        //{
//        //    Console.WriteLine($"[Server] Client Disconnected: {e.ClientId}");
//        //    return Task.CompletedTask;
//        //}
//        //static Task HandleClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs e)
//        //{
//        //    Console.WriteLine($"[Server] Client Subscribed Topic: {e.ClientId} {e.TopicFilter.Topic} {e.TopicFilter.QualityOfServiceLevel}");
//        //    return Task.CompletedTask;
//        //}
//        //static Task HandleClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs e)
//        //{
//        //    Console.WriteLine($"[Server] Client Unsubscribed Topic: {e.ClientId} {e.TopicFilter}");
//        //    return Task.CompletedTask;
//        //}
//        //static Task HandleInterceptingPublishAsync(InterceptingPublishEventArgs e)
//        //{
//        //    Console.WriteLine($"[Server] Intercepting Publish: {e.ClientId} {e.ApplicationMessage.Topic} {e.ApplicationMessage.Payload}"); if (e.ClientId.StartsWith("iot-device-"))
//        //    {
//        //        if (e.ApplicationMessage.Topic.StartsWith($"{e.ClientId}/outbound/"))
//        //        {
//        //            e.ProcessPublish = true;
//        //        }
//        //        else
//        //        {
//        //            e.ProcessPublish = false;
//        //            e.CloseConnection = true;
//        //        }
//        //    }
//        //    else if (e.ClientId.Equals("master"))
//        //    {
//        //        if (e.ApplicationMessage.Topic.Contains("/inbound/"))
//        //        {
//        //            e.ProcessPublish = true;
//        //        }
//        //        else
//        //        {
//        //            e.ProcessPublish = false;
//        //            e.CloseConnection = true;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        e.ProcessPublish = false;
//        //        e.CloseConnection = true;
//        //    }
//        //    return Task.CompletedTask;
//        //}
//        //static Task HandleInterceptingSubscriptionAsync(InterceptingSubscriptionEventArgs e)
//        //{
//        //    Console.WriteLine($"[Server] Intercepting Subscription: {e.ClientId} {e.TopicFilter.Topic}"); if (e.ClientId.StartsWith("iot-device-"))
//        //    {
//        //        if (e.TopicFilter.Topic.StartsWith($"{e.ClientId}/inbound/"))
//        //        {
//        //            e.ProcessSubscription = true;
//        //        }
//        //        else
//        //        {
//        //            e.ProcessSubscription = false;
//        //            e.CloseConnection = true;
//        //        }
//        //    }
//        //    else if (e.ClientId.Equals("master"))
//        //    {
//        //        if (e.TopicFilter.Topic.Contains("/outbound/"))
//        //        {
//        //            e.ProcessSubscription = true;
//        //        }
//        //        else
//        //        {
//        //            e.ProcessSubscription = false;
//        //            e.CloseConnection = true;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        e.ProcessSubscription = false;
//        //        e.CloseConnection = true;
//        //    }
//        //    return Task.CompletedTask;
//        //}
//        //static Task HandleInterceptingUnsubscriptionAsync(InterceptingUnsubscriptionEventArgs e)
//        //{
//        //    Console.WriteLine($"[Server] Intercepting Unsubscription: {e.ClientId} {e.Topic}");
//        //    e.ProcessUnsubscription = true; return Task.CompletedTask;
//        //}
//        //static void Main(string[] args)
//        //{
//        //    Console.WriteLine("[Server] Start"); var builder = new MqttServerOptionsBuilder();
//        //    builder.WithDefaultEndpoint(); var options = builder.Build(); var factory = new MqttFactory(); var server = factory.CreateMqttServer(options);
//        //    server.StartedAsync += HandleStartedAsync;
//        //    server.StoppedAsync += HandleStoppedAsync; server.ValidatingConnectionAsync += HandleValidatingConnectionAsync; server.ClientConnectedAsync += HandleClientConnectedAsync;
//        //    server.ClientDisconnectedAsync += HandleClientDisconnectedAsync;
//        //    server.ClientSubscribedTopicAsync += HandleClientSubscribedTopicAsync;
//        //    server.ClientUnsubscribedTopicAsync += HandleClientUnsubscribedTopicAsync; server.InterceptingPublishAsync += HandleInterceptingPublishAsync;
//        //    server.InterceptingSubscriptionAsync += HandleInterceptingSubscriptionAsync;
//        //    server.InterceptingUnsubscriptionAsync += HandleInterceptingUnsubscriptionAsync; server.StartAsync();
//        //    Console.ReadLine(); server.StopAsync(); Console.WriteLine("[Server] End");
//        //    Console.ReadLine();
//        //}


//        static void Main(string[] args)
//        {
//            Console.WriteLine("[Server] Start");
//            var factory = new MqttFactory();
//            var builder = new MqttServerOptionsBuilder();
//            builder.WithDefaultEndpoint();
//            var options = builder.Build();
//            var server = factory.CreateMqttServer(options);
//            // Started/Stopped Ereignis
//            server.StartedAsync += e =>
//            {
//                Console.WriteLine("[Server] Started");
//                return Task.CompletedTask;
//            };
//            server.StoppedAsync += e =>
//            {
//                Console.WriteLine("[Server] Stopped");
//                return Task.CompletedTask;
//            };
//            // Validating Ereignis
//            server.ValidatingConnectionAsync += e =>
//            {
//                Console.WriteLine($"[Server] Validating connection ({e.ClientId}, {e.UserName}, {e.Password})");
//                if (e.UserName == null || e.Password == null)
//                {
//                    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
//                }
//                else if (e.ClientId.Equals("rule-engine"))
//                {
//                    if (!e.UserName.Equals("rule-engine-x") || !e.Password.Equals("rule-engine-y"))
//                    {
//                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
//                    }
//                }
//                else if (e.ClientId.StartsWith("iot-device-"))
//                {
//                    if (!e.UserName.Equals($"{e.ClientId}-x") || !e.Password.Equals($"{e.ClientId}-y"))
//                    {
//                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
//                    }
//                }
//                else
//                {
//                    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
//                }
//                return Task.CompletedTask;
//            };
//            // Client Ereignisse
//            server.ClientConnectedAsync += e =>
//            {
//                Console.WriteLine($"[Server] Client connected ({e.ClientId}, {e.UserName})");
//                return Task.CompletedTask;
//            };
//            server.ClientDisconnectedAsync += e =>
//            {
//                Console.WriteLine($"[Server] Client disconnected ({e.ClientId}, {e.DisconnectType})");
//                return Task.CompletedTask;
//            };
//            server.ClientSubscribedTopicAsync += e =>
//            {
//                Console.WriteLine($"[Server] Client subscribed topic ({e.ClientId}, {e.TopicFilter.Topic})");
//                return Task.CompletedTask;
//            };
//            server.ClientUnsubscribedTopicAsync += e =>
//            {
//                Console.WriteLine($"[Server] Client unsubscribed topic ({e.ClientId}, {e.TopicFilter})");
//                return Task.CompletedTask;
//            };
//            // Intercepting Ereignisse
//            server.InterceptingPublishAsync += e =>
//            {
//                Console.WriteLine($"[Server] Intercepting publish ({e.ClientId}, {e.ApplicationMessage.Topic}, {e.ApplicationMessage.Payload})");
//                if (e.ClientId.StartsWith("iot-device-"))
//                {
//                    if (!e.ApplicationMessage.Topic.StartsWith($"{e.ClientId}/outbound/"))
//                    {
//                        // iot-device-<n>/outbound/sensor1
//                        // iot-device-<n>/outbound/sensor2
//                        e.ProcessPublish = false;
//                        e.CloseConnection = true;
//                    }
//                }
//                else if (e.ClientId.Equals("rule-engine"))
//                {
//                    var parts = e.ApplicationMessage.Topic.Split('/');
//                    if (parts.Length < 3 || !parts[0].StartsWith("iot-device-") || !parts[1].Equals("inbound"))
//                    {
//                        // iot-device-<n>/inbound/actor1
//                        // iot-device-<n>/inbound/actor2
//                        e.ProcessPublish = false;
//                        e.CloseConnection = true;
//                    }
//                }
//                else
//                {
//                    e.ProcessPublish = false;
//                    e.CloseConnection = true;
//                }
//                return Task.CompletedTask;
//            };
//            server.InterceptingSubscriptionAsync += e =>
//            {
//                Console.WriteLine($"[Server] Intercepting subscription ({e.ClientId}, {e.TopicFilter.Topic}, {e.TopicFilter.QualityOfServiceLevel})");
//                if (e.ClientId.StartsWith("iot-device-"))
//                {
//                    // TODO: only own inbound topics
//                }
//                else if (e.ClientId.Equals("rule-engine"))
//                {
//                    // TODO: only iot-device outbound topics
//                }
//                else
//                {
//                    e.ProcessSubscription = false;
//                    e.CloseConnection = true;
//                }
//                return Task.CompletedTask;
//            };
//            server.InterceptingUnsubscriptionAsync += e =>
//            {
//                Console.WriteLine($"[Server] Intercepting unsubscription ({e.ClientId}, {e.Topic})");
//                return Task.CompletedTask;
//            };
//            server.StartAsync();
//            Console.ReadLine();
//            server.StopAsync();
//            Console.WriteLine("[Server] End");
//            Console.ReadLine();

//        }
//    }
//}