using System.Text;
using Ordering.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ordering.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = Int32.Parse(_configuration["RabbitMQPort"])
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _configuration["RabbitMQExchange"], type: ExchangeType.Fanout, durable: true);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_queueName, _configuration["RabbitMQExchange"], "");

            Console.WriteLine("--> Listening on the message bus");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (moduleHandle, ea) => 
            {
                Console.WriteLine("--> Event Received!");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown");
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}