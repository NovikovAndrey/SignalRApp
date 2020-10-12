using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRController.Models
{
    public class AdminMessagesModel
    {
        public string Name { get; set; }
        public int Status { get; set; }

        public AdminMessagesModel() { }

        public AdminMessagesModel(string name, int status) {
            Name = name;
            Status = status;
        }
    }
}
