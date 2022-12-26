using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace HTTP.Server
{
    internal class Program
    {
        static void Main(string[] args)

        {
            Console.Title = "HTTP-Server";
            Console.WriteLine("[Server] Start");
            //Toddo      Server starten , Anfragen verarbeiten
            // ASP.NET wird üblicherweise verwendet
            //Wir nutzen Basis API

            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8081/");
            listener.Prefixes.Add("http://127.0.0.1:8081/");
            listener.Prefixes.Add("http://localhost:8082/");
            listener.Prefixes.Add("http://127.0.0.1:8082/");

            listener.Start();

            while (listener.IsListening)
            {
                //  HTTP Anfragen empfangen und Antworten senden
                Console.WriteLine("[Server] Wait for request");
                // eingehende Http Anfrage entgegennehmen 
                var context = listener.GetContext();

                // HTTP Anfrage-Objekt auslesen
                var request = context.Request;

                //Anfragezeile der HTTP-Anfrage auslösen
                Console.WriteLine($"[Server] Request.HttpMethod= {request.HttpMethod}");
                Console.WriteLine($"[Server] Request.Url= {request.Url}");
                Console.WriteLine($"[Server] Request.Url.Host= {request.Url.Host}");
                Console.WriteLine($"[Server] Request.Url.Port= {request.Url.Port}");
                Console.WriteLine($"[Server] Request.AbsoultePath= {request.Url.AbsolutePath}");
                Console.WriteLine($"[Server] Request.Url.Query= {request.Url.Query}");

                Console.WriteLine($"[Server] Request.ContentType= {request.ContentType}");
                Console.WriteLine($"[Server] Request.ContentEncoding= {request.ContentEncoding}");
                Console.WriteLine($"[Server] Request.ContentLength64= {request.ContentLength64}");
                //Header-Zusatzfelder der HTTP-Anfrage auslesen
                foreach (string key in request.Headers)
                {
                    var value = request.Headers[key];
                    Console.WriteLine($"[Server] Request.Headers[{key}] = {value}");
                }

                var stream = request.InputStream;
                var reader = new StreamReader(stream, request.ContentEncoding);
                var body = reader.ReadToEnd();

                // HTTP Antwort auslesen;
                var response = context.Response;

                if (request.Url.Host.Equals("localhost"))
                {
                    var writer = new StreamWriter(response.OutputStream);
                    if (request.Url.AbsolutePath.Equals("/a.html") && request.HttpMethod.Equals(HttpMethod.Get.ToString()))
                    {
                        //Statuszeile der HTTP-Antwort definiern
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.StatusDescription = "OK";
                        //Header-Zusatfelder der HTTP-Antwort definiern
                        response.Headers[HttpResponseHeader.ContentType] = "text/html";
                        //Body der HTTP-Antwort definieren
                        writer.WriteLine("<html><body>Hallo Welt!</body></html>");
                    }
                    else if (request.Url.AbsolutePath.Equals("/b.html") && request.HttpMethod.Equals(HttpMethod.Get.ToString()))
                    {
                        //Statuszeile der HTTP-Antwort definiern
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusDescription = "OK";
                        //Header-Zusatfelder der HTTP-Antwort definiern
                        response.Headers[HttpResponseHeader.ContentType] = "text/html";

                        //Body der HTTP-Antwort definieren
                        writer.WriteLine("<html><body>Hallo Welt!</body></html>");
                    }
                    else
                    {
                        //Statuszeile der HTTP-Antwort definiern
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusDescription = " Path not found on server";
                    }
                    writer.Flush();
                }
                else
                {
                    //Statuszeile der HTTP-Antwort definiern
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "Not found";
                }
                response.Close();
            }
            listener.Stop();
            Console.WriteLine("[Server] End");
            Console.ReadLine();
        }
    }
}
