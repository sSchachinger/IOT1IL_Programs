using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace TCP.Server
{
    class Program
    {
        static NetworkStream stream;
        static TcpClient client;
        static bool gRunVar = false;

        static void Main(string[] args)
        {
            Console.Title = "TCP-Server";
            stream = initConnection();
            Task tRecieve = new Task(recieveTask);
            tRecieve.Start();

            while (!gRunVar)
            {
                Thread.Sleep(10000);
            }
        }

        static void recieveTask()
        {
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream, Encoding.UTF8);

            string request;
            bool runVar = false;

            do
            {
                request = reader.ReadLine();

                if (request == "Exit")
                {
                    gRunVar = true;
                    runVar = true;
                    client.Close();
                    Console.WriteLine("[Server] closed");
                }
                else if (request == null) { }
                else if (request.Contains("Equation"))
                    sendResponse(Equation(request), writer);
                else if (request.Contains("Stringcheck"))
                    sendResponse(Stringcheck(request), writer);
                
                Thread.Sleep(100);
            } while (!runVar);
        }

        static NetworkStream initConnection()
        {
            var adress = IPAddress.Parse("127.0.0.1");
            var port = 3001;
            var listener = new TcpListener(adress, port);

            listener.Start();
            client = listener.AcceptTcpClient();
            return client.GetStream();
        }

        static void sendResponse(string response, StreamWriter writer)
        {
            writer.WriteLine($" Response: {response}");
            writer.Flush();
        }

        static string Equation(string request)
        {
            char[] operators = { '*', '/', '+', '-' };
            char separatedChar = ' ';
            int[] numbers = new int[2];

            foreach (var item in operators)
            {
                if (request.Contains(item))
                {
                    string[] strings = request.Split(item);
                    strings[0] = strings[0].Remove(0, 9);
                    for (int i = 0; i < strings.Length; i++)
                    {
                        if (!int.TryParse(strings[i], out numbers[i]))
                            Console.WriteLine("Not a number");
                    }
                    separatedChar = item;
                }
            }
            double result = 0;
            switch (separatedChar)
            {
                case ('*'):
                    result = numbers[0] * numbers[1];
                    break;
                case ('/'):
                    if (numbers[1] == 0)
                        return "Division trough 0";
                    result = numbers[0] / numbers[1];
                    break;
                case ('+'):
                    result = numbers[0] + numbers[1];
                    Console.WriteLine($"[Server]:{result}");
                    break;
                case ('-'):
                    result = numbers[0] - numbers[1];
                    break;
            }
            return "Equation: " + result.ToString();
        }

        static string Stringcheck(string request)
        {
            string[] strings = request.Split(':');
            if (strings[1].Equals(strings[2]))
            {
                Console.WriteLine("[Server]: Equal string");
                return "Stringcheck: Equal String";
            }
            else
            {
                Console.WriteLine("[Server]: Not equal string");
                return "Stringcheck: Not equal String";
            }
        }
    }
}

