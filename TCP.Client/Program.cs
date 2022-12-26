using System;
using System.Diagnostics;
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
            //Task tRecieve = new Task(recieveTask);


            tSend.Start();
            //tRecieve.Start();

            while (!gRunVar)
            {
                Thread.Sleep(10000);
            }
        }
     
        static void sendTask()
        {
            bool runVar = false;
            do
            {
                Console.WriteLine("[Client] What to do? Equation, Exit or Stringcheck");
                string input = Console.ReadLine();
                Stopwatch stopwatch = new Stopwatch();

                switch (input)
                {
                    case ("Equation"):
                        stopwatch = sendEquation(stopwatch);
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
                input = "";
                var reader = new StreamReader(stream);
                string request = "";


                try
                {
                    request = reader.ReadLine() + " Elapsed Time:" + stopwatch.ElapsedMilliseconds.ToString();
                }
                catch (Exception)
                {
                }
                finally { }

                if (request == "Exit")
                {
                    runVar = true;
                    client.Close();
                    Console.WriteLine("[Server] closed");
                }
                else if (request == null) { }
                else if (request.Contains("Equation"))
                    Console.WriteLine("[Server]:" + request);
                else if (request.Contains("Stringcheck"))
                    Console.WriteLine("[Server]:" + request);


                request = "";
                Thread.Sleep(100);
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

        static Stopwatch sendEquation(Stopwatch stopwatch)
        {
            var writer = new StreamWriter(stream, Encoding.ASCII);

            Console.WriteLine("[Client] Insert Equation:");
            string input = Console.ReadLine();
            stopwatch.Restart();
            writer.WriteLine($"Equation:{input}");
            writer.Flush();
            return stopwatch;
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
            var writer = new StreamWriter(stream, Encoding.UTF8);

            writer.WriteLine($"Exit");
            writer.Flush();
        }

    }
}
