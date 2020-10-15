﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class ActiveUsersModel
    {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public string Role { get; set; }

        public ActiveUsersModel() { }
        public ActiveUsersModel(string name, string connectionId, string role)
        {
            Name = name;
            ConnectionId = connectionId;
            Role = role;
        }
    }
}
