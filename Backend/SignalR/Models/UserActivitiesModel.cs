using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class UserActivitiesModel : ActiveUsersModel
    {
        public int Status { get; set; }

        public UserActivitiesModel(string name, string connectionId, string role, int status): base(name, connectionId, role)
        {
            Status = status;
        }

    }
}
