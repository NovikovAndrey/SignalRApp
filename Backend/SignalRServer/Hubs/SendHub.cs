using Microsoft.AspNetCore.SignalR;
using SignalRServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.Hubs
{
    public class SendHub:Hub
    {
        public Task SendMessage()
        {
            return Clients.All.SendAsync("ReceiveOne", "dsfsdfds");
        }

        public Task SendUserMessage(string message)
        {
            return Clients.All.SendAsync("ReceiveMany", message);
        }
    }
}
