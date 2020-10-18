using Microsoft.AspNetCore.SignalR;
using SignalR.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Hubs
{
    public class AdminsHub: Hub
    {
        private readonly string Type = "Admin";

        public void Connect(string name)
        {
            MainController.UserConnect(Context.ConnectionId, name, Type, 1);
        }

        public void Disconnect(string name)
        {
            MainController.UserDisconnect(Context.ConnectionId, name, Type, 0);
        }

        public override async Task OnConnectedAsync()
        {
            //MainController.UserConnect(Context.ConnectionId, "Admin", 1);
            //await Clients.Client(Context.ConnectionId).SendAsync("Hi", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            MainController.UserDisconnect(Context.ConnectionId);
            //MainController.UserDisconnect(Context.ConnectionId, "Admin", 0);
            //await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} покинул в чат");
            await base.OnDisconnectedAsync(exception);
        }

    }
}
