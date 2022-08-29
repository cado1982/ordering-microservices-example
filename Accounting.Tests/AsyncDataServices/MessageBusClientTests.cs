using Accounting.AsyncDataServices;
using Accounting.Dtos;
using Accounting.EventProcessing;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;

namespace Accounting.Tests.AsyncDataServices
{
    public class MessageBusClientTests
    {
        private MessageBusClient? _sut;

        private Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private Mock<IConnection> _connectionMock = new Mock<IConnection>();
        private Mock<IModel> _channelMock = new Mock<IModel>();
        private Mock<IConnectionFactory> _connectionFactoryMock = new Mock<IConnectionFactory>();

        private string _exchangeName = "SampleExchangeName";

        public MessageBusClientTests()
        {
            _configurationMock.Setup(c => c["RabbitMQExchange"]).Returns(_exchangeName);

            _connectionFactoryMock.Setup(f => f.CreateConnection()).Returns(_connectionMock.Object);
            _connectionMock.Setup(c => c.CreateModel()).Returns(_channelMock.Object);
            _connectionMock.Setup(c => c.IsOpen).Returns(true);
            _channelMock.Setup(c => c.IsOpen).Returns(true);
        }

        [Fact]
        public void Constructor_DeclaresExchange()
        {
            // Arrange
            // Act
            CreateSUT();

            // Assert
            _channelMock.Verify(c => c.ExchangeDeclare(_exchangeName, ExchangeType.Fanout, true, false, null), Times.Once);
        }

        [Fact]
        public void PublishNewAccount_SendsRabbitMQMessage()
        {
            // Arrange
            CreateSUT();
            var accountPublishedDto = new AccountPublishedDto { EventType = EventType.AccountPublished, Id = 10 };

            // Act
            _sut.PublishNewAccount(accountPublishedDto);

            // Assert
            _channelMock.Verify(c => c.BasicPublish(_exchangeName, String.Empty, false, null, It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
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
            _channelMock.Verify(c => c.Close(), Times.Once);
            _connectionMock.Verify(c => c.Close(), Times.Once);
        }

        [MemberNotNull(nameof(_sut))]
        private void CreateSUT()
        {
            _sut = new MessageBusClient(_configurationMock.Object, _connectionFactoryMock.Object);
        }
    }
}