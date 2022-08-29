using Ordering.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Ordering.AsyncDataServices
{
    public class MessageBusSubscriber : IMessageBusSubscriber, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(
            IConfiguration configuration,
            IEventProcessor eventProcessor,
            IConnectionFactory connectionFactory)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            _connectionFactory = connectionFactory;

            InitializeRabbitMQ();
        }

        [MemberNotNull(nameof(_connection), nameof(_channel), nameof(_queueName))]
        private void InitializeRabbitMQ()
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _configuration["RabbitMQExchange"], type: ExchangeType.Fanout, durable: true);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_queueName, _configuration["RabbitMQExchange"], "");

            Console.WriteLine("--> Listening on the message bus");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        public void CreateConsumer()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += OnEventReceived;

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        private void OnEventReceived(Object? sender, BasicDeliverEventArgs eventArgs)
        {
            Console.WriteLine("--> Event Received!");

            var body = eventArgs.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown");
        }

        public void Dispose()
        {
            Console.WriteLine("--> RabbitMQ connection shutdown");

            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}