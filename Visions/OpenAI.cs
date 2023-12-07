using System.IO;
using System.Text;
using Newtonsoft.Json;
using Visions.Data;
using System.Diagnostics;
using static Visions.OpenAI;

namespace Visions;

public partial class OpenAI
{
    private const string APIURL = "https://api.openai.com/v1/chat/completions";
    private readonly string APIKey;

    public OpenAI()
    {
        APIKey = LoadApiKeyFromFile(APIKeyPath);

    }

    private static string APIKeyPath => @"C:\keys\APIKEY.text";

    private static string LoadApiKeyFromFile(string filePath)
    {
        var path = Path.Combine(Environment.CurrentDirectory, filePath);
        if (!File.Exists(filePath))
        {
            return "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";
            throw new Exception($"File not found: {path}");
        }
        string apiKey = File.ReadAllText(path);
        return apiKey;
    }

    public string MakePayload(string imageBase64, string prompt, int maxTokens = 150, bool detailHigh = false)
    {
        var request = new JsonRequest
        {
            model = "gpt-4-vision-preview",
            messages = new[]
            {
                new Message
                {
                    role = "user",
                    content = new[]
                    {
                        new Content { 
                            type = "text", 
                            text = "What’s in this image?" },
                        new Content
                        {
                            type = "image_url",
                            image_url = new ImageUrl
                            {
                                url = $"data:image/jpeg;base64,{imageBase64}",
                                detail = detailHigh ? "high" : "low"

                            }
                        }
                    }
                }
            },
            max_tokens = maxTokens
        };

        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        return JsonConvert.SerializeObject(request, Formatting.Indented, settings);
    }

    public async Task<ChatCompletion> GetImageDescription(ImageModel image, string prompt)
    {
        ChatCompletion result = new ChatCompletion();
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {APIKey}");

            var payload = MakePayload(image.ImageBase64, prompt);
            Console.WriteLine(payload);
            var response = await client.PostAsync(APIURL, new StringContent(payload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                result = ResponseParser.ParseResponse(await response.Content.ReadAsStringAsync());
            }
        }
        return result;
    }

}

