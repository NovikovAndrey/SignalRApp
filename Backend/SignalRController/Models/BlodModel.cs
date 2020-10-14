namespace SignalRController.Models
{
    public class BlodModel
    {
        public string Blob { get; set; }
        public string Type { get; set; }

        public BlodModel() { }
        public BlodModel(string blob, string type)
        {
            Blob = blob;
            Type = type;
        }
    }
}
