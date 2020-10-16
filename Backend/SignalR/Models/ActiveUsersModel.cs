using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class ActiveUsersModel
    {
        public string ConnectionId { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public ActiveUsersModel() { }
        public ActiveUsersModel(string connectionId, string name, string role)
        {
            ConnectionId = connectionId;
            Name = name;
            Role = role;
        }
    }
}
