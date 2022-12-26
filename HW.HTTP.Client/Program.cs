using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http;

namespace HW.HTTP.Client
{
    internal class Program
    {
        public class Numbers
        {
            public double param1 { get; set; }
            public double param2 { get; set; }
        }
        public class Equation
        {
            public Numbers Numbers { get; set; }
            public char Operator { get; set; }
            public string Informations { get; set; }
            public double? Result { get; set; }
        }
        static void Main(string[] args)
        {
            bool runVar = false;

            Console.Title = "HTTP-Client";
            Console.WriteLine("[Client] Start");
            do
            {
                Console.WriteLine("[Client] What to do? Equation or Exit");
                string input = Console.ReadLine();
                Stopwatch stopwatch = new Stopwatch();

                Console.WriteLine("[Client] Insert Operator:");
                char.TryParse(Console.ReadLine(), out char operators);
                Console.WriteLine("[Client] Insert param1:");
                double.TryParse(Console.ReadLine(), out double a);
                Console.WriteLine("[Client] Insert param2:");
                double.TryParse(Console.ReadLine(), out double b);


                var payload = new Equation
                {
                    Numbers = new Numbers { param1 = a, param2 = b },
                    Operator = operators,
                    Informations = "",
                    Result = 0
                };

                var stringPayload = JsonConvert.SerializeObject(payload);

                switch (input)
                {
                    case ("Equation"):
                        // HTTP Client erstellen
                        var client = new HttpClient();

                        // HTTP Anfrage bauen
                        var request = new HttpRequestMessage();
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri("http://localhost:8082/b/");
                        request.Content = new StringContent(stringPayload);

                        // HTTP Anfrage senden und auf Antwort warten
                        var sendTask = client.SendAsync(request);
                        sendTask.Wait();

                        var response = sendTask.Result;

                        // Status Code der HTTP Antwort auslesen
                        //Console.WriteLine($"[Client] Response.StatusCode = {response.StatusCode}");

                        // Header der HTTP Antwort auslesen
                        //foreach (var header in response.Headers)
                        //{
                        //    var key = header.Key;
                        //    var values = header.Value;
                        //    foreach (var value in values)
                        //    {
                        //        Console.WriteLine($"[Client] Response.Headers[{key}] = {value}");
                        //    }
                        //}

                        // Body der HTTP Antwort auslesen
                        var readTask = response.Content.ReadAsStringAsync();
                        readTask.Wait();

                        var body = readTask.Result;
                        Equation Payload = JsonConvert.DeserializeObject<Equation>(body);
                        Console.WriteLine($"[Client] Result = {Payload.Result}");
                        Console.WriteLine($"[Client] Information = {Payload.Informations}");



                        break;
                    case ("Exit"):
                        runVar = true;

                        // HTTP Client beenden
                        Console.WriteLine("[Client] End");
                        Console.ReadLine();
                        break;
                    default:
                        break;
                }

            } while (!runVar);











        }
    }
}
