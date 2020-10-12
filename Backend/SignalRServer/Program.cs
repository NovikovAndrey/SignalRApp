using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalRServer.Services;

namespace SignalRServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var serviceContext = services.GetRequiredService<SendMessageService>();

                    serviceContext.StartSend();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occured, message: {ex.Message}, stacktrace: {ex.StackTrace}");
                }
            }

            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
                 .UseStartup<Startup>();
    }
}
