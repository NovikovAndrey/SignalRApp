using Microsoft.Extensions.DependencyInjection;
using SignalRServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.Extension
{
    public static class ServiceProviderExtensions
    {
        public static void AddSendMessageService(this IServiceCollection services)
        {
            services.AddSingleton<SendMessageService>();
        }
    }
}
