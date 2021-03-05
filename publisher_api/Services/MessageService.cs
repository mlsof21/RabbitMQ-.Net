using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace publisher_api.Services
{
    public interface IMessageService    {
        bool Enqueue(string message);

    }
    public class MessageService : IMessageService
    {
        ConnectionFactory _factory;
        IConnection _connection;
        IModel _channel;

        public MessageService()
        {
            Console.WriteLine("About to connect to rabbit");

            _factory = new ConnectionFactory { HostName = "rabbitmq", Port = 5672 };
            _factory.UserName = "guest";
            _factory.Password = "guest";
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        }


        public bool Enqueue(string message)
        {
            var body = Encoding.UTF8.GetBytes($"server processed {message}");
            _channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
            Console.WriteLine($" [x] Published {message} to RabbitMQ");

            return true;

        }
    }
}
