using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRController.Hubs
{
    public class AdminHub: Hub
    {
        public Task SendUsers(string t)
        {
            return Clients.All.SendAsync("GetUsers", t);
        }
    }
}
