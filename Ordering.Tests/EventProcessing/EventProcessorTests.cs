using Ordering.EventProcessing;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Ordering.Tests.EventProcessing
{
    public class EventProcessorTests
    {
        private EventProcessor? _sut;

        private Mock<IEventHandler> _eventHandlerMock = new Mock<IEventHandler>();

        public EventProcessorTests()
        {
            _eventHandlerMock.Setup(h => h.EventType).Returns(EventType.AccountPublished);
        }

        [MemberNotNull(nameof(_sut))]
        private void CreateSUT()
        {
            _sut = new EventProcessor(new[] { _eventHandlerMock.Object });
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
    }
}