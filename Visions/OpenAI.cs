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
            return Secrets.Keys.OpenAIApi;
            throw new Exception($"File not found: {path}");
        }
        string apiKey = File.ReadAllText(path);
        return apiKey;
    }

    public string MakePayload(string imageBase64, string prompt, int maxTokens = 50, bool detailHigh = false)
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
                            text = prompt },
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

    public async Task<ChatCompletion> GetImageDescription(ImageModel image, string prompt, int maxTokens,bool detailHigh = false)
    {
        Console.WriteLine("GetImageDescription started");
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {APIKey}");

            Console.WriteLine("GetImageDescription calling api");
            var payload = MakePayload(image.ImageBase64, prompt, maxTokens, detailHigh);
            

            var response = await client.PostAsync(APIURL, new StringContent(payload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("GetImageDescription return success");
                return ResponseParser.ParseResponse(await response.Content.ReadAsStringAsync());

            }
        }
        throw new Exception("GetImageDescription failed");
    }

}

