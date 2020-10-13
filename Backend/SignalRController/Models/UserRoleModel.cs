using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRController.Models
{
    public class UserRoleModel
    {
        public string Name { get; set; }
        public string Role { get; set; }

        public UserRoleModel() { }
        public UserRoleModel(string name, string role)
        {
            Name = name;
            Role = role;
        }
    }
}
