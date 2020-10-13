using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRController.Models
{
    public class MessageModel : RolesModel
    {
        public string Name { get; set; }
        public int Status { get; set; }

        public MessageModel() { }

        public MessageModel(string name, int status)
        {
            Name = name;
            Status = status;
        }
    }
}
