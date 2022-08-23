using System.Diagnostics.CodeAnalysis;
using Accounting.AsyncDataServices;
using Accounting.Dtos;
using Microsoft.Extensions.Configuration;
using Moq;
using RabbitMQ.Client;

namespace Accounting.Tests.AsyncDataServices
{
    public class MessageBusClientTests
    {
        private MessageBusClient? _sut;

        private Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
        private Mock<IConnection> _connection = new Mock<IConnection>();
        private Mock<IModel> _channel = new Mock<IModel>();
        private Mock<IConnectionFactory> _connectionFactory = new Mock<IConnectionFactory>();

        private string _exchangeName = "SampleExchangeName";

        public MessageBusClientTests()
        {
            _configuration.Setup(c => c["RabbitMQExchange"]).Returns(_exchangeName);

            _connectionFactory.Setup(f => f.CreateConnection()).Returns(_connection.Object);
            _connection.Setup(c => c.CreateModel()).Returns(_channel.Object);
            _connection.Setup(c => c.IsOpen).Returns(true);
            _channel.Setup(c => c.IsOpen).Returns(true);
        }

        [Fact]
        public void Constructor_DeclaresExchange()
        {
            // Arrange
            // Act
            CreateSUT();

            // Assert
            _channel.Verify(c => c.ExchangeDeclare(_exchangeName, ExchangeType.Fanout, true, false, null), Times.Once);
        }

        [Fact]
        public void PublishNewAccount_SendsRabbitMQMessage()
        {
            // Arrange
            CreateSUT();
            var accountPublishedDto = new AccountPublishedDto{ Event = "Account_Published", Id = 10 };

            // Act
            _sut.PublishNewAccount(accountPublishedDto);

            // Assert
            _channel.Verify(c => c.BasicPublish(_exchangeName, String.Empty, false, null, It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
        }

        [Fact]
        public void WhenDisposed_ClosesConnection()
        {
            // Arrange
            CreateSUT();

            // Act
            using (_sut)
            {

            }

            // Assert
            _channel.Verify(c => c.Close(), Times.Once);
            _connection.Verify(c => c.Close(), Times.Once);
        }

        [MemberNotNull(nameof(_sut))]
        private void CreateSUT()
        {
            _sut = new MessageBusClient(_configuration.Object, _connectionFactory.Object);
        }
    }
}