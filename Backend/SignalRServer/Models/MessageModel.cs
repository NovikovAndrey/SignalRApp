using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.Models
{
    public class MessageModel
    {
        public long rand { get; set; }
        public MessageModel(long random)
        {
            rand = random;
        }
        public MessageModel()
        {
            rand = new Random().Next();
        }

    }
}
