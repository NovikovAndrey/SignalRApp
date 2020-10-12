﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRController.Hubs
{
    public class UserHub : Hub
    {
        public Task SendMessages()
        {
            return Clients.All.SendAsync("SendMessages");
        }
    }
}