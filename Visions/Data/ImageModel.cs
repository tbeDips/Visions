namespace Visions.Data
{
    public class ImageModel
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string Description { get; set; }
        public string ImageBase64 => Convert.ToBase64String(Data);
    }
}
