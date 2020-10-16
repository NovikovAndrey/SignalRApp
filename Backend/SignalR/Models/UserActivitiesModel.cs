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

        public UserActivitiesModel(string connectionId, string name, string role, int status): base(connectionId, name, role)
        {
            Status = status;
        }

    }
}
