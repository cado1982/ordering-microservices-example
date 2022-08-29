using Microsoft.Extensions.DependencyInjection;
using Ordering.EventProcessing;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Ordering.Tests.EventProcessing
{
    public class EventProcessorTests
    {
        private EventProcessor? _sut;

        private Mock<IEventHandler> _eventHandlerMock = new Mock<IEventHandler>();
        private Mock<IServiceProvider> _serviceProviderMock = new Mock<IServiceProvider>();

        public EventProcessorTests()
        {
            _eventHandlerMock.Setup(h => h.EventType).Returns(EventType.AccountPublished);

            RegisterServiceProvider();
        }

        [MemberNotNull(nameof(_sut))]
        private void CreateSUT()
        {
            _sut = new EventProcessor(_serviceProviderMock.Object);
        }

        [Fact]
        public void ProcessEvent_DeterminesCorrectEvent()
        {
            // Arrange
            CreateSUT();

            // Act
            var message = "{ \"EventType\": \"AccountPublished\" }";
            _sut.ProcessEvent(message);

            // Assert
            _eventHandlerMock.Verify(h => h.Handle(message));
        }

        [Fact]
        public void ProcessEvent_WhenInvalidEventType_ThrowsException()
        {
            // Arrange
            CreateSUT();

            // Act
            var message = "{ \"EventType\": \"AnInvalidEventType\" }";

            // Assert
            Assert.Throws<JsonException>(() => _sut.ProcessEvent(message));
        }

        private void RegisterServiceProvider()
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler>)))
                .Returns(new [] { _eventHandlerMock.Object });

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
        }
    }
}