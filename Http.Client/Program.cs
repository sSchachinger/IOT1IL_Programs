using System;
using System.IO;
using System.Net;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace Http.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Client] Start");

            // HTTP Client erstellen
            var client = new HttpClient();

            //Request 1 - Not found
            var task1 = client.GetAsync("http://localhost:8081/");
            //Blockiert Main bis Antwort angekommen ist
            task1.Wait();
            //Http Antwort auf der Konsole ausgeben
            PrintResponse(task1.Result);

            //Request 2 -Ok
            var task2 = client.GetAsync("http://localhost:8081/a.html");
            //Blockiert Main bis Antwort angekommen ist
            task2.Wait();
            //Http Antwort auf der Konsole ausgeben
            PrintResponse(task2.Result);

            //Request 3 - Custom Request
            var request3 = new HttpRequestMessage();
            request3.RequestUri = new Uri("http://localhost:8081/b.html?param1=a&param2=b");
            request3.Content = new StringContent("Spalte1;Spalte2;Spalte3",null,"text/csv");

            //Anfrage senden und Antwort empfangen
            var response3 = client.Send(request3);
            PrintResponse(response3);

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
