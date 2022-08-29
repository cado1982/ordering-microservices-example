using Ordering.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ordering.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IEnumerable<IEventHandler> _eventHandlers;

        public EventProcessor(IEnumerable<IEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        public void ProcessEvent(string message)
        {
            EventType eventType = DetermineEvent(message);

            var eventHandler = _eventHandlers.SingleOrDefault(e => e.EventType == eventType);

            if (eventHandler == null)
            {
                Console.WriteLine($"--> Unable to find event handler for event: {eventType}");
                return;
            }

            eventHandler.Handle(message);
        }

        private EventType DetermineEvent(string eventMessage)
        {
            Console.WriteLine($"--> Determining event type {eventMessage}");

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            var genericEventDto = JsonSerializer.Deserialize<GenericEventDto>(eventMessage, options);

            return genericEventDto!.EventType;
        }
    }
}