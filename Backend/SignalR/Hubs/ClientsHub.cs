using Microsoft.AspNetCore.SignalR;
using SignalR.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Hubs
{
    public class ClientsHub: Hub
    {
        private readonly string Type = "Client";

        public void Connect(string name)
        {
            MainController.UserConnect(Context.ConnectionId, name, Type, 1);
        }

        //public void Disconnect(string name)
        //{
        //    MainController.UserDisconnect(Context.ConnectionId, name, Type, 0);
        //}

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            MainController.UserDisconnect(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

    }
}
