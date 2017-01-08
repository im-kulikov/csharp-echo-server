using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace LargeResponseApp
{
    public class Startup
    {
        byte[] data = Encoding.UTF8.GetBytes("Привет мир!");

        public void Configure(IApplicationBuilder App)
        {
            App.Run(async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                context.Response.ContentLength = data.Length;
                
                await context.Response.Body.WriteAsync(data, 0, data.Length);
            });
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseUrls("http://*:8080")
                .UseKestrel(options => {})
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}