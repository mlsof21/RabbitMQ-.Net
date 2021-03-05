using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace worker
{
    class Program
    {
        public static async Task PostMessage(string postData)
        {
            var json = JsonSerializer.Serialize(postData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

                using (var client = new HttpClient(httpClientHandler))
                {
                    var result = await client.PostAsync("https://localhost:44342/api/values", content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    Console.WriteLine($"Server returned: {resultContent}");
                }
            }


        }


        static void Main(string[] args)
        {
            Console.WriteLine("Posting a message!");
            PostMessage("test message").Wait();
        }
    }
}
