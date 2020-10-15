using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class UsersRoleModel
    {
        public string Name { get; set; }
        public string Role { get; set; }

        public UsersRoleModel(string name, string role)
        {
            Name = name;
            Role = role;
        }
    }
}
