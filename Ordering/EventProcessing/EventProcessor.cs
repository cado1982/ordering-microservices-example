using Ordering.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ordering.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public EventProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ProcessEvent(string message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                EventType eventType = DetermineEvent(message);

                var eventHandlers = scope.ServiceProvider.GetServices<IEventHandler>();

                var eventHandler = eventHandlers.SingleOrDefault(e => e.EventType == eventType);

                if (eventHandler == null)
                {
                    Console.WriteLine($"--> Unable to find event handler for event: {eventType}");
                    return;
                }

                eventHandler.Handle(message);
            }

            
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