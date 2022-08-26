using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Moq;
using Ordering.AsyncDataServices;
using Ordering.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ordering.Tests.AsyncDataServices
{
    public class MessageBusSubscriberTests
    {
        private MessageBusSubscriber? _sut;

        private Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private Mock<IEventProcessor> _eventProcessorMock = new Mock<IEventProcessor>();
        private Mock<IConnectionFactory> _connectionFactoryMock = new Mock<IConnectionFactory>();
        private Mock<IConnection> _connectionMock = new Mock<IConnection>();
        private Mock<IModel> _channelMock = new Mock<IModel>();

        private string _exchangeName = "SampleExchangeName";

        public MessageBusSubscriberTests()
        {
            _configurationMock.Setup(c => c["RabbitMQExchange"]).Returns(_exchangeName);

            _connectionFactoryMock.Setup(f => f.CreateConnection()).Returns(_connectionMock.Object);
            _connectionMock.Setup(c => c.CreateModel()).Returns(_channelMock.Object);
            _connectionMock.Setup(c => c.IsOpen).Returns(true);
            _channelMock.Setup(c => c.IsOpen).Returns(true);
            _channelMock.Setup(c => c.QueueDeclare("", false, true, true, null)).Returns(new QueueDeclareOk("", 0, 0));
            
        }

        [MemberNotNull(nameof(_sut))]
        private void CreateSUT()
        {
            _sut = new MessageBusSubscriber(_configurationMock.Object, _eventProcessorMock.Object, _connectionFactoryMock.Object);
        }

        [Fact]
        public void WhenDisposed_ClosesConnection()
        {
            // Arrange
            CreateSUT();

            // Act
            _sut.Dispose();

            // Assert
            _channelMock.Verify(c => c.Close(), Times.Once);
            _connectionMock.Verify(c => c.Close(), Times.Once);
        }

        [Fact]
        public void ExecuteAsync_CallsConsumeOnTheChannel()
        {
            // Arrange
            CreateSUT();

            // Act
            _sut.CreateConsumer();

            // Assert
            _channelMock.Verify(c => c.BasicConsume(It.IsAny<string>(), true, "", false, false, null, It.IsAny<EventingBasicConsumer>()), Times.Once);
        }
    }
}