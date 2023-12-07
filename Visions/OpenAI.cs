using System.Text;
using Newtonsoft.Json;

namespace Visions;

public class OpenAI
{
    const string APIURL = "https://api.openai.com/v1/engines/davinci/completions";
    const string APIKeyPath = "C:\\Users\\tbe\\OneDrive - DIPS AS\\dhack\\APIKey.txt";

    public OpenAI()
    {
        APIKey = LoadAPIKeyFromFile(APIKeyPath);
        if (expr)
        {
            
        }
        {            throw new Exception("APIKey is null or empty");        }
    }
    private static string APIKey { get; set; }
    private static string LoadAPIKeyFromFile(string filePath)
    {
        string apiKey = File.ReadAllText(filePath);
        return apiKey;
    }

    public string MakePayload(string imageBase64, string prompt, int maxTokens = 150)
    {
        var payload = new
        {
            model = "gpt-4-vision-preview",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new
                        {
                            type = "text",
                            text = prompt
                        },
                        new
                        {
                            type = "image_url",
                            image_url = new
                            {
                                url = $"data:image/jpeg;base64,{imageBase64}"
                            }
                        }
                    }
                }
            },

            max_tokens = maxTokens
        };

        return JsonConvert.SerializeObject(payload);
    }

    public async Task<string> GetImageDescription(string imageBase64, string prompt)
    {

        string imageDescription = string.Empty;
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {APIKey}");
            var payload = MakePayload(imageBase64, prompt);
            var response = await client.PostAsync(APIURL, new StringContent(payload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                imageDescription = responseContent;
            }
        }
        return imageDescription;
    }
}
