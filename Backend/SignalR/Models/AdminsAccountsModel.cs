using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class AdminsAccountsModel
    {
        public string Name { get; set; }

        public AdminsAccountsModel() { }

        public AdminsAccountsModel(string name)
        {
            Name = name;
        }
    }
}
