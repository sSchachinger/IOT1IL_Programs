using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP.Client
{
    class Program
    {
        static TcpClient client;
        static NetworkStream stream;
        static bool gRunVar = false;

        static void Main(string[] args)
        {
            Console.Title = "TCP-Client";
            stream = initConnection();

            Task tSend = new Task(sendTask);
            Task tRecieve = new Task(recieveTask);
            tSend.Start();
            tRecieve.Start();

            while (!gRunVar)
            {
                Thread.Sleep(10000);
            }
        }
        static void recieveTask()
        {
            var reader = new StreamReader(stream);
            string request = "";
            bool runVar = false;

            do
            {
                try
                {
                    request = reader.ReadLine();
                }
                catch (Exception)
                {
                }
                finally { reader.Close(); }

                if (request == "Exit")
                {
                    runVar = true;
                    client.Close();
                    Console.WriteLine("[Server] closed");
                }
                else if (request == null)
                { }
                else if (request.Contains("Equation"))
                    Console.WriteLine("[Server]:" + request);
                else if (request.Contains("Stringcheck"))
                    Console.WriteLine("[Server]:" + request);



                Thread.Sleep(100);
            } while (!runVar);
        }

        static void sendTask()
        {
            bool runVar = false;
            do
            {
                Console.WriteLine("[Client] What to do? Equation, Exit or Stringcheck");
                string input = Console.ReadLine();

                switch (input)
                {
                    case ("Equation"):
                        sendEquation();
                        break;
                    case ("Stringcheck"):
                        sendStringcheck();
                        break;
                    case ("Exit"):
                        runVar = true;
                        gRunVar = true;
                        sendExit();
                        client.Close();
                        Console.WriteLine("[Client] closed");
                        break;
                    default:
                        break;
                }
                Thread.Sleep(100);
            } while (!runVar);
        }

        static NetworkStream initConnection()
        {
            string hostname = "127.0.0.1";
            var port = 3001;
            client = new TcpClient(hostname, port);
            return client.GetStream();
        }

        static void sendEquation()
        {
            var reader = new StreamReader(stream, Encoding.ASCII);
            var writer = new StreamWriter(stream, Encoding.ASCII);

            Console.WriteLine("[Client] Insert Equation:");
            string input = Console.ReadLine();

            writer.WriteLine($"Equation:{input}");
            writer.Flush();

        }
        static void sendStringcheck()
        {

            var writer = new StreamWriter(stream, Encoding.UTF8);

            Console.WriteLine("[Client] Insert two Strings separated by ':' :");
            string input = Console.ReadLine();

            writer.WriteLine($"Stringcheck:{input}");
            writer.Flush();
        }
        static void sendExit()
        {
            var reader = new StreamReader(stream, Encoding.UTF8);
            var writer = new StreamWriter(stream, Encoding.UTF8);

            writer.WriteLine($"Exit");
            writer.Flush();
        }
    }
}
