using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
                    var result = await client.PostAsync("http://publisher_api:80/api/values", content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    Console.WriteLine($"Server returned: {resultContent}");
                }
            }


        }


        static void Main(string[] args)
        {
            string[] testStrings = new string[] { "one", "two", "three", "four", "five" };

            Console.WriteLine("Sleeping to wait for Rabbit");
            Task.Delay(10000).Wait();
            Console.WriteLine("Posting messages to webApi");
            foreach(var message in testStrings)
            {
                PostMessage(message).Wait();
            }

            Task.Delay(1000).Wait();
            Console.WriteLine("Consuming Queue Now");

            ConnectionFactory factory = new ConnectionFactory { HostName = "rabbitmq", Port = 5672 };
            factory.UserName = "guest";
            factory.Password = "guest";

            factory.UserName = "guest";
            factory.Password = "guest";
            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received from Rabbit: {message}");
            };

            channel.BasicConsume(queue: "hello",
                                    autoAck: true,
                                    consumer: consumer);

        }
    }
}
