using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class ImageModel
    {
        public string ImageBase64 { get; set; }
        public string Type { get; set; }

        public ImageModel(string imageBase64)
        {
            ImageBase64 = imageBase64;
            Type = "data:image/png;base64,";
        }
    }
}
