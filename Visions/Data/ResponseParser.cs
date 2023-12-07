using Newtonsoft.Json;

public class ResponseParser
{
    public static ChatCompletion ParseResponse(string jsonResponse)
    {
        return JsonConvert.DeserializeObject<ChatCompletion>(jsonResponse);
    }
}

public class ChatCompletion
{
    public string id { get; set; }
    //public string  { get; set; }
    public long created { get; set; }
    public string model { get; set; }
    public Usage usage { get; set; }
    public IEnumerable<Choice> choices { get; set; }
}

public class Usage
{
    // make gpt-4-1106-vision-preview	$0.01 / 1K tokens	$0.03 / 1K tokens
    private double USDperToken = 0.01 / 1000;
    private double NOKperUSD = 10.92;

    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
    public double USD => total_tokens * USDperToken;
    public double Ore => USD * NOKperUSD /10 ;

}

public class Choice
{
    public Message message { get; set; }
    public FinishDetails finish_details { get; set; }
    public int index { get; set; }
}

public class Message
{
    public string role { get; set; }
    public string content { get; set; }
}

public class FinishDetails
{
    public string type { get; set; }
    public string stop { get; set; }
}
