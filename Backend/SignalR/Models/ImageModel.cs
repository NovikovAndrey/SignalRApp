using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class ImageModel
    {
        public string blob { get; set; }
        public string type { get; set; }

        public ImageModel(string imageBase64)
        {
            blob = imageBase64;
            type = "data:image/png;base64,";
        }
    }
}
