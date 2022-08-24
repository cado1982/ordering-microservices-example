using System.Text;
using System.Text.Json;
using Accounting.Dtos;
using RabbitMQ.Client;

namespace Accounting.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection? _connection;
        private readonly IModel? _channel;

        public MessageBusClient(IConfiguration configuration, IConnectionFactory connectionFactory)
        {
            _configuration = configuration;

            try 
            {
                _connection = connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: _configuration["RabbitMQExchange"], type: ExchangeType.Fanout, durable: true);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to message bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Cound not connect to the Message Bus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Rabbit MQ connection shutdown");
        }

        public void PublishNewAccount(AccountPublishedDto accountPublishedDto)
        {
            var message = JsonSerializer.Serialize(accountPublishedDto);

            if (_connection!.IsOpen)
            {
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed. Not sending message");
            }
        }

        public void Dispose()
        {
            Console.WriteLine("--> Message bus disposed");
            if (_channel!.IsOpen)
            {
                _channel!.Close();
                _connection!.Close();
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: _configuration["RabbitMQExchange"], 
                routingKey: string.Empty,
                basicProperties: null,
                body: body);

            Console.WriteLine($"--> We have sent {message}");
        }
    }
}