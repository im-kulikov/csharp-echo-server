using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Linq;

namespace SimpleWebServer
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);
            
            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.ContentType = "text/html; charset=utf8";
                                ctx.Response.ContentEncoding = Encoding.UTF8;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                                
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            WebServer ws = new WebServer(SendResponse, "http://+:8080/");
            ws.Run();
            Console.WriteLine("A simple webserver. Press Enter quit.");

            Thread.Sleep(Timeout.Infinite);

            //Under mono if you deamonize a process a Console.ReadLine with cause an EOF 
            //so we need to block another way
            if (args.Any(s => s.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
            {
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                Console.ReadKey();
            }
            ws.Stop();
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            return string.Format("Привет мир!");
            
        }
    }
}

