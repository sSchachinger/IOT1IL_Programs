using System;
using System.IO;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace HW.HTTP.Server
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
            Console.WriteLine("[Server] Start");
            // HTTP Server stellen und starten

            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8082/prefix/");
            listener.Prefixes.Add("http://localhost:8082/");
            listener.Start();

            // HTTP Anfragen verarbeiten, solange Server läuft

            while (listener.IsListening)
            {
                // Auf HTTP Anfrage warten
                var context = listener.GetContext();
                var request = context.Request;
                var response = context.Response;
                var input = request.InputStream;
                var output = response.OutputStream;
                var reader = new StreamReader(input);
                var writer = new StreamWriter(output);

                // Methode und Pfad der HTTP Anfrage auslesen
                Console.WriteLine($"[Server] Request.HttpMethod = {request.HttpMethod}");
                Console.WriteLine($"[Server] Request.Url = {request.Url}");

                // Header der HTTP Anfrage auslesen
                foreach (string key in request.Headers)
                {
                    var value = request.Headers[key];
                    Console.WriteLine($"[Server] Request.Headers[{key}] = {value}");
                }
                // Body der HTTP Anfrage auslesen
                var body = reader.ReadToEnd();
                Console.WriteLine($"[Server] Request.Content = {body}");

                // Status Code der HTTP Antwort setzen und Body der HTTP Antwort schreiben
                if (request.Url.AbsolutePath.StartsWith("/a/"))
                {
                    if (request.HttpMethod.Equals(HttpMethod.Get.Method))
                        writer.Write("Das ist Antwort A-GET.");

                    else if (request.HttpMethod.Equals(HttpMethod.Post.Method))
                        writer.Write("Das ist Antwort A-POST.");
                    else if (request.HttpMethod.Equals(HttpMethod.Put.Method))
                        writer.Write("Das ist Antwort A-PUT.");
                    else if (request.HttpMethod.Equals(HttpMethod.Delete.Method))
                        writer.Write("Das ist Antwort A-DELETE.");
                }

                else if (request.Url.AbsolutePath.StartsWith("/b/"))
                {
                    if (request.HttpMethod.Equals(HttpMethod.Get.Method))
                        writer.Write("Das ist Antwort B-GET.");
                    else if (request.HttpMethod.Equals(HttpMethod.Post.Method))
                    {
                        Equation Payload = JsonConvert.DeserializeObject<Equation>(body);
                        Equation ResponseEquation = calculate(Payload);
                        string stringResponseEquation = JsonConvert.SerializeObject(ResponseEquation);

                        writer.Write(stringResponseEquation);

                    }
                    else if (request.HttpMethod.Equals(HttpMethod.Put.Method))
                        writer.Write("Das ist Antwort B-PUT.");
                    else if (request.HttpMethod.Equals(HttpMethod.Delete.Method))
                        writer.Write("Das ist Antwort B-DELETE.");
                }

                else if (request.Url.AbsolutePath.StartsWith("/c/"))
                {
                    if (request.HttpMethod.Equals(HttpMethod.Get.Method))
                        writer.Write("Das ist Antwort C-GET.");
                    else if (request.HttpMethod.Equals(HttpMethod.Post.Method))
                        writer.Write("Das ist Antwort C-POST.");
                    else if (request.HttpMethod.Equals(HttpMethod.Put.Method))
                        writer.Write("Das ist Antwort C-PUT.");
                    else if (request.HttpMethod.Equals(HttpMethod.Delete.Method))
                        writer.Write("Das ist Antwort C-DELETE.");
                }
                else
                {
                    // Status Code der HTTP Antwort setzen
                    response.StatusCode = ((int)HttpStatusCode.NotFound);
                    writer.Write("Pfad nicht gefunden!");
                }
                writer.Flush();

                // HTTP Antwort senden
                response.Close();
            }
            // HTTP Server stoppen
            listener.Stop();
            // HTTP Server beenden
            Console.WriteLine("[Server] End");
            Console.ReadLine();
        }

        private static Equation calculate(Equation data)
        {
            switch (data.Operator)
            {
                case ('*'):
                    data.Result = data.Numbers.param1 * data.Numbers.param2;
                    break;
                case ('/'):
                    if (data.Numbers.param2 == 0)
                    {
                        data.Informations = "Division trough 0";
                        data.Result = null;
                        break;
                    }
                    data.Result = data.Numbers.param1 * data.Numbers.param2;
                    break;
                case ('+'):
                    data.Result = data.Numbers.param1 + data.Numbers.param2;
                    break;
                case ('-'):
                    data.Result = data.Numbers.param1 - data.Numbers.param2;
                    break;
                default:
                    data.Informations = "No Operator inserted";
                    break;
            }
            return data;
        }
    }
}

