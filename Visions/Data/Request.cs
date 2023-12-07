namespace Visions;

public partial class OpenAI
{
    public class Content
    {
        public string type { get; set; }
        public string text { get; set; }
        public ImageUrl image_url { get; set; }
    }
    public class ImageUrl
    {
        public string url { get; set; }
        public string detail { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public Content[] content { get; set; }
    }

    public class JsonRequest
    {
        public string model { get; set; }
        public Message[] messages { get; set; }
        public int max_tokens { get; set; }
    }
}