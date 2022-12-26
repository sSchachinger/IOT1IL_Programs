using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text;
using static System.Net.WebRequestMethods;
using Json.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace Http.Client1
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
            public override string ToString()
            {
                return base.ToString();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("[Client] Start");

            // HTTP Client erstellen
            var client = new HttpClient();




            //var addtask = new HttpRequestMessage();
            //addtask.RequestUri = new Uri("http://localhost:8083/b.html?param1=a&param2=b");
            //addtask.Content = new StringContent("");

            //var response = client.Send(addtask);
            //PrintResponse(response);



            ////Request 1 - Not found
            //var task1 = client.GetAsync("http://localhost:8081/");
            ////Blockiert Main bis Antwort angekommen ist
            //task1.Wait();
            ////Http Antwort auf der Konsole ausgeben
            //PrintResponse(task1.Result);

            ////Request 2 -Ok
            //var task2 = client.GetAsync("http://localhost:8081/a.html");
            ////Blockiert Main bis Antwort angekommen ist
            //task2.Wait();
            ////Http Antwort auf der Konsole ausgeben
            //PrintResponse(task2.Result);

            ////Request 3 - Custom Request
            //var request3 = new HttpRequestMessage();
            //request3.RequestUri = new Uri("http://localhost:8083/b.html?param1=a&param2=b");
            //request3.Content = new StringContent("Spalte1;Spalte2;Spalte3", null, "text/csv");

            ////Anfrage senden und Antwort empfangen
            //var response3 = client.Send(request3);
            //PrintResponse(response3);

            int a = 2;
            int b = 3;
            char[] operators = { '*', '/', '+', '-' };

            string equation = $"param1={a}&param2={b}&operator={operators[1]}";
            string uriString = "http://localhost:8083/b.html?" + equation;

            var payload = new Equation
            {
                Numbers = new Numbers { param1 = a, param2 = b },
                Operator = operators[0]
            };

            var stringPayload = JsonConvert.SerializeObject(payload);
  
            //Request 3 - Custom Request
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://localhost:8083/b.html");
           // request.Content = new StringContent(stringPayload, Encoding.UTF8);
            request.Content = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            //Anfrage senden und Antwort empfangen
            var response = client.Send(request);
            PrintResponse(response);


            //var task4 = client.GetAsync("https://www.google.com/search?q=fh+wels");
            //task4.Wait();
            //var response4 = task4.Result;
            //PrintResponse(response4);




            // HTTP Client beenden
            Console.WriteLine("[Client] End");
            Console.ReadLine();
        }

        private static void PrintResponse(HttpResponseMessage response)
        {
            // Status Code der HTTP Antwort auslesen
            Console.WriteLine($"[Client] Response.StatusCode = {response.StatusCode}");
            Console.WriteLine($"[Client] Response.ReasonPhrase = {response.ReasonPhrase}");

            // Header der HTTP Antwort auslesen
            foreach (var header in response.Headers)
            {
                var key = header.Key;
                var values = header.Value;
                foreach (var value in values)
                {
                    Console.WriteLine($"[Client] Response.Headers[{key}] = {value}");
                }
            }

            // Body als String auslesen
            var task = response.Content.ReadAsStringAsync();
            //Warten bis Body vollständig angekommen ist
            task.Wait();
            //Body auslesen
            var body = task.Result;
            //Body ausgeben
            Console.WriteLine($"[Client] Response.Content = {body}");
        }
    }
}
